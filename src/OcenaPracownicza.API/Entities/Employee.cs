using Microsoft.AspNetCore.Identity;

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
        public required string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; } = null!;
    }
}
