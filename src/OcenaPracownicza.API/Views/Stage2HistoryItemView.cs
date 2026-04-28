namespace OcenaPracownicza.API.Views;

public class Stage2HistoryItemView
{
    public Guid AchievementId { get; set; }
    public Guid EmployeeId { get; set; }
    public string FullName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public string AchievementName { get; set; } = null!;
    public string Period { get; set; } = null!;
    public string FinalScore { get; set; } = null!;
    public int Stage2Status { get; set; }
    public DateTime Date { get; set; }
    public DateTime? Stage2ReviewedAtUtc { get; set; }
}
