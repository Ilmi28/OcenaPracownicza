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
            a.Stage2Status,
            EvaluationPeriodName = a.EvaluationPeriod != null ? a.EvaluationPeriod.Name : "Brak okresu",
            EvaluationPeriodId = a.EvaluationPeriodId,

            AchievementElementId = a.AchievementElementId,
            AchievementElementCode = a.AchievementElement != null ? a.AchievementElement.Code : "-",
            AchievementElementBasePoints = a.AchievementElement != null ? a.AchievementElement.BasePoints : 0,
            
            ActivityId = a.AchievementElement != null ? a.AchievementElement.ActivityId : 0,
            DepartmentId = a.AchievementElement != null ? a.AchievementElement.DepartmentId : 0,
            CategoryId = a.AchievementElement != null ? a.AchievementElement.CategoryId : 0
        })
        .ToListAsync();

        return Ok(achievements);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] AddAchievementRequest request, IFormFile? file)
    {
        var period = await periodService.GetPeriodByDateAsync(request.Date);

        if (period == null)
        {
            return BadRequest("Data osiągnięcia nie mieści się w żadnym zdefiniowanym okresie ocen.");
        }

        if (period.IsClosed)
        {
            return BadRequest("Nie można dodawać osiągnięć do zamkniętego (zarchiwizowanego) okresu.");
        }

        Attachment? attachment = null;

        if (file != null && file.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var folderPath = Path.Combine("Storage", "Attachments");

            Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            attachment = new Attachment
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                StoragePath = fullPath
            };

            await context.Attachments.AddAsync(attachment);
            await context.SaveChangesAsync();
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
            Stage2Status = request.IsDraft ? EvaluationStageStatus.Draft : EvaluationStageStatus.PendingStage2,
            AttachmentId = attachment?.Id,
            
            AchievementElementId = request.AchievementElementId
        };

        await context.Achievements.AddAsync(achievement);
        await context.SaveChangesAsync();

        return Ok(new { id = achievement.Id, message = "Osiągnięcie zostało zapisane." });
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

    [HttpGet("attachments/{id}")]
    [Authorize(Roles = "Manager,Admin")] 
    public async Task<IActionResult> GetAttachment(Guid id)
    {
        var attachment = await context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
        if (attachment == null) return NotFound("Nie znaleziono załącznika.");

        if (!System.IO.File.Exists(attachment.StoragePath))
            return NotFound("Plik fizyczny nie istnieje na serwerze.");

        var fileBytes = await System.IO.File.ReadAllBytesAsync(attachment.StoragePath);
        return File(fileBytes, attachment.ContentType, attachment.FileName);
    }
}