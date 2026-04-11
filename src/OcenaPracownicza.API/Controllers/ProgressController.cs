using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Interfaces.Services;

namespace OcenaPracownicza.API.Controllers;

[ApiController]
[Authorize]
[Route("api/progress")]
public class ProgressController : ControllerBase
{
    private readonly IEvaluationProgressService _progressService;

    public ProgressController(IEvaluationProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProgress()
    {
        var response = await _progressService.GetMyProgressAsync();
        return Ok(response);
    }
}