namespace OcenaPracownicza.API.Views;

public class Stage2ReviewDetailsView
{
    public Guid AchievementId { get; set; }
    public Guid EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string AchievementName { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string FinalScore { get; set; } = string.Empty;
    public string AchievementsSummary { get; set; } = string.Empty;
    public int Stage2Status { get; set; }
    public string? Stage2Comment { get; set; }
    public string? Stage2ReviewedByUserId { get; set; }
    public DateTime? Stage2ReviewedAtUtc { get; set; }
    public List<AchievementStage2View> Achievements { get; set; } = [];
}
