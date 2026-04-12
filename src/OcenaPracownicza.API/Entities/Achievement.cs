using OcenaPracownicza.API.Enums;

namespace OcenaPracownicza.API.Entities;

public class Achievement : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Date { get; set; }
    public AchievementCategory Category { get; set; }
    public string Period { get; set; } = null!;
    public string FinalScore { get; set; } = null!;
    public string AchievementsSummary { get; set; } = null!;
    public EvaluationStageStatus Stage2Status { get; set; } = EvaluationStageStatus.Draft;
    public string? Stage2Comment { get; set; }
    public string? Stage2ReviewedByUserId { get; set; }
    public DateTime? Stage2ReviewedAtUtc { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
}
