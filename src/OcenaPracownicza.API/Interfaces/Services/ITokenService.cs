using Microsoft.AspNetCore.Identity;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(IdentityUser user, IList<string> roles);
    }
}
