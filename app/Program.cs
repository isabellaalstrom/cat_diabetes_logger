using CatDiabetesLogger.Data;
using CatDiabetesLogger.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

// Registrera HomeAssistantService
builder.Services.AddScoped<HomeAssistantService>();

// Kolla om vi kör i Home Assistant genom att leta efter SUPERVISOR_TOKEN
var isHomeAssistant = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SUPERVISOR_TOKEN"));

if (isHomeAssistant)
{
    // Home Assistant addon - använd /config katalogen
    var configPath = "/config";
    Directory.CreateDirectory(configPath);
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite($"Data Source={configPath}/app.db"));
    
    Console.WriteLine($"Running in Home Assistant mode - DB path: {configPath}/app.db");
}
else
{
    // Lokal utveckling - använd app.db i projektmappen
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=app.db"));
    
    Console.WriteLine("Running in local development mode - DB path: app.db");
}

var app = builder.Build();

app.UseForwardedHeaders();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Skapa DB vid start
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
