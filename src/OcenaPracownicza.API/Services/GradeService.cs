using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Services;

public class GradeService(ApplicationDbContext context, IUserManager userManager) : IGradeService
{
    public async Task<BaseResponse<List<GradeView>>> GetEmployeeGradesAsync(Guid employeeId)
    {
        var currentUserId = userManager.GetCurrentUserId() ?? throw new UnauthorizedAccessException();

        if (userManager.IsCurrentUserManager() || userManager.IsCurrentUserAdmin())
        {
        }
        else
        {
            var myEmployeeId = await context.Employees
                .Where(e => e.IdentityUserId == currentUserId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            if (myEmployeeId == Guid.Empty)
                throw new ForbiddenException("Twoje konto nie jest powiązane z profilem pracownika.");

            employeeId = myEmployeeId;
        }

        var grades = await context.Grades
            .Include(g => g.EvaluationPeriod)
            .Where(g => g.EmployeeId == employeeId)
            .Select(g => new GradeView
            {
                Id = g.Id,
                Value = g.Value,
                Comment = g.Comment,
                EmployeeId = g.EmployeeId,
                PeriodName = g.EvaluationPeriod.Name
            })
            .ToListAsync();

        return new BaseResponse<List<GradeView>> { Data = grades };
    }
    public async Task<BaseResponse<GradeView>> CreateGradeAsync(CreateGradeRequest request)
    {
        EnsureManagerOrAdmin();
        var grade = new Grade
        {
            Value = request.Value,
            EmployeeId = request.EmployeeId,
            EvaluationPeriodId = request.EvaluationPeriodId,
            Comment = request.Comment
        };

        context.Grades.Add(grade);
        await context.SaveChangesAsync();

        return new BaseResponse<GradeView>
        {
            Data = new GradeView
            {
                Id = grade.Id,
                Value = grade.Value,
                Comment = grade.Comment,
                EmployeeId = grade.EmployeeId,
                PeriodName = await context.EvaluationPeriods    
                    .Where(p => p.Id == grade.EvaluationPeriodId)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync() ?? "Brak"
            }
        };
    }

    public async Task<BaseResponse<GradeView>> UpdateGradeAsync(Guid gradeId, UpdateGradeRequest request)
    {
        EnsureManagerOrAdmin();
        var grade = await context.Grades.FirstOrDefaultAsync(g => g.Id == gradeId)
            ?? throw new NotFoundException("Nie znaleziono oceny.");

        grade.Value = request.Value;
        grade.Comment = request.Comment;

        await context.SaveChangesAsync();
        return new BaseResponse<GradeView> { Data = new GradeView { Id = grade.Id, Value = grade.Value, Comment = grade.Comment } };
    }

    public async Task<BaseResponse<string>> DeleteGradeAsync(Guid gradeId)
    {
        EnsureManagerOrAdmin();
        var grade = await context.Grades.FirstOrDefaultAsync(g => g.Id == gradeId)
            ?? throw new NotFoundException("Nie znaleziono oceny.");

        context.Grades.Remove(grade);
        await context.SaveChangesAsync();

        return new BaseResponse<string> { Data = "Deleted" };
    }

    private void EnsureManagerOrAdmin()
    {
        if (!userManager.IsCurrentUserManager() && !userManager.IsCurrentUserAdmin())
            throw new ForbiddenException("Brak uprawnień do edycji ocen.");
    }

    public async Task<BaseResponse<List<GradeView>>> GetGradesByPeriodAsync(Guid periodId)
    {
        EnsureManagerOrAdmin();
        var grades = await context.Grades
            .Include(g => g.EvaluationPeriod)
            .Where(g => g.EvaluationPeriodId == periodId)
            .Select(g => new GradeView
            {
                Id = g.Id,
                Value = g.Value,
                Comment = g.Comment,
                EmployeeId = g.EmployeeId,
                PeriodName = g.EvaluationPeriod.Name
            })
            .ToListAsync();

        return new BaseResponse<List<GradeView>> { Data = grades };
    }
}