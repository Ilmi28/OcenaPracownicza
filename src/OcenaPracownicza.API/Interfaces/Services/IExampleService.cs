using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IExampleService
{
    Task<ExampleResponse> ExampleOperation();
}
