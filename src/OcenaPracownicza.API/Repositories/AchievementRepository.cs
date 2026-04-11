using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;

namespace OcenaPracownicza.API.Repositories;

public class AchievementRepository : BaseRepository<Achievement>, IAchievementRepository
{
    public AchievementRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Achievement>> GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _dbSet.Where(a => a.EmployeeId == employeeId).ToListAsync();
    }
}