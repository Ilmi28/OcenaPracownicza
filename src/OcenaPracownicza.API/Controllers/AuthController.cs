using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OcenaPracownicza.API.Controllers;

[Authorize]  
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Requests.LoginRequest request)
    {
        var token = await authService.Login(request);

        Response.Cookies.Append("jwt", token.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true, 
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return Ok(new { Token = token });
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
