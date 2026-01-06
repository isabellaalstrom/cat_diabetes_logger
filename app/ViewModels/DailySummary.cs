using CatDiabetesLogger.Models;

namespace CatDiabetesLogger.ViewModels;

public class DailySummary
{
    public DateOnly Date { get; set; }

    public int LogCount { get; set; }

    // Senvelgo
    public bool SenvelgoGiven { get; set; }
    public List<SenvelgoIntakeEstimate> SenvelgoIntakes { get; set; } = new();
    public DateTime? LastSenvelgoTime { get; set; }

    // Allmänt tillstånd
    public List<AppetiteLevel> AppetiteLevels { get; set; } = new();
    public List<ThirstLevel> ThirstLevels { get; set; } = new();
    public List<GeneralState> GeneralStates { get; set; } = new();

    // Låda
    public List<UrinationLevel> UrinationLevels { get; set; } = new();
    public List<StoolQuality> StoolQualities { get; set; } = new();
    public bool VomitingOccurred { get; set; }

    // Mätvärden
    public List<double> BloodGlucoseReadings { get; set; } = new();
    public List<double> KetoneReadings { get; set; } = new();
    public double? MinGlucose { get; set; }
    public double? MaxGlucose { get; set; }
    public double? AvgGlucose { get; set; }
    public double? MinKetones { get; set; }
    public double? MaxKetones { get; set; }
    public double? AvgKetones { get; set; }

    // Vikt
    public List<double> WeightReadings { get; set; } = new();
    public double? MinWeight { get; set; }
    public double? MaxWeight { get; set; }
    public double? LastWeight { get; set; }
}
