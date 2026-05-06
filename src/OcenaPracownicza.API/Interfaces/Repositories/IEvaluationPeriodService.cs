using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Requests;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IEvaluationPeriodService
    {
        Task<IEnumerable<EvaluationPeriod>> GetAllPeriodsAsync();
        Task<EvaluationPeriod?> GetPeriodByDateAsync(DateTime date);
        Task<EvaluationPeriod> CreatePeriodAsync(EvaluationPeriod period);
        Task UpdatePeriodAsync(Guid id, EvaluationPeriodRequest request);
        Task<bool> DeletePeriodAsync(Guid id); 
    }
}