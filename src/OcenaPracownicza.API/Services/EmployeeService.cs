using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<EmployeeResponse> GetById(int id)
    {
        var entity = await _employeeRepository.GetById(id);
        if (entity == null)
            throw new DirectoryNotFoundException("Employee not found");

        return MapToResponse(entity);
    }

    public async Task<EmployeeListResponse> GetAll()
    {
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
                    AchievementsSummary = x.AchievementsSummary
                };
            }).ToList()
        };
        return response;
    }

    public async Task<EmployeeResponse> Add(EmployeeRequest request)
    {
        var entity = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Position = request.Position,
            Period = request.Period,
            FinalScore = request.FinalScore,
            AchievementsSummary = request.AchievementsSummary
        };

        var created = await _employeeRepository.Create(entity);
        return MapToResponse(created);
    }

    public async Task<EmployeeResponse> Update(int id, EmployeeRequest request)
    {
        var entity = await _employeeRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException("Employee not found");

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Position = request.Position;
        entity.Period = request.Period;
        entity.FinalScore = request.FinalScore;
        entity.AchievementsSummary = request.AchievementsSummary;

        var updated = await _employeeRepository.Update(entity);
        return MapToResponse(updated);
    }

    public async Task<EmployeeResponse> Delete(int id)
    {
        var entity = await _employeeRepository.GetById(id);
        if (entity == null)
            throw new NotFoundException("Employee not found");

        await _employeeRepository.Delete(id);
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
