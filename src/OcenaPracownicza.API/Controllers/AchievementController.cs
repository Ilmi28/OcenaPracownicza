using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Requests;
using System.Security.Claims;

namespace OcenaPracownicza.API.Controllers;

[ApiController]
[Authorize]
[Route("api/achievement")]
public class AchievementController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrEmpty(identityUserId)) return Unauthorized();

        var employee = await context.Employees
            .FirstOrDefaultAsync(e => e.IdentityUserId == identityUserId);

        var query = context.Achievements.AsQueryable();

        if (userRole != "Admin" && userRole != "Manager")
        {
            if (employee == null) return Ok(new List<Achievement>());

            query = query.Where(a => a.EmployeeId == employee.Id);
        }

        var achievements = await query.ToListAsync();
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
            Category = request.Category,
            Period = request.Period,
            FinalScore = request.FinalScore,
            AchievementsSummary = request.AchievementsSummary,
            Stage2Status = EvaluationStageStatus.PendingStage2

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
