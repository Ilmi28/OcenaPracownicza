using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;

namespace OcenaPracownicza.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/reports")]
public class ReportsController(
    IEmployeeRepository employeeRepository,
    ApplicationDbContext context,
    IDocumentGeneratorService documentGeneratorService) : ControllerBase
{
    [HttpGet("employee/{id}/pdf")]
    public async Task<IActionResult> GenerateEmployeeReport(Guid id)
    {
        var employee = await employeeRepository.GetById(id);
        if (employee == null)
            return NotFound();

        var achievements = await context.Achievements
            .Include(a => a.EvaluationPeriod)
            .Where(a => a.EmployeeId == id)
            .ToListAsync();

        var pdf = documentGeneratorService.GenerateReport(employee, achievements);

        return File(pdf, "application/pdf", $"report_{employee.LastName}.pdf");
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GenerateSummaryReport([FromQuery] Guid? evaluationPeriodId)
    {
        var query = context.Achievements
            .Include(a => a.Employee)
            .Include(a => a.EvaluationPeriod)
            .AsQueryable();

        if (evaluationPeriodId.HasValue && evaluationPeriodId.Value != Guid.Empty)
        {
            query = query.Where(a => a.EvaluationPeriodId == evaluationPeriodId.Value);
        }

        var achievements = await query.ToListAsync();

        var pdf = documentGeneratorService.GenerateSummaryReport(achievements);

        return File(pdf, "application/pdf", "summary_report.pdf");
    }
}