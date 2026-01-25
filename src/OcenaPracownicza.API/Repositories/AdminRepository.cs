using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;

namespace OcenaPracownicza.API.Repositories;

public class AdminRepository : BaseRepository<Admin>, IAdminRepository
{
    public AdminRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Admin?> GetByUserId(string userId)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.IdentityUserId == userId);
    }
}
