using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Interfaces.Repositories
{
    public interface IEvaluationPeriodRepository
    {
        Task<EvaluationPeriod> AddAsync(EvaluationPeriod period);
        Task<IEnumerable<EvaluationPeriod>> GetAllAsync();
        Task<EvaluationPeriod?> GetByDateAsync(DateTime date);
        Task<EvaluationPeriod?> GetByIdAsync(Guid id);
        Task UpdateAsync(EvaluationPeriod period);
        Task DeleteAsync(Guid id);
    }
}