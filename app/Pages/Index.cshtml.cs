using CatDiabetesLogger.Data;
using CatDiabetesLogger.Models;
using CatDiabetesLogger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CatDiabetesLogger.ViewModels;

namespace CatDiabetesLogger.Pages;

[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly HomeAssistantService _homeAssistant;

    public IndexModel(AppDbContext db, HomeAssistantService homeAssistant)
    {
        _db = db;
        _homeAssistant = homeAssistant;
    }
    
    // Bind mot samma typer som modellen
    [BindProperty]
    public bool? SenvelgoGiven { get; set; }

    [BindProperty]
    public SenvelgoIntakeEstimate? SenvelgoIntake { get; set; }

    [BindProperty]
    public TimeOnly? SenvelgoTime { get; set; }

    [BindProperty]
    public AppetiteLevel? Appetite { get; set; }

    [BindProperty]
    public ThirstLevel? Thirst { get; set; }

    [BindProperty]
    public GeneralState? GeneralState { get; set; }

    [BindProperty]
    public UrinationLevel? Urination { get; set; }

    [BindProperty]
    public StoolQuality? Stool { get; set; }

    [BindProperty]
    public bool? Vomiting { get; set; }

    [BindProperty]
    public double? BloodGlucose { get; set; }

    [BindProperty]
    public double? KetoneValue { get; set; }

    [BindProperty]
    public double? WeightKg { get; set; }

    [BindProperty]
    public string? Comment { get; set; }

    public bool Saved { get; set; }
    public string? ErrorMessage { get; set; }
    public bool SenvelgoAlreadyGivenToday { get; set; }
    public List<DailyLogGroup> GroupedLogs { get; set; } = new();
    public List<DailySummary> DailySummaries { get; set; } = new();

    private void LoadHistory()
    {
        var logs = _db.DailyLogs
            .OrderByDescending(l => l.LoggedAt)
            .Take(200)
            .ToList();

        GroupedLogs = logs
            .GroupBy(l => DateOnly.FromDateTime(l.LoggedAt))
            .OrderByDescending(g => g.Key)
            .Select(g => new DailyLogGroup
            {
                Date = g.Key,
                Logs = g.OrderBy(l => l.LoggedAt).ToList()
            })
            .ToList();

        DailySummaries = GroupedLogs.Select(g => new DailySummary
        {
            Date = g.Date,
            LogCount = g.Logs.Count,

            // Senvelgo
            SenvelgoGiven = g.Logs.Any(l => l.SenvelgoGiven == true),
            SenvelgoIntakes = g.Logs
                .Where(l => l.SenvelgoIntake.HasValue)
                .Select(l => l.SenvelgoIntake!.Value)
                .Distinct()
                .OrderBy(i => i)
                .ToList(),
            LastSenvelgoTime = g.Logs
                .Where(l => l.SenvelgoGiven == true)
                .OrderByDescending(l => l.SenvelgoGivenAt ?? l.LoggedAt)
                .Select(l => l.SenvelgoGivenAt ?? l.LoggedAt)
                .FirstOrDefault(),

            // Allmänt
            AppetiteLevels = g.Logs
                .Where(l => l.Appetite.HasValue)
                .Select(l => l.Appetite!.Value)
                .Distinct()
                .OrderBy(a => a)
                .ToList(),
            ThirstLevels = g.Logs
                .Where(l => l.Thirst.HasValue)
                .Select(l => l.Thirst!.Value)
                .Distinct()
                .OrderBy(t => t)
                .ToList(),
            GeneralStates = g.Logs
                .Where(l => l.GeneralState.HasValue)
                .Select(l => l.GeneralState!.Value)
                .Distinct()
                .OrderBy(s => s)
                .ToList(),

            // Låda
            UrinationLevels = g.Logs
                .Where(l => l.Urination.HasValue)
                .Select(l => l.Urination!.Value)
                .Distinct()
                .OrderBy(u => u)
                .ToList(),
            StoolQualities = g.Logs
                .Where(l => l.Stool.HasValue)
                .Select(l => l.Stool!.Value)
                .Distinct()
                .OrderBy(s => s)
                .ToList(),
            VomitingOccurred = g.Logs.Any(l => l.Vomiting == true),

            // Mätvärden - Glukos
            BloodGlucoseReadings = g.Logs
                .Where(l => l.BloodGlucose.HasValue)
                .Select(l => l.BloodGlucose!.Value)
                .OrderBy(v => v)
                .ToList(),
            MinGlucose = g.Logs
                .Where(l => l.BloodGlucose.HasValue)
                .Select(l => l.BloodGlucose)
                .Min(),
            MaxGlucose = g.Logs
                .Where(l => l.BloodGlucose.HasValue)
                .Select(l => l.BloodGlucose)
                .Max(),
            AvgGlucose = g.Logs
                .Where(l => l.BloodGlucose.HasValue)
                .Select(l => l.BloodGlucose!.Value)
                .DefaultIfEmpty()
                .Average(),

            // Mätvärden - Ketoner
            KetoneReadings = g.Logs
                .Where(l => l.KetoneValue.HasValue)
                .Select(l => l.KetoneValue!.Value)
                .OrderBy(v => v)
                .ToList(),
            MinKetones = g.Logs
                .Where(l => l.KetoneValue.HasValue)
                .Select(l => l.KetoneValue)
                .Min(),
            MaxKetones = g.Logs
                .Where(l => l.KetoneValue.HasValue)
                .Select(l => l.KetoneValue)
                .Max(),
            AvgKetones = g.Logs
                .Where(l => l.KetoneValue.HasValue)
                .Select(l => l.KetoneValue!.Value)
                .DefaultIfEmpty()
                .Average(),

            // Vikt
            WeightReadings = g.Logs
                .Where(l => l.WeightKg.HasValue)
                .Select(l => l.WeightKg!.Value)
                .OrderBy(w => w)
                .ToList(),
            MinWeight = g.Logs
                .Where(l => l.WeightKg.HasValue)
                .Select(l => l.WeightKg)
                .Min(),
            MaxWeight = g.Logs
                .Where(l => l.WeightKg.HasValue)
                .Select(l => l.WeightKg)
                .Max(),
            LastWeight = g.Logs
                .Where(l => l.WeightKg.HasValue)
                .OrderByDescending(l => l.LoggedAt)
                .Select(l => l.WeightKg)
                .FirstOrDefault()

        }).ToList();

        // Kolla om Senvelgo redan givits idag
        var today = DateOnly.FromDateTime(DateTime.Today);
        SenvelgoAlreadyGivenToday = _db.DailyLogs
            .Any(l => DateOnly.FromDateTime(l.LoggedAt) == today && l.SenvelgoGiven == true);
    }

    public async Task<IActionResult> OnGetAsync(int? deleteId)
    {
        // Hantera delete via GET parameter
        if (deleteId.HasValue)
        {
            Console.WriteLine($"=== DELETE START === ID: {deleteId.Value}");
            
            var log = await _db.DailyLogs.FindAsync(deleteId.Value);
            
            if (log != null)
            {
                _db.DailyLogs.Remove(log);
                await _db.SaveChangesAsync();
                Console.WriteLine($"Deleted log with ID: {deleteId.Value}");
            }
            
            return RedirectToPage();
        }

        LoadHistory();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine("=== POST START ===");
        Console.WriteLine($"SenvelgoGiven: {SenvelgoGiven}");
        Console.WriteLine($"SenvelgoTime: {SenvelgoTime}");
        Console.WriteLine($"Vomiting: {Vomiting}");

        // Validera att Senvelgo inte redan givits idag
        if (SenvelgoGiven == true)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var senvelgoGivenToday = _db.DailyLogs
                .Any(l => DateOnly.FromDateTime(l.LoggedAt) == today && l.SenvelgoGiven == true);

            if (senvelgoGivenToday)
            {
                ErrorMessage = "Senvelgo har redan givits idag!";
                LoadHistory();
                return Page();
            }
        }

        DateTime? senvelgoGivenAt = null;

        if (SenvelgoGiven == true)
        {
            senvelgoGivenAt = SenvelgoTime.HasValue
                ? DateTime.Today.Add(SenvelgoTime.Value.ToTimeSpan())
                : DateTime.Now;
        }

        var log = new DailyLog
        {
            LoggedAt = DateTime.Now,

            SenvelgoGiven = SenvelgoGiven,
            SenvelgoGivenAt = senvelgoGivenAt,
            SenvelgoIntake = SenvelgoIntake,

            Appetite = Appetite,
            Thirst = Thirst,
            GeneralState = GeneralState,

            Urination = Urination,
            Stool = Stool,
            Vomiting = Vomiting,

            BloodGlucose = BloodGlucose,
            KetoneValue = KetoneValue,

            WeightKg = WeightKg,

            Comment = Comment
        };

        _db.DailyLogs.Add(log);
        _db.SaveChanges();

        // Skicka till Home Assistant
        await _homeAssistant.SendLogToHomeAssistant(log);

        // Visa sidan igen UTAN redirect för att undvika iframe-problem
        Saved = true;
        LoadHistory();
        return Page();
    }
}
