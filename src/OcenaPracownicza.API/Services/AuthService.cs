using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OcenaPracownicza.API.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserManager _userManager;
    private readonly ITokenService _tokenService;
    public AuthService(IConfiguration configuration, IUserManager userManager, ITokenService tokenService)
    {
        _configuration = configuration;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<string> Login(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            throw new ArgumentException("Username i Password nie mogą być puste.");

        if (request.Username != "admin" || request.Password != "Admin123!")
            throw new UnauthorizedAccessException("Nieprawidłowa nazwa użytkownika lub hasło.");

        var existingUser = await _userManager.FindByNameAsync("Admin");
        if (existingUser != null)
        {
            var roles = await _userManager.GetUserRolesAsync(existingUser.Id);
            return _tokenService.GenerateToken(existingUser, roles);
        }
        var identityUser = new IdentityUser
        {
            UserName = "admin",
            Email = "admin@mail.com"
        };
        var result = await _userManager.CreateAsync(identityUser, "Admin123!");

        if (result)
        {
            result = await _userManager.AddToRoleAsync(identityUser.Id, "Admin");
            var roles = await _userManager.GetUserRolesAsync(identityUser.Id);

            return _tokenService.GenerateToken(identityUser, roles);
        }
        throw new UnauthorizedAccessException("Nie udało się utworzyć użytkownika Admin.");
    }

    public async Task<string> LoginWithGoogle(AuthenticateResult authenticateResult)
    {
        var email = authenticateResult.Principal!.FindFirst(ClaimTypes.Email)?.Value;

        var existingUser = await _userManager.FindByEmailAsync(email!);
        if (existingUser != null)
        {
            var roles = await _userManager.GetUserRolesAsync(existingUser.Id);
            return _tokenService.GenerateToken(existingUser, roles);
        }
        var identityUser = new IdentityUser
        {
            UserName = email, 
            Email = email
        };
        var result = await _userManager.CreateWithoutPassword(identityUser);

        if (result)
        {
            result = await _userManager.AddToRoleAsync(identityUser.Id, "Employee");
            var roles = await _userManager.GetUserRolesAsync(identityUser.Id);

            return _tokenService.GenerateToken(identityUser, roles);
        }
        throw new Exception();
    }
}
