using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;

namespace OcenaPracownicza.API.Controllers
{
    [ApiController]
    [Route("api/evaluation-periods")]
    public class EvaluationPeriodsController : ControllerBase
    {
        private readonly IEvaluationPeriodService _service;
        public EvaluationPeriodsController(IEvaluationPeriodService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EvaluationPeriod period)
        {
            var result = await _service.CreatePeriodAsync(period);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllPeriodsAsync();
            return Ok(result);
        }
    }
}
