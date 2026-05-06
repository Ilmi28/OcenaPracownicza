using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;

namespace OcenaPracownicza.API.Repositories
{
    public class EvaluationPeriodRepository : IEvaluationPeriodRepository
    {
        private readonly ApplicationDbContext _context;

        public EvaluationPeriodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EvaluationPeriod> AddAsync(EvaluationPeriod period)
        {
            _context.EvaluationPeriods.Add(period);
            await _context.SaveChangesAsync();
            return period;
        }

        public async Task<IEnumerable<EvaluationPeriod>> GetAllAsync()
        {
            return await _context.EvaluationPeriods
                .Include(p => p.Achievements)
                .ToListAsync();
        }

        public async Task<EvaluationPeriod?> GetByDateAsync(DateTime date)
        {
            return await _context.EvaluationPeriods
                .FirstOrDefaultAsync(p => date >= p.StartDate && date <= p.EndDate);
        }

        public async Task<EvaluationPeriod?> GetByIdAsync(Guid id)
        {
            return await _context.EvaluationPeriods.FindAsync(id);
        }

        public async Task UpdateAsync(EvaluationPeriod period)
        {
            _context.EvaluationPeriods.Update(period);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var period = await _context.EvaluationPeriods.FindAsync(id);
            if (period != null)
            {
                _context.EvaluationPeriods.Remove(period);
                await _context.SaveChangesAsync();
            }
        }
    }
}