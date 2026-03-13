using Microsoft.AspNetCore.Identity;

namespace OcenaPracownicza.API.Entities
{
    public class Manager : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string AchievementsSummary { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public required string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; } = null!;
    }
}
