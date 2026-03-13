using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;
using System.Security.Claims;

namespace OcenaPracownicza.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await authService.Login(request);

        Response.Cookies.Append("jwt", token.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        var dto = new LoginRegisterView
        {
            Token = token
        };

        return Ok(new LoginRegisterResponse
        {
            Data = dto
        });
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        return Ok(new { Message = "Wylogowano pomyślnie" });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMyInfo()
    {
        var userName = User.Identity?.Name;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new { userId, userName, email, role });
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var token = await authService.LoginWithGoogle(result);

        Response.Cookies.Append("jwt", token.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        var dto = new LoginRegisterView
        {
            Token = token
        };

        return Ok(new LoginRegisterResponse
        {
            Data = dto
        });
    }

    [HttpGet("secure")]
    public IActionResult Secure()
    {
        return Ok("Dostęp tylko dla zalogowanych użytkowników");
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Administrator")]
    public IActionResult Admin()
    {
        return Ok("Dostęp tylko dla administratorów");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var token = await authService.Register(request);

        Response.Cookies.Append("jwt", token.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return Ok(new LoginRegisterResponse
        {
            Data = new LoginRegisterView
            {
                Token = token
            }
        });
    }
}
