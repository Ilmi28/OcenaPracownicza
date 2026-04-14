using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Interfaces.Repositories
{
    public interface IEvaluationPeriodService
    {
        Task<EvaluationPeriod> CreatePeriodAsync(EvaluationPeriod period);
        Task<IEnumerable<EvaluationPeriod>> GetAllPeriodsAsync();
    }
}
