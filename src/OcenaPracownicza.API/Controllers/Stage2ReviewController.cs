using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;

namespace OcenaPracownicza.API.Controllers;

[ApiController]
[Authorize]
[Route("api/evaluation/stage2")]
public class Stage2ReviewController(IStage2ReviewService stage2ReviewService) : ControllerBase
{
    [HttpGet("history")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetHistory()
    {
        var response = await stage2ReviewService.GetHistoryAsync();
        return Ok(response);
    }
    
    [HttpGet("history/me")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetMyHistory()
    {
        var response = await stage2ReviewService.GetMyHistoryAsync();
        return Ok(response);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetPending()
    {
        var response = await stage2ReviewService.GetPendingAsync();
        return Ok(response);
    }

    [HttpGet("approved")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetApproved()
    {
        var response = await stage2ReviewService.GetApprovedAsync();
        return Ok(response);
    }

    [HttpGet("archived")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetArchived()
    {
        var response = await stage2ReviewService.GetArchivedAsync();
        return Ok(response);
    }

    [HttpGet("{achievementId:guid}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetDetails(Guid achievementId)
    {
        var response = await stage2ReviewService.GetDetailsAsync(achievementId);
        return Ok(response);
    }
    
    [HttpGet("history/me/{achievementId:guid}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetMyDetails(Guid achievementId)
    {
        var response = await stage2ReviewService.GetMyDetailsAsync(achievementId);
        return Ok(response);
    }

    [HttpPost("{achievementId:guid}/approve")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Approve(Guid achievementId, [FromBody] Stage2DecisionRequest request)
    {
        var response = await stage2ReviewService.ApproveAsync(achievementId, request.Comment);
        return Ok(response);
    }

    [HttpPost("{achievementId:guid}/reject")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Reject(Guid achievementId, [FromBody] Stage2DecisionRequest request)
    {
        var response = await stage2ReviewService.RejectAsync(achievementId, request.Comment);
        return Ok(response);
    }

    [HttpPost("{achievementId:guid}/close")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Close(Guid achievementId)
    {
        var response = await stage2ReviewService.CloseAsync(achievementId);
        return Ok(response);
    }

    [HttpPost("{achievementId:guid}/archive")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Archive(Guid achievementId)
    {
        var response = await stage2ReviewService.ArchiveAsync(achievementId);
        return Ok(response);
    }
}
