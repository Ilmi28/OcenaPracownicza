namespace OcenaPracownicza.API.Views;

public class AchievementStage2View
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Category { get; set; }
    public int Stage2Status { get; set; }
    public string? Stage2Comment { get; set; }
}
