using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;


namespace OcenaPracownicza.Controllers;

[ApiController]
[Route("employee")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
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
    public async Task<IActionResult> Post(EmployeeRequest request)
    {
        var response = await employeeService.Add(request);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, EmployeeRequest request)
    {
        var response = await employeeService.Update(id, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await employeeService.Delete(id);
        return Ok(response);
    }
}
