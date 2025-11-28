using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;

namespace OcenaPracownicza.API.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }
}
