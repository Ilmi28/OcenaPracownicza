using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;

namespace OcenaPracownicza.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/user")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            return Ok(await userService.GetById(id));
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            return Ok(await userService.GetCurrentUser());
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            return Ok(await userService.ChangePassword(request));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await userService.GetAll());
        }
    }
}
