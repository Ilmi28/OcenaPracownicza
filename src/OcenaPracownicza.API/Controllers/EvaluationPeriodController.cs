using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;

namespace OcenaPracownicza.API.Controllers
{
    [ApiController]
    [Route("api/evaluation-periods")]
    public class EvaluationPeriodsController : ControllerBase
    {
        private readonly IEvaluationPeriodService _service;

        public EvaluationPeriodsController(IEvaluationPeriodService service)
        {
            _service = service;
        }

        [HttpGet("by-date")]
        [Authorize]
        public async Task<ActionResult> GetByDate([FromQuery] DateTime date)
        {
            var period = await _service.GetPeriodByDateAsync(date);

            if (period == null)
            {
                return NotFound(new { message = "Nie znaleziono okresu ocen dla podanej daty." });
            }

            return Ok(new
            {
                id = period.Id,
                name = period.Name
            });
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var periods = await _service.GetAllPeriodsAsync();

            var result = periods.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                startDate = p.StartDate,
                endDate = p.EndDate,
                regulationVersion = p.RegulationVersion
            });

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] EvaluationPeriodRequest request)
        {
            var period = new EvaluationPeriod
            {
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RegulationVersion = request.RegulationVersion
            };

            try
            {
                var result = await _service.CreatePeriodAsync(period);

                return Ok(new
                {
                    id = result.Id,
                    name = result.Name,
                    message = "Okres został utworzony pomyślnie."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EvaluationPeriodRequest request)
        {
            try
            {
                await _service.UpdatePeriodAsync(id, request);
                return Ok(new { message = "Zaktualizowano pomyślnie." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Nie znaleziono okresu o podanym ID." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeletePeriodAsync(id);

            if (!success)
            {
                return BadRequest(new { message = "Nie można usunąć okresu, który posiada przypisane osiągnięcia." });
            }

            return Ok(new { message = "Okres został usunięty." });
        }
    }
}