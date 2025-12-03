using Microsoft.AspNetCore.Identity;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;

namespace OcenaPracownicza.API.Services;

public class AdminService : IAdminService
{
    private readonly IUserManager _userManager;
    private const string AdminRoleName = "Admin";

    public AdminService(IUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<AdminResponse> GetById(string id)
    {
        if (!_userManager.IsCurrentUserAdmin())
             throw new ForbiddenException();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new NotFoundException();

        return MapToResponse(user);
    }

    public async Task<List<AdminResponse>> GetAll()
    {
        if (!_userManager.IsCurrentUserAdmin())
            throw new ForbiddenException();
        
        var admins = await _userManager.GetUsersInRoleAsync(AdminRoleName);

        return admins.Select(MapToResponse).ToList();
    }

    public async Task<AdminResponse> Add(CreateAdminRequest request)
    {
        if (!_userManager.IsCurrentUserAdmin())
            throw new ForbiddenException();

        var identityUser = new IdentityUser
        {
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(identityUser, request.Password);

        if (result)
        {
            await _userManager.AddToRoleAsync(identityUser.Id, AdminRoleName);
            return MapToResponse(identityUser);
        }
        
        throw new Exception("Wystąpił błąd podczas tworzenia administratora.");
    }

    public async Task<AdminResponse> Update(string id, UpdateAdminRequest request)
    {
        var isAccountOwner = await _userManager.IsUserAccountOwner(id);
        
        if (!_userManager.IsCurrentUserAdmin() && !isAccountOwner)
            throw new ForbiddenException();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new NotFoundException();

        user.UserName = request.UserName;
        user.Email = request.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result)
             throw new Exception("Wystąpił błąd podczas aktualizacji administratora.");

        return MapToResponse(user);
    }

    public async Task Delete(string id)
    {
        if (!_userManager.IsCurrentUserAdmin())
            throw new ForbiddenException();
        
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            throw new NotFoundException();
        
        var isAccountOwner = await _userManager.IsUserAccountOwner(id);
        if (isAccountOwner)
            throw new Exception("Nie możesz usunąć własnego konta.");
        
        var result = await _userManager.DeleteAsync(id);
        
        if (!result)
            throw new Exception("Nie udało się usunąć użytkownika.");
    }

    private static AdminResponse MapToResponse(IdentityUser user)
    {
        return new AdminResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }
}