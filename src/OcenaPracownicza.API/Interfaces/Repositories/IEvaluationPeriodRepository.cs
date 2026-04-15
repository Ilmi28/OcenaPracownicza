using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Interfaces.Repositories
{
    public interface IEvaluationPeriodRepository
    {
        Task<EvaluationPeriod> AddAsync(EvaluationPeriod period);
        Task<IEnumerable<EvaluationPeriod>> GetAllAsync();
    }
}
