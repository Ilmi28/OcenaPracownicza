using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Interfaces.Services;
using System.Security.Claims;

namespace OcenaPracownicza.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AchievementController(
    ApplicationDbContext context,
    IEvaluationPeriodService periodService) : ControllerBase
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

        var achievements = await query
        .Select(a => new {
            a.Id,
            a.Name,
            a.Description,
            a.Date,
            a.Category,
            a.FinalScore,
            a.AchievementsSummary,
            a.EmployeeId,
            EvaluationPeriodName = a.EvaluationPeriod != null ? a.EvaluationPeriod.Name : "Brak okresu"
        })
        .ToListAsync();

        return Ok(achievements);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AddAchievementRequest request)
    {
        var period = await periodService.GetPeriodByDateAsync(request.Date);

        if (period == null)
        {
            return BadRequest("Data osi¹gniêcia nie mieœci siê w ¿adnym zdefiniowanym okresie ocen.");
        }

        if (period.IsClosed)
        {
            return BadRequest("Nie mo¿na dodawaæ osi¹gniêæ do zamkniêtego (zarchiwizowanego) okresu.");
        }

        var achievement = new Achievement
        {
            Name = request.Name,
            Description = request.Description,
            Date = request.Date,
            EmployeeId = request.EmployeeId,
            Category = request.Category,
            EvaluationPeriodId = period.Id,   
            FinalScore = request.FinalScore,
            AchievementsSummary = request.AchievementsSummary,
            Stage2Status = EvaluationStageStatus.PendingStage2

        };

        await context.Achievements.AddAsync(achievement);
        await context.SaveChangesAsync();

        return Ok(new { id = achievement.Id, message = "Osi¹gniêcie zosta³o zapisane." });
    }

    [HttpGet("employee-dropdown")]
    [Authorize(Roles = "Manager,Admin")]       
    public async Task<IActionResult> GetEmployeeDropdown()
    {
        var employees = await context.Employees.Select(e => new
        {
            e.Id,
            e.FirstName,
            e.LastName
        }).ToListAsync();

        return Ok(employees);
    }
}