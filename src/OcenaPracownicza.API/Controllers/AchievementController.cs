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
        .OrderByDescending(a => a.Date)
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
            Stage2Comment = a.Stage2Comment,
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromForm] AddAchievementRequest request)
    {
        var achievement = await context.Achievements
            .FirstOrDefaultAsync(a => a.Id == id);

        if (achievement == null)
        {
            return NotFound("Nie znaleziono modyfikowanego osiągnięcia.");
        }

        var period = await periodService.GetPeriodByDateAsync(request.Date);

        if (period == null)
        {
            return BadRequest("Data osiągnięcia nie mieści się w żadnym zdefiniowanym okresie ocen.");
        }

        if (period.IsClosed)
        {
            return BadRequest("Nie można edytować osiągnięć w zamkniętym (zarchiwizowanym) okresie.");
        }

        achievement.Name = request.Name;
        achievement.Description = request.Description;
        achievement.Date = request.Date;
        achievement.EmployeeId = request.EmployeeId;
        achievement.Category = request.Category;
        achievement.EvaluationPeriodId = period.Id;
        achievement.FinalScore = request.FinalScore;
        achievement.AchievementsSummary = request.Description.Length > 100
            ? request.Description.Substring(0, 100)
            : request.Description;

        achievement.Stage2Status = request.IsDraft
            ? EvaluationStageStatus.Draft
            : EvaluationStageStatus.PendingStage2;

        context.Achievements.Update(achievement);
        await context.SaveChangesAsync();

        return Ok(new { message = "Osiągnięcie zostało pomyślnie zaktualizowane." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var achievement = await context.Achievements.FirstOrDefaultAsync(a => a.Id == id);

        if (achievement == null)
        {
            return NotFound("Nie znaleziono osiągnięcia o podanym ID.");
        }

        if (achievement.Stage2Status != EvaluationStageStatus.Draft)
        {
            return BadRequest("Można usuwać wyłącznie osiągnięcia o statusie Szkic (Draft).");
        }

        context.Achievements.Remove(achievement);
        await context.SaveChangesAsync();

        return Ok(new { message = "Szkic został pomyślnie usunięty." });
    }

    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        var categories = Enum.GetValues(typeof(AchievementCategory))
            .Cast<AchievementCategory>()
            .Select(c => new
            {
                Id = (int)c,
                Name = c switch
                {
                    AchievementCategory.ProjectDelivery => "Sukces projektowy (Project Delivery)",
                    AchievementCategory.TechnicalGrowth => "Rozwój techniczny (Technical Growth)",
                    AchievementCategory.ProcessImprovement => "Ulepszenie procesu (Process Improvement)",
                    AchievementCategory.Mentorship => "Mentoring (Mentorship)",
                    AchievementCategory.Innovation => "Innowacja (Innovation)",
                    AchievementCategory.Leadership => "Liderowanie (Leadership)",
                    AchievementCategory.CustomerSuccess => "Sukces klienta (Customer Success)",
                    _ => c.ToString()
                }
            }).ToList();

        return Ok(categories);
    }

    [HttpPost("copy-package")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CopyPackage([FromBody] CopyPackageRequest request)
    {
        if (request.SourcePeriodId == request.TargetPeriodId)
        {
            return BadRequest("Okres źródłowy i docelowy nie mogą być takie same.");
        }

        var sourceElements = await context.AchievementElements
            .Where(e => e.EvaluationPeriodId == request.SourcePeriodId)
            .ToListAsync();

        if (!sourceElements.Any())
        {
            return BadRequest("Wybrany okres źródłowy nie zawiera żadnych szablonów do skopiowania.");
        }

        var existingCodes = await context.AchievementElements
            .Where(e => e.EvaluationPeriodId == request.TargetPeriodId)
            .Select(e => e.Code)
            .ToListAsync();

        var newElements = new List<AchievementElement>();

        foreach (var src in sourceElements)
        {
            if (existingCodes.Contains(src.Code)) continue;

            newElements.Add(new AchievementElement
            {
                Code = src.Code,
                Name = src.Name,
                ActivityId = src.ActivityId,
                DepartmentId = src.DepartmentId,
                CategoryId = src.CategoryId,
                BasePoints = src.BasePoints,
                IsStackable = src.IsStackable,
                EvaluationPeriodId = request.TargetPeriodId     
            });
        }

        if (newElements.Any())
        {
            await context.AchievementElements.AddRangeAsync(newElements);
            await context.SaveChangesAsync();
        }

        return Ok(new { message = $"Pomyślnie skopiowano {newElements.Count} szablonów." });
    }

    public class CopyPackageRequest
    {
        public Guid SourcePeriodId { get; set; }
        public Guid TargetPeriodId { get; set; }
    }
}