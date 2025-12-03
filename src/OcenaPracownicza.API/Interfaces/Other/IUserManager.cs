using Microsoft.AspNetCore.Identity;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Interfaces.Other
{
    public interface IUserManager
    {
        Task<bool> CreateAsync(IdentityUser user, string password);
        Task<bool> CreateWithoutPassword(IdentityUser user);
        Task<IdentityUser?> FindByIdAsync(string userId);
        Task<IdentityUser?> FindByEmailAsync(string email);
        Task<IdentityUser?> FindByNameAsync(string userName);
        Task<bool> CheckPasswordAsync(string userId, string password);
        Task<bool> DeleteAsync(string userId);
        Task<bool> UpdateAsync(IdentityUser user);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> AddToRoleAsync(string userId, string roleName);
        Task<bool> RemoveFromRoleAsync(string userId, string roleName);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName);
        Task<bool> IsUserAccountOwner(string userId);
        bool IsCurrentUserAdmin();
        bool IsCurrentUserManager();
        bool IsCurrentUserEmployee();
    }
}
