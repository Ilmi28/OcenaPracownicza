using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReportsController(
    IEmployeeRepository employeeRepository,
    ApplicationDbContext context,
    IDocumentGeneratorService documentGeneratorService) : ControllerBase
{
    private async Task<bool> CanAccessEmployee(Guid actualEmployeeId)
    {
        if (User.IsInRole("Admin") || User.IsInRole("Manager")) return true;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.IdentityUserId == userId);

        return employee != null && employee.Id == actualEmployeeId;
    }

    [HttpGet("employee/{id}/pdf")]
    public async Task<IActionResult> GenerateEmployeeReport(Guid id, [FromQuery] Guid? evaluationPeriodId)
    {
        var employee = await employeeRepository.GetById(id)
            ?? await context.Employees.FirstOrDefaultAsync(e => e.IdentityUserId == id.ToString());

        if (employee == null) return NotFound();

        if (!await CanAccessEmployee(employee.Id)) return Forbid();

        var query = context.Achievements.Include(a => a.EvaluationPeriod).Where(a => a.EmployeeId == employee.Id);
        if (evaluationPeriodId.HasValue) query = query.Where(a => a.EvaluationPeriodId == evaluationPeriodId.Value);

        var achievements = await query.ToListAsync();
        var elements = await context.AchievementElements.ToListAsync();
        var evaluationPeriod = evaluationPeriodId.HasValue ? await context.EvaluationPeriods.FindAsync(evaluationPeriodId.Value) : null;

        var pdf = documentGeneratorService.GenerateReport(employee, achievements, elements, evaluationPeriod);
        return File(pdf, "application/pdf", $"report_{employee.LastName}.pdf");
    }

    [HttpGet("employee/{id}/excel")]
    public async Task<IActionResult> GenerateEmployeeExcelReport(Guid id, [FromQuery] Guid? evaluationPeriodId)
    {
        var employee = await employeeRepository.GetById(id)
            ?? await context.Employees.FirstOrDefaultAsync(e => e.IdentityUserId == id.ToString());

        if (employee == null) return NotFound();

        if (!await CanAccessEmployee(employee.Id)) return Forbid();

        var query = context.Achievements.Where(a => a.EmployeeId == employee.Id).AsQueryable();
        if (evaluationPeriodId.HasValue) query = query.Where(a => a.EvaluationPeriodId == evaluationPeriodId.Value);

        var achievements = await query.ToListAsync();
        var elements = await context.AchievementElements.ToListAsync();

        var excelBytes = documentGeneratorService.GenerateExcelReport(employee, achievements, elements);
        return File(excelBytes, "text/csv; charset=utf-8", "raport.csv");
    }

    [HttpGet("summary")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GenerateSummaryReport([FromQuery] Guid? evaluationPeriodId)
    {
        var query = context.Achievements.Include(a => a.Employee).Include(a => a.EvaluationPeriod).AsQueryable();
        if (evaluationPeriodId.HasValue && evaluationPeriodId.Value != Guid.Empty)
            query = query.Where(a => a.EvaluationPeriodId == evaluationPeriodId.Value);

        var achievements = await query.ToListAsync();
        var pdf = documentGeneratorService.GenerateSummaryReport(achievements);
        return File(pdf, "application/pdf", "summary_report.pdf");
    }

    [HttpGet("summary/excel")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GenerateSummaryExcelReport([FromQuery] Guid? evaluationPeriodId)
    {
        var query = context.Achievements.Include(a => a.Employee).Include(a => a.EvaluationPeriod).AsQueryable();
        if (evaluationPeriodId.HasValue && evaluationPeriodId.Value != Guid.Empty)
            query = query.Where(a => a.EvaluationPeriodId == evaluationPeriodId.Value);

        var achievements = await query.ToListAsync();
        var excelBytes = documentGeneratorService.GenerateExcelSummaryReport(achievements);
        return File(excelBytes, "text/csv; charset=utf-8", "raport_zbiorczy.csv");
    }
}