using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;

[ApiController]
[Authorize]
[Route("api/grades")]
public class GradeController(IGradeService gradeService) : ControllerBase
{
    [HttpGet("employee/{employeeId:guid}")]
    [Authorize(Roles = "Employee,Manager,Admin")]
    public async Task<IActionResult> GetForEmployee(Guid employeeId)
        => Ok(await gradeService.GetEmployeeGradesAsync(employeeId));

    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateGradeRequest request)
        => Ok(await gradeService.CreateGradeAsync(request));

    [HttpPut("{gradeId:guid}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Update(Guid gradeId, [FromBody] UpdateGradeRequest request)
        => Ok(await gradeService.UpdateGradeAsync(gradeId, request));

    [HttpDelete("{gradeId:guid}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Delete(Guid gradeId)
        => Ok(await gradeService.DeleteGradeAsync(gradeId));

    [HttpGet("period/{periodId:guid}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetByPeriod(Guid periodId)
    => Ok(await gradeService.GetGradesByPeriodAsync(periodId));
}