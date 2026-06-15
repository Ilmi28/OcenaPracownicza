using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IGradeService
    {
        Task<BaseResponse<List<GradeView>>> GetEmployeeGradesAsync(Guid employeeId);
        Task<BaseResponse<GradeView>> CreateGradeAsync(CreateGradeRequest request);
        Task<BaseResponse<GradeView>> UpdateGradeAsync(Guid gradeId, UpdateGradeRequest request);
        Task<BaseResponse<string>> DeleteGradeAsync(Guid gradeId);
        Task<BaseResponse<List<GradeView>>> GetGradesByPeriodAsync(Guid periodId);
    }
}
