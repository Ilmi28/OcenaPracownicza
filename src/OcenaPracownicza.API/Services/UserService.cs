using Microsoft.AspNetCore.Identity;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> CreateAsync(UserRequest request, string password)
        {
            var user = new IdentityUser
            {
                UserName = request.UserName,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, password);

            return result.Succeeded;
        }

        public async Task<UserResponse?> FindByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? null : MapToResponse(user);
        }

        public async Task<UserResponse?> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null ? null : MapToResponse(user);
        }

        public async Task<UserResponse?> FindByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user == null ? null : MapToResponse(user);
        }

        public async Task<bool> CheckPasswordAsync(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> UpdateAsync(string userId, UserRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.Email = request.Email;
            user.UserName = request.UserName;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> AddToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<bool> RemoveFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();

            return await _userManager.GetRolesAsync(user);
        }

        private UserResponse MapToResponse(IdentityUser user)
        {
            return new UserResponse
            {
                Data = new UserView
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty
                }
            };
        }
    }
}