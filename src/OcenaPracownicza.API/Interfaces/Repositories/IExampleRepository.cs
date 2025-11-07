using OcenaPracownicza.API.Models;

namespace OcenaPracownicza.API.Interfaces.Repositories;

public interface IExampleRepository
{
    Task<ExampleModel?> DatabaseOperation();
}
