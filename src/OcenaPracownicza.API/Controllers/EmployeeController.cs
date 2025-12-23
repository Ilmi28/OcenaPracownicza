using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;


namespace OcenaPracownicza.Controllers;

[ApiController]
[Authorize]
[Route("api/employee")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await employeeService.GetById(id);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await employeeService.GetAll();
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateEmployeeRequest request)
    {
        var response = await employeeService.Add(request);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UpdateEmployeeRequest request)
    {
        var response = await employeeService.Update(id, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await employeeService.Delete(id);
        return Ok(response);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentEmployee()
    {
        var response = await employeeService.GetCurrent();
        return Ok(response);
    }
}
