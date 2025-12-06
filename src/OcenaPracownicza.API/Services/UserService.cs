using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;
using System.Security.Claims;

namespace OcenaPracownicza.API.Services
{
    public class UserService(IUserManager userManager) : IUserService
    {
        public async Task<BaseResponse> ChangePassword(ChangePasswordRequest request)
        {

            var currentUserId = userManager.GetCurrentUserId()
                ?? throw new UnauthorizedAccessException();

            if (!userManager.IsCurrentUserAdmin() && !userManager.IsUserAccountOwner(currentUserId))
                throw new ForbiddenException();


            var result = await userManager.ChangePasswordAsync(currentUserId, request.CurrentPassword, request.NewPassword);

            if (result)
                return new BaseResponse();
            throw new ForbiddenException();
        }

        public async Task<BaseResponse> DeleteById(string userId)
        {
            if (!userManager.IsCurrentUserAdmin() && !userManager.IsUserAccountOwner(userId))
                throw new ForbiddenException();

            var result = await userManager.DeleteAsync(userId);

            if (result)
                return new BaseResponse();
            throw new Exception("An error occurred while deleting the user.");
        }

        public async Task<UserListResponse> GetAll()
        {
            if (!userManager.IsCurrentUserAdmin())
                throw new ForbiddenException();

            var users = await userManager.GetAllUsersAsync();

            return new UserListResponse
            {
                Data = users.Select(u => new UserView
                {
                    Id = u.Id,
                    UserName = u.UserName!,
                    Email = u.Email!,
                }).ToList()
            };
        }

        public async Task<UserResponse> GetById(string userId)
        {
            if (!userManager.IsCurrentUserAdmin() && !userManager.IsUserAccountOwner(userId))
                throw new ForbiddenException();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            return new UserResponse
            {
                Data = new UserView
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                }
            };
        }

        public async Task<UserResponse> GetCurrentUser()
        {
            var userId = userManager.GetCurrentUserId()
                ?? throw new UnauthorizedAccessException();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new NotFoundException("User not found.");

            return new UserResponse
            {
                Data = new UserView
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                }
            };
        }
    }
}
