using CatDiabetesLogger.Models;

namespace CatDiabetesLogger.ViewModels;

public class DailyLogGroup
{
    public DateOnly Date { get; set; }
    public List<DailyLog> Logs { get; set; } = new();
}
