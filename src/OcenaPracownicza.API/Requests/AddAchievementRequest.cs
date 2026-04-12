using OcenaPracownicza.API.Enums;

namespace OcenaPracownicza.API.Requests;

public class AddAchievementRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Date { get; set; }
    public Guid EmployeeId { get; set; }
    public AchievementCategory Category { get; set; }
    public string Period { get; set; } = null!;
    public string FinalScore { get; set; } = null!;
    public string AchievementsSummary { get; set; } = null!;
}
