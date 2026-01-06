using System.ComponentModel.DataAnnotations;

namespace CatDiabetesLogger.Models;

public class DailyLog
{
    public int Id { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.Now;

    // Medication
    public bool? SenvelgoGiven { get; set; }
    public DateTime? SenvelgoGivenAt { get; set; }
    public SenvelgoIntakeEstimate? SenvelgoIntake { get; set; }

    // General condition
    public AppetiteLevel? Appetite { get; set; }
    public ThirstLevel? Thirst { get; set; }
    public GeneralState? GeneralState { get; set; }

    // Elimination / GI
    public UrinationLevel? Urination { get; set; }
    public StoolQuality? Stool { get; set; }
    public bool? Vomiting { get; set; }

    // Measurements
    public double? BloodGlucose { get; set; }
    public double? KetoneValue { get; set; }

    // Notes
    public string? Comment { get; set; }

    public double? WeightKg { get; set; }
}

public enum SenvelgoIntakeEstimate
{
    [Display(Name = "Allt")]
    Full = 0,
    
    [Display(Name = "Det mesta")]
    Most = 1,
    
    [Display(Name = "Hälften")]
    Half = 2,
    
    [Display(Name = "Lite")]
    Little = 3,
    
    [Display(Name = "Inget")]
    None = 4
}

public enum AppetiteLevel
{
    [Display(Name = "Mycket dålig")]
    VeryLow = 0,
    
    [Display(Name = "Dålig")]
    Low = 1,
    
    [Display(Name = "Normal")]
    Normal = 2,
    
    [Display(Name = "Bra")]
    Good = 3,
    
    [Display(Name = "Mycket bra")]
    VeryGood = 4
}

public enum ThirstLevel
{
    [Display(Name = "Normal")]
    Normal = 0,
    
    [Display(Name = "Ökad")]
    Increased = 1,
    
    [Display(Name = "Hög")]
    High = 2
}

public enum UrinationLevel
{
    [Display(Name = "Normal")]
    Normal = 0,
    
    [Display(Name = "Ökad")]
    Increased = 1,
    
    [Display(Name = "Hög")]
    High = 2,
    
    [Display(Name = "Trängningar (ofta men lite eller inget)")]
    Urgency = 3
}

public enum StoolQuality
{
    [Display(Name = "Normal")]
    Normal = 0,
    
    [Display(Name = "Lös")]
    Loose = 1,
    
    [Display(Name = "Diarré")]
    Diarrhea = 2,
    
    [Display(Name = "Ingen")]
    None = 3
}

public enum GeneralState
{
    [Display(Name = "Normal")]
    Normal = 0,
    
    [Display(Name = "Trött")]
    Tired = 1,
    
    [Display(Name = "Dålig")]
    Off = 2
}

