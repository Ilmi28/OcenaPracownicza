using Microsoft.AspNetCore.Identity;

namespace OcenaPracownicza.API.Entities
{
    public class Admin : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; } = null!;
    }
}
