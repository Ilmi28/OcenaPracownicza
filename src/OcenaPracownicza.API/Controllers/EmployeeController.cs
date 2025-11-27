using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        this.employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IEnumerable<EmployeeResponse>> GetAll()
    {
        return await employeeService.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<EmployeeResponse> GetById(int id)
    {
        return await employeeService.GetById(id);
    }

    [HttpPost]
    public async Task<EmployeeResponse> Create(CreateEmployeeRequest request)
    {
        return await employeeService.Create(request);
    }

    [HttpPut("{id}")]
    public async Task<EmployeeResponse> Update(int id, UpdateEmployeeRequest request)
    {
        return await employeeService.Update(id, request);
    }

    [HttpDelete("{id}")]
    public async Task<DeleteEmployeeResponse> Delete(int id)
    {
        return await employeeService.Delete(id);
    }
}
