using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Repositories;

namespace OcenaPracownicza.API.Services
{
    public class EvaluationPeriodService : IEvaluationPeriodService
    {
        private readonly IEvaluationPeriodRepository _repo;
        public EvaluationPeriodService(IEvaluationPeriodRepository repo) => _repo = repo;

        public Task<EvaluationPeriod> CreatePeriodAsync(EvaluationPeriod period) => _repo.AddAsync(period);
        public Task<IEnumerable<EvaluationPeriod>> GetAllPeriodsAsync() => _repo.GetAllAsync();
    }
}
