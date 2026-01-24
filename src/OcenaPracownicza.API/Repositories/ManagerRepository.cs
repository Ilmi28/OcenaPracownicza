using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;


namespace OcenaPracownicza.API.Repositories
{
    public class ManagerRepository : BaseRepository<Manager>, IManagerRepository
    {
        public ManagerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Manager?> GetByUserId(string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.IdentityUserId == userId);
        }
    }
}