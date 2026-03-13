using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Repositories;

// Repozytorium sluzy do pobierania danych z bazy danych. Powinno zwracac dane albo null, nie wyrzucac wyjatkow.
// Kazde repozytorium powinno miec swoj interfejs w folderze Interfaces/Repositories.
public class ExampleRepository : BaseRepository<ExampleEntity>, IExampleRepository
{
    public ExampleRepository(ApplicationDbContext context) : base(context)
    {
    }
}
