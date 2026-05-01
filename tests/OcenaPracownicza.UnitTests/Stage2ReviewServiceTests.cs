using Microsoft.EntityFrameworkCore;
using Moq;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Services;

namespace OcenaPracownicza.UnitTests;

public class Stage2ReviewServiceTests
{
    [Fact]
    public async Task GetMyHistoryAsync_ReturnsOnlyCurrentEmployeeAchievements()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"stage2-history-{Guid.NewGuid()}")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var currentUserId = "current-user-id";
        var otherUserId = "other-user-id";

        var currentEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            IdentityUserId = currentUserId,
            FirstName = "Jan",
            LastName = "Nowak",
            Position = "Programista"
        };
        var otherEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            IdentityUserId = otherUserId,
            FirstName = "Anna",
            LastName = "Kowalska",
            Position = "Tester"
        };

        var period2026 = new EvaluationPeriod
        {
            Id = Guid.NewGuid(),
            Name = "2026-Q1",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 3, 31),
            RegulationVersion = "1.0"
        };
        context.EvaluationPeriods.Add(period2026);

        context.Employees.AddRange(currentEmployee, otherEmployee);
        context.Achievements.AddRange(
            new Achievement
            {
                Id = Guid.NewGuid(),
                EmployeeId = currentEmployee.Id,
                Name = "A",
                Description = "A",
                Date = new DateTime(2026, 1, 1),
                Category = AchievementCategory.ProjectDelivery,
                EvaluationPeriodId = period2026.Id,
                FinalScore = "9.0",
                AchievementsSummary = "A",
                Stage2Status = EvaluationStageStatus.Stage2Approved,
                Stage2Comment = "Komentarz A"
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                EmployeeId = otherEmployee.Id,
                Name = "B",
                Description = "B",
                Date = new DateTime(2026, 2, 1),
                Category = AchievementCategory.ProjectDelivery,
                EvaluationPeriodId = period2026.Id,
                FinalScore = "7.0",
                AchievementsSummary = "B",
                Stage2Status = EvaluationStageStatus.Stage2Approved,
                Stage2Comment = "Komentarz B"
            });
        await context.SaveChangesAsync();

        var userManagerMock = new Mock<IUserManager>();
        userManagerMock.Setup(x => x.GetCurrentUserId()).Returns(currentUserId);

        var service = new Stage2ReviewService(context, userManagerMock.Object);

        var result = await service.GetMyHistoryAsync();

        Assert.Single(result.Data);
        Assert.Equal(currentEmployee.Id, result.Data[0].EmployeeId);
        Assert.Equal("9.0", result.Data[0].FinalScore);
    }

    [Fact]
    public async Task GetMyDetailsAsync_ThrowsNotFound_WhenAchievementBelongsToOtherEmployee()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"stage2-details-{Guid.NewGuid()}")
            .Options;

        await using var context = new ApplicationDbContext(options);
        var currentUserId = "current-user-id";
        var otherUserId = "other-user-id";

        var currentEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            IdentityUserId = currentUserId,
            FirstName = "Jan",
            LastName = "Nowak",
            Position = "Programista"
        };
        var otherEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            IdentityUserId = otherUserId,
            FirstName = "Anna",
            LastName = "Kowalska",
            Position = "Tester"
        };

        var otherAchievementId = Guid.NewGuid();
        var period2026 = new EvaluationPeriod
        {
            Id = Guid.NewGuid(),
            Name = "2026-Q1",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 3, 31),
            RegulationVersion = "1.0"
        };
        context.EvaluationPeriods.Add(period2026);
        context.Employees.AddRange(currentEmployee, otherEmployee);
        context.Achievements.Add(new Achievement
        {
            Id = otherAchievementId,
            EmployeeId = otherEmployee.Id,
            Name = "B",
            Description = "B",
            Date = new DateTime(2026, 2, 1),
            Category = AchievementCategory.ProjectDelivery,
            EvaluationPeriodId = period2026.Id,
            FinalScore = "7.0",
            AchievementsSummary = "B",
            Stage2Status = EvaluationStageStatus.Stage2Approved,
            Stage2Comment = "Komentarz B"
        });
        await context.SaveChangesAsync();

        var userManagerMock = new Mock<IUserManager>();
        userManagerMock.Setup(x => x.GetCurrentUserId()).Returns(currentUserId);

        var service = new Stage2ReviewService(context, userManagerMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetMyDetailsAsync(otherAchievementId));
    }
}
