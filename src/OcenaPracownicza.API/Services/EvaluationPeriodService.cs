using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Data;      

namespace OcenaPracownicza.API.Services
{
    public class EvaluationPeriodService : IEvaluationPeriodService
    {
        private readonly IEvaluationPeriodRepository _repo;
        private readonly ApplicationDbContext _context;     

        public EvaluationPeriodService(IEvaluationPeriodRepository repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<EvaluationPeriod> CreatePeriodAsync(EvaluationPeriod period)
        {
            if (period.EndDate <= period.StartDate)
            {
                throw new ArgumentException("Data zakończenia okresu musi być późniejsza niż data rozpoczęcia.");
            }

            period.IsClosed = false;
            return await _repo.AddAsync(period);
        }

        public async Task<IEnumerable<EvaluationPeriod>> GetAllPeriodsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<EvaluationPeriod?> GetPeriodByDateAsync(DateTime date)
        {
            return await _repo.GetByDateAsync(date);
        }

        public async Task UpdatePeriodAsync(Guid id, EvaluationPeriodRequest request)
        {
            var period = await _repo.GetByIdAsync(id);
            if (period == null) throw new KeyNotFoundException("Okres nie istnieje");

            if (request.EndDate <= request.StartDate)
            {
                throw new ArgumentException("Data zakończenia musi być późniejsza niż rozpoczęcia.");
            }

            period.Name = request.Name;
            period.StartDate = request.StartDate;
            period.EndDate = request.EndDate;

            await _repo.UpdateAsync(period);
        }

        public async Task<bool> DeletePeriodAsync(Guid id)
        {
            var hasAchievements = await _context.Achievements.AnyAsync(a => a.EvaluationPeriodId == id);

            if (hasAchievements)
                return false;

            await _repo.DeleteAsync(id);
            return true;
        }
    }
}