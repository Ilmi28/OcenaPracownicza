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
    private readonly IUserManager _userManager;
    private readonly ITokenService _tokenService;
    public AuthService(IUserManager userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<string> Login(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.UserNameEmail) || string.IsNullOrEmpty(request.Password))
            throw new ArgumentException("Username and Password cannot be empty.");

        var user = await _userManager.FindByEmailAsync(request.UserNameEmail);

        if (user == null)
        {
            user = await _userManager.FindByNameAsync(request.UserNameEmail);
        }

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user.Id, request.Password);

        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        var roles = await _userManager.GetUserRolesAsync(user.Id);
        return _tokenService.GenerateToken(user, roles);
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
