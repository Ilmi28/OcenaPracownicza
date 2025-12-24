using Microsoft.AspNetCore.Authorization;
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

public class EmployeeService : IEmployeeService
{
    private readonly IUserManager _userManager;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserService _userService;

    public EmployeeService(IUserManager userManager, IEmployeeRepository employeeRepository, IUserService userService)
    {
        _userManager = userManager;
        _employeeRepository = employeeRepository;
        _userService = userService;
    }

    public async Task<EmployeeResponse> GetById(Guid id)
    {
        var entity = await _employeeRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException();

        var isAccountOwner = _userManager.IsUserAccountOwner(entity.IdentityUserId);
        if (_userManager.IsCurrentUserAdmin() || _userManager.IsCurrentUserManager() || isAccountOwner) 
            return MapToResponse(entity);
        throw new ForbiddenException();
    }

    public async Task<EmployeeListResponse> GetAll()
    {
        if (!_userManager.IsCurrentUserAdmin() && !_userManager.IsCurrentUserManager())
            throw new ForbiddenException();
        var entities = await _employeeRepository.GetAll();
        var response = new EmployeeListResponse
        {
            Data = entities.Select(x =>
            {
                return new EmployeeView
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Position = x.Position,
                    Period = x.Period,
                    FinalScore = x.FinalScore,
                    AchievementsSummary = x.AchievementsSummary,
                    UserId = x.IdentityUserId
                };
            }).ToList()
        };
        return response;
    }

    public async Task<EmployeeResponse> Add(CreateEmployeeRequest request)
    {
        if (!_userManager.IsCurrentUserAdmin() && !_userManager.IsCurrentUserManager())
            throw new ForbiddenException();
        var identityUser = new IdentityUser
        {
            UserName = request.UserName,
            Email = request.Email
        };
        var result = await _userManager.CreateAsync(identityUser, request.Password);

        if (result)
        {
            result = await _userManager.AddToRoleAsync(identityUser.Id, "Employee");
            var entity = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Position = request.Position,
                Period = request.Period,
                FinalScore = request.FinalScore,
                AchievementsSummary = request.AchievementsSummary,
                IdentityUserId = identityUser.Id
            };

            var created = await _employeeRepository.Create(entity);
            return MapToResponse(created);
        }
        throw new Exception("Wystąpił błąd podczas tworzenia użytkownika");

    }

    public async Task<EmployeeResponse> Update(Guid id, UpdateEmployeeRequest request)
    {
        var entity = await _employeeRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException();

        var user = await _userManager.FindByIdAsync(entity.IdentityUserId);

        if (user == null)
            throw new NotFoundException();

        var isAccountOwner = _userManager.IsUserAccountOwner(entity.IdentityUserId);
        if (!_userManager.IsCurrentUserAdmin() && !_userManager.IsCurrentUserManager() && !isAccountOwner)
            throw new ForbiddenException();

        user.UserName = request.UserName;
        user.Email = request.Email;

        var result = await _userManager.UpdateAsync(user);

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Position = request.Position;
        entity.Period = request.Period;
        entity.FinalScore = request.FinalScore;
        entity.AchievementsSummary = request.AchievementsSummary;

        var updated = await _employeeRepository.Update(entity);
        return MapToResponse(updated);
    }

    public async Task<EmployeeResponse> Delete(Guid id)
    {
        
        var entity = await _employeeRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException();

        var isAccountOwner = _userManager.IsUserAccountOwner(entity.IdentityUserId);
        if (!_userManager.IsCurrentUserAdmin() && !_userManager.IsCurrentUserManager() && !isAccountOwner)
            throw new ForbiddenException();

        var result = await _userManager.DeleteAsync(entity.IdentityUserId);

        await _employeeRepository.Delete(id);
        return MapToResponse(entity);
    }


    public async Task<EmployeeResponse> GetCurrent()
    {
        var userResponse = await _userService.GetCurrentUser();
        var userId = userResponse.Data.Id;

        var entity = await _employeeRepository.GetByUserId(userId);

        if (entity == null)
        {
            throw new NotFoundException("Profil pracownika nie istnieje dla tego użytkownika.");
        }

        return MapToResponse(entity);
    }

    private static EmployeeResponse MapToResponse(Employee entity)
    {
        return new EmployeeResponse
        {
            Data = new EmployeeView
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Position = entity.Position,
                Period = entity.Period,
                FinalScore = entity.FinalScore,
                AchievementsSummary = entity.AchievementsSummary
            }
        };
    }
}
