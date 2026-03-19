namespace OcenaPracownicza.API.Views;

public class Stage2ReviewItemView
{
    public Guid EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string FinalScore { get; set; } = string.Empty;
    public int Stage2Status { get; set; }
    public int AchievementsCount { get; set; }
}
