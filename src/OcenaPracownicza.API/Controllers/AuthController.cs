using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Services;
using OcenaPracownicza.API.Views;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OcenaPracownicza.API.Controllers;
  
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Requests.LoginRequest request)
    {
        var token = await authService.Login(request);

        Response.Cookies.Append("jwt", token.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        var dto = new LoginView
        {
            Token = token
        };

        return Ok(new LoginResponse
        {
            Data = dto
        });
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

        var dto = new LoginView
        {
            Token = token
        };

        return Ok(new LoginResponse
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
}
