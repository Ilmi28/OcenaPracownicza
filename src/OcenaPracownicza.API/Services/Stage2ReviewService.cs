using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Services;

public class Stage2ReviewService(ApplicationDbContext context, IUserManager userManager) : IStage2ReviewService
{
    public async Task<BaseResponse<List<Stage2ReviewItemView>>> GetPendingAsync()
    {
        var items = await context.Employees
            .Where(e => e.Stage2Status == EvaluationStageStatus.PendingStage2)
            .Select(e => new Stage2ReviewItemView
            {
                EmployeeId = e.Id,
                FullName = $"{e.FirstName} {e.LastName}",
                Position = e.Position,
                Period = e.Period,
                FinalScore = e.FinalScore,
                Stage2Status = (int)e.Stage2Status,
                AchievementsCount = context.Achievements.Count(a => a.EmployeeId == e.Id)
            })
            .ToListAsync();

        return new BaseResponse<List<Stage2ReviewItemView>>
        {
            Data = items
        };
    }

    public async Task<BaseResponse<List<Stage2ReviewItemView>>> GetArchivedAsync()
    {
        var items = await context.Employees
            .Where(e => e.Stage2Status == EvaluationStageStatus.Archived)
            .Select(e => new Stage2ReviewItemView
            {
                EmployeeId = e.Id,
                FullName = $"{e.FirstName} {e.LastName}",
                Position = e.Position,
                Period = e.Period,
                FinalScore = e.FinalScore,
                Stage2Status = (int)e.Stage2Status,
                AchievementsCount = context.Achievements.Count(a => a.EmployeeId == e.Id)
            })
            .ToListAsync();

        return new BaseResponse<List<Stage2ReviewItemView>>
        {
            Data = items
        };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> GetDetailsAsync(Guid employeeId)
    {
        var details = await BuildDetails(employeeId);
        return new BaseResponse<Stage2ReviewDetailsView> { Data = details };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> ApproveAsync(Guid employeeId, string? comment)
    {
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
            throw new NotFoundException("Nie znaleziono pracownika.");
        }

        Stage2TransitionValidator.EnsureCanReview(employee.Stage2Status);
        var reviewerId = userManager.GetCurrentUserId() ?? throw new UnauthorizedAccessException();

        employee.Stage2Status = EvaluationStageStatus.Stage2Approved;
        employee.Stage2Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        employee.Stage2ReviewedAtUtc = DateTime.UtcNow;
        employee.Stage2ReviewedByUserId = reviewerId;

        var achievements = await context.Achievements.Where(a => a.EmployeeId == employeeId).ToListAsync();
        foreach (var achievement in achievements)
        {
            achievement.Stage2Status = EvaluationStageStatus.Stage2Approved;
            achievement.Stage2Comment = employee.Stage2Comment;
        }

        await context.SaveChangesAsync();
        return new BaseResponse<Stage2ReviewDetailsView> { Data = await BuildDetails(employeeId) };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> RejectAsync(Guid employeeId, string? comment)
    {
        Stage2TransitionValidator.EnsureRejectComment(comment);

        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
            throw new NotFoundException("Nie znaleziono pracownika.");
        }

        Stage2TransitionValidator.EnsureCanReview(employee.Stage2Status);
        var reviewerId = userManager.GetCurrentUserId() ?? throw new UnauthorizedAccessException();
        var normalizedComment = comment!.Trim();

        employee.Stage2Status = EvaluationStageStatus.Stage2Rejected;
        employee.Stage2Comment = normalizedComment;
        employee.Stage2ReviewedAtUtc = DateTime.UtcNow;
        employee.Stage2ReviewedByUserId = reviewerId;

        var achievements = await context.Achievements.Where(a => a.EmployeeId == employeeId).ToListAsync();
        foreach (var achievement in achievements)
        {
            achievement.Stage2Status = EvaluationStageStatus.Stage2Rejected;
            achievement.Stage2Comment = normalizedComment;
        }

        await context.SaveChangesAsync();
        return new BaseResponse<Stage2ReviewDetailsView> { Data = await BuildDetails(employeeId) };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> CloseAsync(Guid employeeId)
    {
        EnsureAdminRole();

        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
            throw new NotFoundException("Nie znaleziono pracownika.");
        }

        Stage2TransitionValidator.EnsureCanClose(employee.Stage2Status);

        employee.Stage2Status = EvaluationStageStatus.Closed;
        var achievements = await context.Achievements.Where(a => a.EmployeeId == employeeId).ToListAsync();
        foreach (var achievement in achievements)
        {
            achievement.Stage2Status = EvaluationStageStatus.Closed;
        }

        await context.SaveChangesAsync();
        return new BaseResponse<Stage2ReviewDetailsView> { Data = await BuildDetails(employeeId) };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> ArchiveAsync(Guid employeeId)
    {
        EnsureAdminRole();

        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
            throw new NotFoundException("Nie znaleziono pracownika.");
        }

        Stage2TransitionValidator.EnsureCanArchive(employee.Stage2Status);

        employee.Stage2Status = EvaluationStageStatus.Archived;
        var achievements = await context.Achievements.Where(a => a.EmployeeId == employeeId).ToListAsync();
        foreach (var achievement in achievements)
        {
            achievement.Stage2Status = EvaluationStageStatus.Archived;
        }

        await context.SaveChangesAsync();
        return new BaseResponse<Stage2ReviewDetailsView> { Data = await BuildDetails(employeeId) };
    }

    private async Task<Stage2ReviewDetailsView> BuildDetails(Guid employeeId)
    {
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
            throw new NotFoundException("Nie znaleziono pracownika.");
        }

        var achievements = await context.Achievements
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return new Stage2ReviewDetailsView
        {
            EmployeeId = employee.Id,
            FullName = $"{employee.FirstName} {employee.LastName}",
            Position = employee.Position,
            Period = employee.Period,
            FinalScore = employee.FinalScore,
            AchievementsSummary = employee.AchievementsSummary,
            Stage2Status = (int)employee.Stage2Status,
            Stage2Comment = employee.Stage2Comment,
            Stage2ReviewedAtUtc = employee.Stage2ReviewedAtUtc,
            Stage2ReviewedByUserId = employee.Stage2ReviewedByUserId,
            Achievements = achievements.Select(a => new AchievementStage2View
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Date = a.Date,
                Category = (int)a.Category,
                Stage2Status = (int)a.Stage2Status,
                Stage2Comment = a.Stage2Comment
            }).ToList()
        };
    }

    private void EnsureAdminRole()
    {
        if (!userManager.IsCurrentUserAdmin())
        {
            throw new ForbiddenException("Tylko administrator może wykonać tę operację.");
        }
    }
}
