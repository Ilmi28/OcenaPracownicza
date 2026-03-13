using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponse> GetCurrentUser();
        Task<UserResponse> GetById(string userId);
        Task<UserListResponse> GetAll();
        Task<BaseResponse> ChangePassword(ChangePasswordRequest request);
    }
}
