using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Dtos;
using OcenaPracownicza.API.Interfaces.Services;
using System.Security.Claims;

namespace OcenaPracownicza.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // wymaga autoryzacji
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("profile")]
        public ActionResult<EmployeeProfileDto> GetProfile()
        {
            // Pobierz ID pracownika z tokena JWT
            var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (employeeIdClaim == null)
                return Unauthorized();

            if (!int.TryParse(employeeIdClaim.Value, out int employeeId))
                return BadRequest("Nieprawidłowy identyfikator użytkownika.");

            var profile = _employeeService.GetProfile(employeeId);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }
    }
}