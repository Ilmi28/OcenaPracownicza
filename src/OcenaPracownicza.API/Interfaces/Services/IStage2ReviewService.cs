using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IStage2ReviewService
{
    Task<BaseResponse<List<Stage2ReviewItemView>>> GetPendingAsync();
    Task<BaseResponse<Stage2ReviewDetailsView>> GetDetailsAsync(Guid employeeId);
    Task<BaseResponse<Stage2ReviewDetailsView>> ApproveAsync(Guid employeeId, string? comment);
    Task<BaseResponse<Stage2ReviewDetailsView>> RejectAsync(Guid employeeId, string? comment);
}
