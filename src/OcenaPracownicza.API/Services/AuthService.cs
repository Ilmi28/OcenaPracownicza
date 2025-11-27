using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OcenaPracownicza.API.Services;

public class AuthService : IAuthService
{
    public IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Login(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            throw new ArgumentException("Username i Password nie mogą być puste.");


        if (request.Username == "admin" && request.Password == "admin123")
        {
            var adminClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };

            return GenerateJwtToken(adminClaims);
        }


        if (request.Username == "employee" && request.Password == "employee123")
        {
            var employeeClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Employee"),
                new Claim(ClaimTypes.NameIdentifier, "10")  
            };

            return GenerateJwtToken(employeeClaims);
        }

        throw new UnauthorizedAccessException("Nieprawidłowa nazwa użytkownika lub hasło.");
    }

    public string LoginWithGoogle(AuthenticateResult result)
    {
        var email = result.Principal!.FindFirst(ClaimTypes.Email)?.Value;
        var name = result.Principal!.FindFirst(ClaimTypes.Name)?.Value;
        var nameIdentifier = result.Principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var picture = result.Principal!.FindFirst("picture")?.Value;

        return GenerateJwtToken(new[]
        {
            new Claim(ClaimTypes.Email, email ?? ""),
            new Claim(ClaimTypes.Name, name ?? ""),
            new Claim(ClaimTypes.NameIdentifier, nameIdentifier ?? ""),
            new Claim("picture", picture ?? ""),
            new Claim(ClaimTypes.Role, "User")
        });
    }

    private string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
