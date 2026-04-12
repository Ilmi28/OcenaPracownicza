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

    [HttpGet("{achievementId:guid}")]
    public async Task<IActionResult> GetDetails(Guid achievementId)
    {
        var response = await stage2ReviewService.GetDetailsAsync(achievementId);
        return Ok(response);
    }

    [HttpPost("{achievementId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid achievementId, [FromBody] Stage2DecisionRequest request)
    {
        var response = await stage2ReviewService.ApproveAsync(achievementId, request.Comment);
        return Ok(response);
    }

    [HttpPost("{achievementId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid achievementId, [FromBody] Stage2DecisionRequest request)
    {
        var response = await stage2ReviewService.RejectAsync(achievementId, request.Comment);
        return Ok(response);
    }

    [HttpPost("{achievementId:guid}/close")]
    public async Task<IActionResult> Close(Guid achievementId)
    {
        var response = await stage2ReviewService.CloseAsync(achievementId);
        return Ok(response);
    }

    [HttpPost("{achievementId:guid}/archive")]
    public async Task<IActionResult> Archive(Guid achievementId)
    {
        var response = await stage2ReviewService.ArchiveAsync(achievementId);
        return Ok(response);
    }
}
