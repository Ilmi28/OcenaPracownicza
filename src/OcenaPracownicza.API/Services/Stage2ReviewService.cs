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
    public async Task<BaseResponse<List<Stage2HistoryItemView>>> GetHistoryAsync()
    {
        var items = await context.Achievements
            .Include(a => a.Employee)
            .Select(a => new Stage2HistoryItemView
            {
                AchievementId = a.Id,
                EmployeeId = a.EmployeeId,
                FullName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                Position = a.Employee.Position,
                AchievementName = a.Name,
                Period = a.Period,
                FinalScore = a.FinalScore,
                Stage2Status = (int)a.Stage2Status,
                Date = a.Date,
                Stage2ReviewedAtUtc = a.Stage2ReviewedAtUtc
            })
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return new BaseResponse<List<Stage2HistoryItemView>>
        {
            Data = items
        };
    }
    
    public async Task<BaseResponse<List<Stage2HistoryItemView>>> GetMyHistoryAsync()
    {
        var currentEmployeeId = await GetCurrentEmployeeIdAsync();
        var items = await context.Achievements
            .Include(a => a.Employee)
            .Where(a => a.EmployeeId == currentEmployeeId)
            .Select(a => new Stage2HistoryItemView
            {
                AchievementId = a.Id,
                EmployeeId = a.EmployeeId,
                FullName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                Position = a.Employee.Position,
                AchievementName = a.Name,
                Period = a.Period,
                FinalScore = a.FinalScore,
                Stage2Status = (int)a.Stage2Status,
                Date = a.Date,
                Stage2ReviewedAtUtc = a.Stage2ReviewedAtUtc
            })
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return new BaseResponse<List<Stage2HistoryItemView>>
        {
            Data = items
        };
    }

    public async Task<BaseResponse<List<Stage2ReviewItemView>>> GetPendingAsync()
    {
        var items = await context.Achievements
            .Include(a => a.Employee)
            .Where(a => a.Stage2Status == EvaluationStageStatus.PendingStage2)
            .Select(a => new Stage2ReviewItemView
            {
                AchievementId = a.Id,
                EmployeeId = a.EmployeeId,
                FullName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                Position = a.Employee.Position,
                AchievementName = a.Name,
                Period = a.Period,
                FinalScore = a.FinalScore,
                Stage2Status = (int)a.Stage2Status
            })
            .OrderByDescending(a => a.Period)
            .ToListAsync();

        return new BaseResponse<List<Stage2ReviewItemView>>
        {
            Data = items
        };
    }

    public async Task<BaseResponse<List<Stage2ReviewItemView>>> GetApprovedAsync()
    {
        EnsureAdminRole();

        var items = await context.Achievements
            .Include(a => a.Employee)
            .Where(a => a.Stage2Status == EvaluationStageStatus.Stage2Approved)
            .Select(a => new Stage2ReviewItemView
            {
                AchievementId = a.Id,
                EmployeeId = a.EmployeeId,
                FullName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                Position = a.Employee.Position,
                AchievementName = a.Name,
                Period = a.Period,
                FinalScore = a.FinalScore,
                Stage2Status = (int)a.Stage2Status
            })
            .OrderByDescending(a => a.Period)
            .ToListAsync();

        return new BaseResponse<List<Stage2ReviewItemView>>
        {
            Data = items
        };
    }

    public async Task<BaseResponse<List<Stage2ReviewItemView>>> GetArchivedAsync()
    {
        var items = await context.Achievements
            .Include(a => a.Employee)
            .Where(a => a.Stage2Status == EvaluationStageStatus.Archived)
            .Select(a => new Stage2ReviewItemView
            {
                AchievementId = a.Id,
                EmployeeId = a.EmployeeId,
                FullName = $"{a.Employee.FirstName} {a.Employee.LastName}",
                Position = a.Employee.Position,
                AchievementName = a.Name,
                Period = a.Period,
                FinalScore = a.FinalScore,
                Stage2Status = (int)a.Stage2Status
            })
            .OrderByDescending(a => a.Period)
            .ToListAsync();

        return new BaseResponse<List<Stage2ReviewItemView>>
        {
            Data = items
        };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> GetDetailsAsync(Guid achievementId)
    {
        var details = await BuildDetails(achievementId);
        return new BaseResponse<Stage2ReviewDetailsView> { Data = details };
    }
    
    public async Task<BaseResponse<Stage2ReviewDetailsView>> GetMyDetailsAsync(Guid achievementId)
    {
        var currentEmployeeId = await GetCurrentEmployeeIdAsync();
        var details = await BuildDetails(achievementId, currentEmployeeId);
        return new BaseResponse<Stage2ReviewDetailsView> { Data = details };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> ApproveAsync(Guid achievementId, string? comment)
    {
        var achievement = await context.Achievements.FirstOrDefaultAsync(a => a.Id == achievementId);
        if (achievement == null)
        {
            throw new NotFoundException("Nie znaleziono osiągnięcia.");
        }

        Stage2TransitionValidator.EnsureCanReview(achievement.Stage2Status);
        var reviewerId = userManager.GetCurrentUserId() ?? throw new UnauthorizedAccessException();

        achievement.Stage2Status = EvaluationStageStatus.Stage2Approved;
        achievement.Stage2Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        achievement.Stage2ReviewedAtUtc = DateTime.UtcNow;
        achievement.Stage2ReviewedByUserId = reviewerId;

        await context.SaveChangesAsync();
        return new BaseResponse<Stage2ReviewDetailsView> { Data = await BuildDetails(achievementId) };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> RejectAsync(Guid achievementId, string? comment)
    {
        Stage2TransitionValidator.EnsureRejectComment(comment);

        var achievement = await context.Achievements.FirstOrDefaultAsync(a => a.Id == achievementId);
        if (achievement == null)
        {
            throw new NotFoundException("Nie znaleziono osiągnięcia.");
        }

        Stage2TransitionValidator.EnsureCanReview(achievement.Stage2Status);
        var reviewerId = userManager.GetCurrentUserId() ?? throw new UnauthorizedAccessException();
        var normalizedComment = comment!.Trim();

        achievement.Stage2Status = EvaluationStageStatus.Stage2Rejected;
        achievement.Stage2Comment = normalizedComment;
        achievement.Stage2ReviewedAtUtc = DateTime.UtcNow;
        achievement.Stage2ReviewedByUserId = reviewerId;

        await context.SaveChangesAsync();
        return new BaseResponse<Stage2ReviewDetailsView> { Data = await BuildDetails(achievementId) };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> CloseAsync(Guid achievementId)
    {
        EnsureAdminRole();

        var achievement = await context.Achievements.FirstOrDefaultAsync(a => a.Id == achievementId);
        if (achievement == null)
        {
            throw new NotFoundException("Nie znaleziono osiągnięcia.");
        }

        Stage2TransitionValidator.EnsureCanClose(achievement.Stage2Status);

        achievement.Stage2Status = EvaluationStageStatus.Closed;

        await context.SaveChangesAsync();
        return new BaseResponse<Stage2ReviewDetailsView> { Data = await BuildDetails(achievementId) };
    }

    public async Task<BaseResponse<Stage2ReviewDetailsView>> ArchiveAsync(Guid achievementId)
    {
        EnsureAdminRole();

        var achievement = await context.Achievements.FirstOrDefaultAsync(a => a.Id == achievementId);
        if (achievement == null)
        {
            throw new NotFoundException("Nie znaleziono osiągnięcia.");
        }

        Stage2TransitionValidator.EnsureCanArchive(achievement.Stage2Status);

        achievement.Stage2Status = EvaluationStageStatus.Archived;

        await context.SaveChangesAsync();
        return new BaseResponse<Stage2ReviewDetailsView> { Data = await BuildDetails(achievementId) };
    }

    private async Task<Stage2ReviewDetailsView> BuildDetails(Guid achievementId, Guid? requiredEmployeeId = null)
    {
        var selectedAchievement = await context.Achievements
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a =>
                a.Id == achievementId &&
                (!requiredEmployeeId.HasValue || a.EmployeeId == requiredEmployeeId.Value));
        if (selectedAchievement == null)
        {
            throw new NotFoundException("Nie znaleziono osiągnięcia.");
        }

        var achievements = await context.Achievements
            .Where(a => a.EmployeeId == selectedAchievement.EmployeeId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return new Stage2ReviewDetailsView
        {
            AchievementId = selectedAchievement.Id,
            EmployeeId = selectedAchievement.EmployeeId,
            FullName = $"{selectedAchievement.Employee.FirstName} {selectedAchievement.Employee.LastName}",
            Position = selectedAchievement.Employee.Position,
            AchievementName = selectedAchievement.Name,
            Period = selectedAchievement.Period,
            FinalScore = selectedAchievement.FinalScore,
            AchievementsSummary = selectedAchievement.AchievementsSummary,
            Stage2Status = (int)selectedAchievement.Stage2Status,
            Stage2Comment = selectedAchievement.Stage2Comment,
            Stage2ReviewedAtUtc = selectedAchievement.Stage2ReviewedAtUtc,
            Stage2ReviewedByUserId = selectedAchievement.Stage2ReviewedByUserId,
            Achievements = achievements.Select(a => new AchievementStage2View
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Date = a.Date,
                Category = (int)a.Category,
                Period = a.Period,
                FinalScore = a.FinalScore,
                AchievementsSummary = a.AchievementsSummary,
                Stage2Status = (int)a.Stage2Status,
                Stage2Comment = a.Stage2Comment,
                Stage2ReviewedByUserId = a.Stage2ReviewedByUserId,
                Stage2ReviewedAtUtc = a.Stage2ReviewedAtUtc
            }).ToList()
        };
    }
    
    private async Task<Guid> GetCurrentEmployeeIdAsync()
    {
        var currentUserId = userManager.GetCurrentUserId() ?? throw new UnauthorizedAccessException();
        var currentEmployeeId = await context.Employees
            .Where(e => e.IdentityUserId == currentUserId)
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

        if (currentEmployeeId == Guid.Empty)
        {
            throw new NotFoundException("Nie znaleziono profilu pracownika.");
        }

        return currentEmployeeId;
    }

    private void EnsureAdminRole()
    {
        if (!userManager.IsCurrentUserAdmin())
        {
            throw new ForbiddenException("Tylko administrator może wykonać tę operację.");
        }
    }
}
