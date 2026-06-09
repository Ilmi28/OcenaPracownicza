using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AchievementElementController(ApplicationDbContext context) : ControllerBase
{
    private string GetName(int id, List<AchievementDictionary.Definition> source)
        => source.FirstOrDefault(x => x.Id == id)?.Label ?? "Nieznane";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var elements = await context.AchievementElements
            .Include(e => e.EvaluationPeriod)
            .ToListAsync();

        var response = elements.Select(e => new AchievementElementResponse
        {
            Id = e.Id,
            Code = e.Code,
            Activity = e.ActivityId,     
            ActivityName = GetName(e.ActivityId, AchievementDictionary.Activities),
            Department = e.DepartmentId,     
            DepartmentName = GetName(e.DepartmentId, AchievementDictionary.Departments),
            Category = e.CategoryId,     
            CategoryName = GetName(e.CategoryId, AchievementDictionary.Categories),
            Name = e.Name,
            BasePoints = e.BasePoints,
            IsStackable = e.IsStackable,
            EvaluationPeriodId = e.EvaluationPeriodId,
            EvaluationPeriodName = e.EvaluationPeriod?.Name ?? "Brak okresu"
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var element = await context.AchievementElements
            .Include(e => e.EvaluationPeriod)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (element == null) return NotFound("Nie znaleziono szablonu.");

        return Ok(new AchievementElementResponse
        {
            Id = element.Id,
            Code = element.Code,
            Activity = element.ActivityId,
            ActivityName = GetName(element.ActivityId, AchievementDictionary.Activities),
            Department = element.DepartmentId,
            DepartmentName = GetName(element.DepartmentId, AchievementDictionary.Departments),
            Category = element.CategoryId,
            CategoryName = GetName(element.CategoryId, AchievementDictionary.Categories),
            Name = element.Name,
            BasePoints = element.BasePoints,
            IsStackable = element.IsStackable,
            EvaluationPeriodId = element.EvaluationPeriodId,
            EvaluationPeriodName = element.EvaluationPeriod?.Name ?? "Brak"
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post([FromBody] AddUpdateAchievementElementRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var element = new AchievementElement
        {
            Code = request.Code,
            ActivityId = request.Activity,     
            DepartmentId = request.Department,
            CategoryId = request.Category,
            Name = request.Name,
            BasePoints = request.BasePoints,
            IsStackable = request.IsStackable,
            EvaluationPeriodId = request.EvaluationPeriodId
        };

        await context.AchievementElements.AddAsync(element);
        await context.SaveChangesAsync();

        return Ok(new { id = element.Id, message = "Dodano nowe kryterium." });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(Guid id, [FromBody] AddUpdateAchievementElementRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var element = await context.AchievementElements.FirstOrDefaultAsync(e => e.Id == id);
        if (element == null) return NotFound();

        element.Code = request.Code;
        element.ActivityId = request.Activity;
        element.DepartmentId = request.Department;
        element.CategoryId = request.Category;
        element.Name = request.Name;
        element.BasePoints = request.BasePoints;
        element.IsStackable = request.IsStackable;
        element.EvaluationPeriodId = request.EvaluationPeriodId;

        await context.SaveChangesAsync();
        return Ok(new { message = "Zaktualizowano pomyślnie." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var element = await context.AchievementElements.FirstOrDefaultAsync(e => e.Id == id);
        if (element == null) return NotFound();

        var isUsed = await context.Achievements.AnyAsync(a => a.AchievementElementId == id);
        if (isUsed) return BadRequest("Kryterium jest używane.");

        context.AchievementElements.Remove(element);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("dictionary")]
    [AllowAnonymous]          
    public IActionResult GetDictionary()
    {
        return Ok(new
        {
            Activities = AchievementDictionary.Activities,
            Departments = AchievementDictionary.Departments,
            Categories = AchievementDictionary.Categories
        });
    }
}