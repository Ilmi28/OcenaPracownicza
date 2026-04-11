using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IEvaluationProgressService
{
    Task<BaseResponse<EmployeeProgressView>> GetMyProgressAsync();
}