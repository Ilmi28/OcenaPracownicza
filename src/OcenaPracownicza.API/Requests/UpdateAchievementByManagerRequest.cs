using OcenaPracownicza.API.Enums;

namespace OcenaPracownicza.API.Requests;

public class UpdateAchievementByManagerRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public AchievementCategory? Category { get; set; }
    public string? FinalScore { get; set; }
    public string? AchievementsSummary { get; set; }
    public string? Stage2Comment { get; set; }
}