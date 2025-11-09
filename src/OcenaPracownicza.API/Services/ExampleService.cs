using OcenaPracownicza.API.Dtos;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Services;

// Service sluzy do logiki w odroznieniu od repozytorium ktore sluzy do dostepu do danych
// Kazdy service powinien miec swoj interfejs w folderze Interfaces/Services
// W przypadku niepowodzenia nalezy rzucic odpowiedni wyjatek 
// Najlepiej by bylo to wyjatek zdefiniowany w projekcie OcenaPracownicza.API/Exceptions albo UnauthorizedAccessException, ArgumentNullException, InvalidOperationException
// Wyjatki wyrzucamy tylko w service nie w repozytorium!!!
// W przypadku sukcesu i gdy potrzebujemy zwrocic jakies dane zwracamy odpowiedz DTO z folderu Responses
// Nie zwracamy encji bezposrednio z bazy danych
public class ExampleService(IExampleRepository exampleRepository) : IExampleService
{

    public async Task<ExampleResponse> GetById(int id)
    {
        // Przyklad wyjatku NotFoundException zdefiniowanego w projekcie
        var data = await exampleRepository.GetById(id)
            ?? throw new NotFoundException("Nie znaleziono takiego obiektu.");

        var dto = new ExampleDto
        {
            Name = data.Name,
            Description = data.Description,
            SomeDetail = data.SomeDetail
        };

        return new ExampleResponse
        {
            Data = dto
        };
    }

    public async Task<ExampleListResponse> GetAll()
    {
        var data = await exampleRepository.GetAll();
        var dtos = data.Select(d => new ExampleDto
        {
            Name = d.Name,
            Description = d.Description,
            SomeDetail = d.SomeDetail
        }).ToList();

        return new ExampleListResponse
        {
            Data = dtos
        };
    }

    public async Task<ExampleResponse> Add(ExampleRequest request)
    {
        // Przyklad wyjatku ArgumentNullException
        _ = request ?? throw new ArgumentNullException(nameof(request));

        var entity = new ExampleEntity
        {
            Name = request.Name,
            Description = request.Description,
            SomeDetail = request.SomeDetail
        };
        
        var created = await exampleRepository.Create(entity);

        var dto = new ExampleDto
        {
            Name = created.Name,
            Description = created.Description,
            SomeDetail = created.SomeDetail
        };

        return new ExampleResponse
        {
            Data = dto
        };
    }

    public async Task<ExampleResponse> Update(int id, ExampleRequest request)
    {
        var existing = await exampleRepository.GetById(id)
            ?? throw new NotFoundException("Nie znaleziono takiego obiektu.");
        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.SomeDetail = request.SomeDetail;
        var updated = await exampleRepository.Update(existing);
        var dto = new ExampleDto
        {
            Name = updated.Name,
            Description = updated.Description,
            SomeDetail = updated.SomeDetail
        };
        return new ExampleResponse
        {
            Data = dto
        };
    }

    public async Task<ExampleResponse> Delete(int id)
    {
        var exists = await exampleRepository.Exists(id);
        if (!exists)
        {
            throw new NotFoundException("Nie znaleziono takiego obiektu.");
        }
        await exampleRepository.Delete(id);

        return new ExampleResponse();
    }
}
