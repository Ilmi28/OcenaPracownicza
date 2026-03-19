using Microsoft.AspNetCore.Identity;
using OcenaPracownicza.API.Enums;

namespace OcenaPracownicza.API.Entities
{
    public class Employee : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Position { get; set; }
        public required string Period { get; set; }
        public required string FinalScore { get; set; }
        public required string AchievementsSummary { get; set; }
        public EvaluationStageStatus Stage2Status { get; set; } = EvaluationStageStatus.Draft;
        public string? Stage2Comment { get; set; }
        public string? Stage2ReviewedByUserId { get; set; }
        public DateTime? Stage2ReviewedAtUtc { get; set; }
        public Guid? ManagerId { get; set; }
        public Manager? Manager { get; set; }
        public required string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; } = null!;
    }
}
