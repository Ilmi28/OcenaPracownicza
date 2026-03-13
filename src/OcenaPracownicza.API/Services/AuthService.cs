using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using System.Security.Claims;

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

    public async Task<string> Register(RegisterRequest request)
    {
        var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingUserByEmail != null)
        {
            throw new ArgumentException("Email is already in use.");
        }
        var existingUserByUsername = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserByUsername != null)
        {
            throw new ArgumentException("Username is already in use.");
        }
        var newUser = new IdentityUser
        {
            UserName = request.UserName,
            Email = request.Email
        };
        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result)
        {
            throw new Exception("User registration failed.");
        }
        result = await _userManager.AddToRoleAsync(newUser.Id, "Guest");
        if (!result)
        {
            throw new Exception("Assigning role failed.");
        }
        var roles = await _userManager.GetUserRolesAsync(newUser.Id);
        return _tokenService.GenerateToken(newUser, roles);
    }
}
