using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IExampleService
{
    Task<ExampleResponse> GetById(Guid id);
    Task<ExampleListResponse> GetAll();
    Task<ExampleResponse> Add(ExampleRequest request);
    Task<ExampleResponse> Update(Guid id, ExampleRequest request);
    Task<ExampleResponse> Delete(Guid id);
}
