using Microsoft.AspNetCore.Identity;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Services;

public class AdminService : IAdminService
{
    private readonly IUserManager _userManager;
    private readonly IAdminRepository _adminRepository;
    private readonly IUserService _userService;

    public AdminService(IUserManager userManager, IAdminRepository adminRepository, IUserService userService)
    {
        _userManager = userManager;
        _adminRepository = adminRepository;
        _userService = userService;
    }

    public async Task<AdminResponse> GetById(Guid id)
    {
        var entity = await _adminRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException();

        var user = await _userManager.FindByIdAsync(entity.IdentityUserId);

        var isAccountOwner = _userManager.IsUserAccountOwner(entity.IdentityUserId);

        if (_userManager.IsCurrentUserAdmin() || isAccountOwner)
            return MapToResponse(entity, user);

        throw new ForbiddenException();
    }

    public async Task<AdminListResponse> GetAll()
    {
        if (!_userManager.IsCurrentUserAdmin())
            throw new ForbiddenException();

        var entities = await _adminRepository.GetAll();

        var adminListView = new List<AdminView>();

        foreach (var entity in entities)
        {
            var user = await _userManager.FindByIdAsync(entity.IdentityUserId);
            if (user == null)
                throw new NotFoundException();

            adminListView.Add(new AdminView
            {
                Id = entity.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                UserId = entity.IdentityUserId
            });
        }

        var response = new AdminListResponse
        {
            Data = adminListView
        };
        return response;
    }

    public async Task<AdminResponse> Add(CreateAdminRequest request, bool bypassRoles = false)
    {
        if (!_userManager.IsCurrentUserAdmin() && !bypassRoles)
            throw new ForbiddenException();

        var identityUser = new IdentityUser
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(identityUser, request.Password);

        if (result)
        {
            result = await _userManager.AddToRoleAsync(identityUser.Id, "Admin");

            var entity = new Admin
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IdentityUserId = identityUser.Id
            };

            var created = await _adminRepository.Create(entity);
            return MapToResponse(created, identityUser);
        }

        throw new Exception("Wystąpił błąd podczas tworzenia użytkownika");
    }

    public async Task<AdminResponse> Update(Guid id, UpdateAdminRequest request)
    {
        var entity = await _adminRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException();

        var user = await _userManager.FindByIdAsync(entity.IdentityUserId);

        if (user == null)
            throw new NotFoundException();

        var isAccountOwner = _userManager.IsUserAccountOwner(entity.IdentityUserId);

        if (!_userManager.IsCurrentUserAdmin() && !isAccountOwner)
            throw new ForbiddenException();

        user.UserName = request.UserName;
        user.Email = request.Email;

        var result = await _userManager.UpdateAsync(user);

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;

        var updated = await _adminRepository.Update(entity);
        return MapToResponse(updated, user);
    }

    public async Task<AdminResponse> Delete(Guid id)
    {
        var entity = await _adminRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException();

        var user = await _userManager.FindByIdAsync(entity.IdentityUserId);
        var isAccountOwner = _userManager.IsUserAccountOwner(entity.IdentityUserId);

        if (!_userManager.IsCurrentUserAdmin() && !isAccountOwner)
            throw new ForbiddenException();

        var result = await _userManager.DeleteAsync(entity.IdentityUserId);

        await _adminRepository.Delete(id);
        return MapToResponse(entity, user);
    }

    private static AdminResponse MapToResponse(Admin entity, IdentityUser? user)
    {
        return new AdminResponse
        {
            Data = new AdminView
            {
                Id = entity.Id,
                UserName = user?.UserName ?? "",
                Email = user?.Email ?? "",
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                UserId = entity.IdentityUserId
            }
        };
    }

    public async Task<AdminResponse> GetCurrent()
    {
        var userResponse = await _userService.GetCurrentUser();
        var userId = userResponse.Data.Id;

        var entity = await _adminRepository.GetByUserId(userId);

        var user = await _userManager.FindByIdAsync(userId);

        if (entity == null)
        {
            throw new NotFoundException("Profil menadżera nie istnieje dla tego użytkownika.");
        }

        return MapToResponse(entity, user);
    }
}