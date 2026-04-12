namespace OcenaPracownicza.API.Views;

public class AchievementStage2View
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Category { get; set; }
    public string Period { get; set; } = string.Empty;
    public string FinalScore { get; set; } = string.Empty;
    public string AchievementsSummary { get; set; } = string.Empty;
    public int Stage2Status { get; set; }
    public string? Stage2Comment { get; set; }
    public string? Stage2ReviewedByUserId { get; set; }
    public DateTime? Stage2ReviewedAtUtc { get; set; }
}
