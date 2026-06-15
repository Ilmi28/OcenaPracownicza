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
