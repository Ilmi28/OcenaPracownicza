using Microsoft.AspNetCore.Identity;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Repositories;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Services;

public class ManagerService : IManagerService
{
    private readonly IUserManager _userManager;
    private readonly IManagerRepository _managerRepository;

    public ManagerService(IUserManager userManager, IManagerRepository managerRepository)
    {
        _userManager = userManager;
        _managerRepository = managerRepository;
    }

    public async Task<ManagerResponse> GetById(Guid id)
    {
        var entity = await _managerRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException();

        var isAccountOwner = _userManager.IsUserAccountOwner(entity.IdentityUserId);

        if (_userManager.IsCurrentUserAdmin() || isAccountOwner)
            return MapToResponse(entity);

        throw new ForbiddenException();
    }

    public async Task<ManagerListResponse> GetAll()
    {
        if (!_userManager.IsCurrentUserAdmin())
            throw new ForbiddenException();

        var entities = await _managerRepository.GetAll();
        var response = new ManagerListResponse
        {
            Data = entities.Select(x => new ManagerView
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                AchievementsSummary = x.AchievementsSummary,
                UserId = x.IdentityUserId
            }).ToList()
        };
        return response;
    }

    public async Task<ManagerResponse> Add(CreateManagerRequest request)
    {
        if (!_userManager.IsCurrentUserAdmin())
            throw new ForbiddenException();

        var identityUser = new IdentityUser
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(identityUser, request.Password);
        if (result)
        {
            await _userManager.AddToRoleAsync(identityUser.Id, "Manager");

            var entity = new Manager
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                AchievementsSummary = request.AchievementsSummary,
                IdentityUserId = identityUser.Id
            };

            var created = await _managerRepository.Create(entity);
            return MapToResponse(created);
        }

        throw new Exception("Wystąpił błąd podczas tworzenia użytkownika");
    }

    public async Task<ManagerResponse> Update(Guid id, UpdateManagerRequest request)
    {
        var entity = await _managerRepository.GetById(id);
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
        await _userManager.UpdateAsync(user);

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.AchievementsSummary = request.AchievementsSummary;

        var updated = await _managerRepository.Update(entity);
        return MapToResponse(updated);
    }

    public async Task<ManagerResponse> Delete(Guid id)
    {
        var entity = await _managerRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException();

        var isAccountOwner = _userManager.IsUserAccountOwner(entity.IdentityUserId);
        if (!_userManager.IsCurrentUserAdmin() && !isAccountOwner)
            throw new ForbiddenException();

        await _userManager.DeleteAsync(entity.IdentityUserId);
        await _managerRepository.Delete(id);

        return MapToResponse(entity);
    }

    private static ManagerResponse MapToResponse(Manager entity)
    {
        return new ManagerResponse
        {
            Data = new ManagerView
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                AchievementsSummary = entity.AchievementsSummary,
                UserId = entity.IdentityUserId
            }
        };
    }
}
