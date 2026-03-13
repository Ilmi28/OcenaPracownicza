using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Requests;

namespace OcenaPracownicza.API.Controllers;

[ApiController]
[Route("api/achievement")]
public class AchievementController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var achievements = context.Achievements.ToList();
        return Ok(achievements);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AddAchievementRequest request)
    {
        var achievement = new Achievement
        {
            Name = request.Name,
            Description = request.Description,
            Date = request.Date,
            EmployeeId = request.EmployeeId,
            Category = request.Category

        };
        await context.Achievements.AddAsync(achievement);
        await context.SaveChangesAsync();
        return Ok(achievement);
    }

    [HttpGet("employee-dropdown")]
    public IActionResult GetEmployeeDropdown()
    {
        var employees = context.Employees.Select(e => new
        {
            e.Id,
            e.FirstName,
            e.LastName
        }).ToList();
        return Ok(employees);
    }
}
