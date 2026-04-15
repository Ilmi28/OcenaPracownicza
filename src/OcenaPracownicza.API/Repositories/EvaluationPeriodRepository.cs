using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;

namespace OcenaPracownicza.API.Repositories
{
    public class EvaluationPeriodRepository : IEvaluationPeriodRepository
    {
        private readonly ApplicationDbContext _context;
        public EvaluationPeriodRepository(ApplicationDbContext context) => _context = context;

        public async Task<EvaluationPeriod> AddAsync(EvaluationPeriod period)
        {
            _context.EvaluationPeriods.Add(period);
            await _context.SaveChangesAsync();
            return period;
        }

        public async Task<IEnumerable<EvaluationPeriod>> GetAllAsync() =>
            await _context.EvaluationPeriods.Include(p => p.Criteria).ToListAsync();
    }
}
