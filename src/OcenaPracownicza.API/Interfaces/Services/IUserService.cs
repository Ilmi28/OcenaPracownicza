using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> CreateAsync(UserRequest user, string password);
        Task<UserResponse?> FindByIdAsync(string userId);
        Task<UserResponse?> FindByEmailAsync(string email);
        Task<UserResponse?> FindByNameAsync(string userName);
        Task<bool> CheckPasswordAsync(string userId, string password);
        Task<bool> DeleteAsync(string userId);
        Task<bool> UpdateAsync(string userId, UserRequest user);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> AddToRoleAsync(string userId, string roleName);
        Task<bool> RemoveFromRoleAsync(string userId, string roleName);
        Task<IList<string>> GetUserRolesAsync(string userId);
    }
}
