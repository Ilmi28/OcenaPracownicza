using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;

namespace OcenaPracownicza.API.Controllers;

[ApiController]
[Authorize(Roles = "Manager,Admin")]
[Route("api/evaluation/stage2")]
public class Stage2ReviewController(IStage2ReviewService stage2ReviewService) : ControllerBase
{
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var response = await stage2ReviewService.GetPendingAsync();
        return Ok(response);
    }

    [HttpGet("archived")]
    public async Task<IActionResult> GetArchived()
    {
        var response = await stage2ReviewService.GetArchivedAsync();
        return Ok(response);
    }

    [HttpGet("{employeeId:guid}")]
    public async Task<IActionResult> GetDetails(Guid employeeId)
    {
        var response = await stage2ReviewService.GetDetailsAsync(employeeId);
        return Ok(response);
    }

    [HttpPost("{employeeId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid employeeId, [FromBody] Stage2DecisionRequest request)
    {
        var response = await stage2ReviewService.ApproveAsync(employeeId, request.Comment);
        return Ok(response);
    }

    [HttpPost("{employeeId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid employeeId, [FromBody] Stage2DecisionRequest request)
    {
        var response = await stage2ReviewService.RejectAsync(employeeId, request.Comment);
        return Ok(response);
    }

    [HttpPost("{employeeId:guid}/close")]
    public async Task<IActionResult> Close(Guid employeeId)
    {
        var response = await stage2ReviewService.CloseAsync(employeeId);
        return Ok(response);
    }

    [HttpPost("{employeeId:guid}/archive")]
    public async Task<IActionResult> Archive(Guid employeeId)
    {
        var response = await stage2ReviewService.ArchiveAsync(employeeId);
        return Ok(response);
    }
}
