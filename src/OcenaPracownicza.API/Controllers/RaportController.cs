using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/reports")]
public class ReportsController(
    IEmployeeRepository employeeRepository,
    ApplicationDbContext context,
    IDocumentGeneratorService documentGeneratorService) : ControllerBase
{
    private async Task<List<AchievementElement>> GetElementsAsync()
        => await context.AchievementElements.ToListAsync();

    [HttpGet("employee/{id}/pdf")]
    public async Task<IActionResult> GenerateEmployeeReport(Guid id, [FromQuery] Guid? evaluationPeriodId)
    {
        var employee = await employeeRepository.GetById(id);
        if (employee == null) return NotFound();

        var query = context.Achievements
            .Include(a => a.EvaluationPeriod)
            .Where(a => a.EmployeeId == id);

        if (evaluationPeriodId.HasValue)
            query = query.Where(a => a.EvaluationPeriodId == evaluationPeriodId.Value);

        var achievements = await query.ToListAsync();
        var elements = await GetElementsAsync();   
        var evaluationPeriod = evaluationPeriodId.HasValue ? await context.EvaluationPeriods.FindAsync(evaluationPeriodId.Value) : null;

        var pdf = documentGeneratorService.GenerateReport(employee, achievements, elements, evaluationPeriod);

        var periodSuffix = evaluationPeriod != null ? $"_{evaluationPeriod.Name}" : "";
        return File(pdf, "application/pdf", $"report_{employee.LastName}{periodSuffix}.pdf");
    }

    [HttpGet("employee/{id}/excel")]
    public async Task<IActionResult> GenerateEmployeeExcelReport(Guid id, [FromQuery] Guid? evaluationPeriodId)
    {
        var employee = await employeeRepository.GetById(id);
        if (employee == null) return NotFound();

        EvaluationPeriod? evaluationPeriod = evaluationPeriodId.HasValue
            ? await context.EvaluationPeriods.FindAsync(evaluationPeriodId.Value)
            : null;

        var query = context.Achievements
            .Include(a => a.EvaluationPeriod)
            .Where(a => a.EmployeeId == id);

        if (evaluationPeriodId.HasValue)
            query = query.Where(a => a.EvaluationPeriodId == evaluationPeriodId.Value);

        var achievements = await query.ToListAsync();
        var elements = await GetElementsAsync();   

        var excelBytes = documentGeneratorService.GenerateExcelReport(employee, achievements, elements, evaluationPeriod);

        var periodSuffix = evaluationPeriod != null ? $"_{evaluationPeriod.Name}" : "";
        return File(excelBytes, "text/csv; charset=utf-8", $"raport_{employee.LastName}{periodSuffix}.csv");
    }

    [HttpGet("summary")]
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
    public async Task<IActionResult> GenerateSummaryExcelReport([FromQuery] Guid? evaluationPeriodId)
    {
        var query = context.Achievements.Include(a => a.Employee).Include(a => a.EvaluationPeriod).AsQueryable();
        if (evaluationPeriodId.HasValue && evaluationPeriodId.Value != Guid.Empty)
            query = query.Where(a => a.EvaluationPeriodId == evaluationPeriodId.Value);

        var achievements = await query.ToListAsync();
        var excelBytes = documentGeneratorService.GenerateExcelSummaryReport(achievements);
        return File(excelBytes, "text/csv; charset=utf-8", "raport_zbiorczy_pracownikow.csv");
    }
}