using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CatDiabetesLogger.Models;
using CatDiabetesLogger.Extensions;

namespace CatDiabetesLogger.Services;

public class HomeAssistantService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HomeAssistantService> _logger;

    public HomeAssistantService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<HomeAssistantService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendLogToHomeAssistant(DailyLog log)
    {
        try
        {
            var supervisorToken = Environment.GetEnvironmentVariable("SUPERVISOR_TOKEN");
            if (string.IsNullOrEmpty(supervisorToken))
            {
                _logger.LogWarning("SUPERVISOR_TOKEN not found, skipping Home Assistant integration");
                return;
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", supervisorToken);

            // Skicka data till olika sensors
            await SendSensorState(client, "sensor.cat_last_log_time", log.LoggedAt.ToString("yyyy-MM-ddTHH:mm:ss"));
            
            if (log.SenvelgoGiven == true)
            {
                await SendSensorState(client, "sensor.cat_senvelgo_given", "on", new Dictionary<string, object>
                {
                    ["last_given"] = log.SenvelgoGivenAt?.ToString("yyyy-MM-ddTHH:mm:ss") ?? log.LoggedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ["intake"] = log.SenvelgoIntake?.GetDisplayName() ?? "Okänt"
                });
                
                // Skicka även en event
                await FireEvent(client, "cat_medication_given", new
                {
                    medication = "Senvelgo",
                    timestamp = log.SenvelgoGivenAt ?? log.LoggedAt,
                    intake = log.SenvelgoIntake?.ToString() ?? "unknown"
                });
            }

            if (log.BloodGlucose.HasValue)
            {
                await SendSensorState(client, "sensor.cat_blood_glucose", log.BloodGlucose.Value.ToString("F1"), new Dictionary<string, object>
                {
                    ["unit_of_measurement"] = "mmol/L",
                    ["device_class"] = "measurement",
                    ["timestamp"] = log.LoggedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                });
            }

            if (log.KetoneValue.HasValue)
            {
                await SendSensorState(client, "sensor.cat_ketones", log.KetoneValue.Value.ToString("F1"), new Dictionary<string, object>
                {
                    ["unit_of_measurement"] = "mmol/L",
                    ["device_class"] = "measurement",
                    ["timestamp"] = log.LoggedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                });
            }

            if (log.WeightKg.HasValue)
            {
                await SendSensorState(client, "sensor.cat_weight", log.WeightKg.Value.ToString("F1"), new Dictionary<string, object>
                {
                    ["unit_of_measurement"] = "kg",
                    ["device_class"] = "weight",
                    ["timestamp"] = log.LoggedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                });
            }

            if (log.Appetite.HasValue)
            {
                await SendSensorState(client, "sensor.cat_appetite", log.Appetite.Value.GetDisplayName(), new Dictionary<string, object>
                {
                    ["level"] = (int)log.Appetite.Value,
                    ["timestamp"] = log.LoggedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                });
            }

            if (log.Thirst.HasValue)
            {
                await SendSensorState(client, "sensor.cat_thirst", log.Thirst.Value.GetDisplayName(), new Dictionary<string, object>
                {
                    ["level"] = (int)log.Thirst.Value,
                    ["timestamp"] = log.LoggedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                });
            }

            if (log.GeneralState.HasValue)
            {
                await SendSensorState(client, "sensor.cat_general_state", log.GeneralState.Value.GetDisplayName(), new Dictionary<string, object>
                {
                    ["level"] = (int)log.GeneralState.Value,
                    ["timestamp"] = log.LoggedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                });
            }

            if (log.Vomiting == true)
            {
                await FireEvent(client, "cat_vomiting", new
                {
                    timestamp = log.LoggedAt
                });
            }

            _logger.LogInformation("Successfully sent log data to Home Assistant");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending data to Home Assistant");
        }
    }

    private async Task SendSensorState(HttpClient client, string entityId, string state, Dictionary<string, object>? attributes = null)
    {
        try
        {
            var payload = new
            {
                state = state,
                attributes = attributes ?? new Dictionary<string, object>()
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                $"http://supervisor/core/api/states/{entityId}",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to update {EntityId}: {StatusCode} - {Error}", 
                    entityId, response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending sensor state for {EntityId}", entityId);
        }
    }

    private async Task FireEvent(HttpClient client, string eventType, object eventData)
    {
        try
        {
            var json = JsonSerializer.Serialize(eventData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                $"http://supervisor/core/api/events/{eventType}",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to fire event {EventType}: {StatusCode} - {Error}", 
                    eventType, response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error firing event {EventType}", eventType);
        }
    }
}   