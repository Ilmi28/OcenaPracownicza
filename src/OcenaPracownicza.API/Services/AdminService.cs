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

    public AdminService(IUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<AdminResponse>> GetAll()
    {
        if (!_userManager.IsCurrentUserAdmin())
        {
            throw new ForbiddenException("Brak uprawnień do przeglądania administratorów.");
        }

        var users = await _userManager.GetUsersInRoleAsync("Admin");

        return users.Select(u => new AdminResponse
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email
        }).ToList();
    }

    public async Task<AdminResponse> GetById(string id)
    {
        if (!_userManager.IsCurrentUserAdmin())
        {
            throw new ForbiddenException("Brak uprawnień.");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"Nie znaleziono użytkownika o ID: {id}");
        }

        return new AdminResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task<AdminResponse> Add(CreateAdminRequest request)
    {
        if (!_userManager.IsCurrentUserAdmin())
        {
            throw new ForbiddenException("Tylko administrator może dodawać nowych administratorów.");
        }

        var user = new IdentityUser
        {
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = true
        };
        
        var created = await _userManager.CreateAsync(user, request.Password);
        if (!created)
        {
            throw new BadRequestException("Nie udało się utworzyć użytkownika. Sprawdź poprawność hasła.");
        }
        
        var roleAdded = await _userManager.AddToRoleAsync(user.Id, "Admin");
        if (!roleAdded)
        {
            await _userManager.DeleteAsync(user.Id);
            throw new BadRequestException("Nie udało się przypisać roli Administratora.");
        }

        return new AdminResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task<AdminResponse> Update(string id, UpdateAdminRequest request)
    {
        var isGlobalAdmin = _userManager.IsCurrentUserAdmin();
        var isOwner = await _userManager.IsUserAccountOwner(id);

        if (!isGlobalAdmin && !isOwner)
        {
            throw new ForbiddenException("Brak uprawnień do edycji tego konta.");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"Nie znaleziono użytkownika o ID: {id}");
        }

        user.UserName = request.UserName;
        user.Email = request.Email;
        
        var updated = await _userManager.UpdateAsync(user);
        if (!updated)
        {
            throw new BadRequestException("Nie udało się zaktualizować użytkownika.");
        }

        return new AdminResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task<AdminResponse> Delete(string id)
    {
        if (!_userManager.IsCurrentUserAdmin())
        {
            throw new ForbiddenException("Tylko administrator może usuwać użytkowników.");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException($"Nie znaleziono użytkownika o ID: {id}");
        }
        
        var deleted = await _userManager.DeleteAsync(user.Id);

        if (!deleted)
        {
            throw new BadRequestException("Nie udało się usunąć użytkownika.");
        }

        return new AdminResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }
}