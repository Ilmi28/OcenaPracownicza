using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IStage2ReviewService
{
    Task<BaseResponse<List<Stage2ReviewItemView>>> GetPendingAsync();
    Task<BaseResponse<List<Stage2ReviewItemView>>> GetApprovedAsync();
    Task<BaseResponse<List<Stage2ReviewItemView>>> GetArchivedAsync();
    Task<BaseResponse<Stage2ReviewDetailsView>> GetDetailsAsync(Guid achievementId);
    Task<BaseResponse<Stage2ReviewDetailsView>> ApproveAsync(Guid achievementId, string? comment);
    Task<BaseResponse<Stage2ReviewDetailsView>> RejectAsync(Guid achievementId, string? comment);
    Task<BaseResponse<Stage2ReviewDetailsView>> CloseAsync(Guid achievementId);
    Task<BaseResponse<Stage2ReviewDetailsView>> ArchiveAsync(Guid achievementId);
}
