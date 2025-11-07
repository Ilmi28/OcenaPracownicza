using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Models;

namespace OcenaPracownicza.API.Repositories;

// Repozytorium sluzy do pobierania danych z bazy danych. Powinno zwracac dane albo null, nie wyrzucac wyjatkow.
// Kazde repozytorium powinno miec swoj interfejs w folderze Interfaces/Repositories.
public class ExampleRepository : IExampleRepository
{
    public async Task<ExampleModel?> DatabaseOperation()
    {
        // Tutaj można dodać logikę operacji bazodanowej, np. pobieranie danych z bazy danych.
        return new ExampleModel
        {
            Id = 1,
            Name = "Przykładowa nazwa",
            Description = "To jest przykładowy opis z operacji bazodanowej."
        };
    }
}
