using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Interfaces.Repositories;

public interface IAchievementRepository : IBaseRepository<Achievement>
{
    Task<IEnumerable<Achievement>> GetByEmployeeIdAsync(Guid employeeId);
}