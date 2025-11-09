using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;

namespace OcenaPracownicza.Controllers;


// W kontrolerze dajemy odwolanie do serwisu poprzez wstrzykiwanie zależności w konstruktorze
// Jak zwracamy jakies dane to zwracamy Ok(data) z danymi jak nie to zwracamy NoContent() analogicznie z innymi kodami statusu HTTP
// Nie implementujemy logiki biznesowej w kontrolerze, tylko w serwisach
// Wyrzucone wyjatki z serwisów sa obsługiwane globalnie w middleware w Program.cs wiec tutaj nic nie trzeba robic jedynie zwrocic odpowiedni rezultat
[ApiController]
[Route("example")]
public class ExampleController(IExampleService exampleService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var response = await exampleService.GetById(id);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await exampleService.GetAll();
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post(ExampleRequest request)
    {
        var response = await exampleService.Add(request);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, ExampleRequest request)
    {
        var response = await exampleService.Update(id, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await exampleService.Delete(id);
        return Ok(response);
    }
}
