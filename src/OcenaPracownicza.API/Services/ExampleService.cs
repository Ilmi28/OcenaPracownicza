using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
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

    public async Task<ExampleResponse> ExampleOperation()
    {
        // Przyklad wyjatku NotFoundException zdefiniowanego w projekcie
        var data = await exampleRepository.DatabaseOperation() 
            ?? throw new NotFoundException("Nie znaleziono takiego obiektu.");


        var response = new ExampleResponse
        {
            Id = data.Id,
            Name = data.Name
        };

        return response;
    }
}
