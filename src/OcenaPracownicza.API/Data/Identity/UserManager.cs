using Microsoft.AspNetCore.Identity;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OcenaPracownicza.API.Data.Identity
{
    public class UserManager : IUserManager
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserManager(UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IList<IdentityUser>> GetAllUsersAsync()
        {
            return await Task.FromResult(_userManager.Users.ToList());
        }

        public async Task<bool> CreateAsync(IdentityUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            return result.Succeeded;
        }

        public async Task<IdentityUser?> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityUser?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityUser?> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public string? GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue("sub");
            return userId;
        }

        public async Task<bool> CheckPasswordAsync(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> UpdateAsync(IdentityUser user)
        {
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

        public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }
        
        public bool IsUserAccountOwner(string userId)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || user.FindFirstValue(ClaimTypes.NameIdentifier) != userId) return false;

            return true;
        }

        public bool IsCurrentUserAdmin()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user != null && user.IsInRole("Admin");
        }

        public bool IsCurrentUserManager()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user != null && user.IsInRole("Manager");
        }

        public bool IsCurrentUserEmployee()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user != null && user.IsInRole("Employee");
        }

        public async Task<bool> CreateWithoutPassword(IdentityUser user)
        {
            var result = await _userManager.CreateAsync(user);

            return result.Succeeded;
        }
    }
}
