using Microsoft.AspNetCore.Identity;
using Moq;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Services;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.UnitTests;

public class ManagerServiceTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly Mock<IManagerRepository> _managerRepoMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly ManagerService _service;

    public ManagerServiceTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _managerRepoMock = new Mock<IManagerRepository>();
        _userServiceMock = new Mock<IUserService>();
        _service = new ManagerService(_userManagerMock.Object, _managerRepoMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task GetById_ReturnsManager_WhenUserIsAdmin()
    {
        var manId = Guid.NewGuid();
        var manager = new Manager
        {
            Id = manId,
            FirstName = "Jan",
            LastName = "Kowalski",
            AchievementsSummary = "Good performance",
            IdentityUserId = "user123"
        };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(manager);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(new IdentityUser { Id = "user123", UserName = "jan", Email = "jan@mail.com" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);

        var result = await _service.GetById(manId);

        Assert.NotNull(result);
        Assert.Equal("Jan", result.Data.FirstName);
        Assert.Equal("Kowalski", result.Data.LastName);
    }

    [Fact]
    public async Task GetById_ReturnsManager_WhenUserIsAccountOwner()
    {
        var manId = Guid.NewGuid();
        var manager = new Manager
        {
            Id = manId,
            FirstName = "Adam",
            LastName = "Nowak",
            AchievementsSummary = "Own account",
            IdentityUserId = "owner456"
        };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(manager);
        _userManagerMock.Setup(u => u.FindByIdAsync("owner456")).ReturnsAsync(new IdentityUser { Id = "owner456", UserName = "adam", Email = "adam@mail.com" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("owner456")).Returns(true);

        var result = await _service.GetById(manId);

        Assert.NotNull(result);
        Assert.Equal("Adam", result.Data.FirstName);
    }

    [Fact]
    public async Task GetById_ThrowsForbiddenException_WhenUserNotAuthorized()
    {
        var manId = Guid.NewGuid();
        var manager = new Manager
        {
            FirstName = "John",
            LastName = "Smith",
            Id = manId,
            AchievementsSummary = "Average",
            IdentityUserId = "otheruser789"
        };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(manager);
        _userManagerMock.Setup(u => u.FindByIdAsync("otheruser789")).ReturnsAsync(new IdentityUser { Id = "otheruser789" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("otheruser789")).Returns(false);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetById(manId));
    }

    [Fact]
    public async Task GetById_ThrowsNotFoundException_WhenManagerNotExists()
    {
        var manId = Guid.NewGuid();
        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync((Manager)null!);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetById(manId));
    }

    [Fact]
    public async Task GetAll_ReturnsAllManagers_WhenUserIsAdmin()
    {
        var managers = new List<Manager>
        {
            new() { IdentityUserId = "user1", Id = Guid.NewGuid(), FirstName = "Manager1", LastName = "A", AchievementsSummary = "Good" },
            new() { IdentityUserId = "user2", Id = Guid.NewGuid(), FirstName = "Manager2", LastName = "B", AchievementsSummary = "Excellent" }
        };

        _managerRepoMock.Setup(r => r.GetAll()).ReturnsAsync(managers);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(new IdentityUser { Id = "user1", UserName = "u1", Email = "u1@mail.com" });
        _userManagerMock.Setup(u => u.FindByIdAsync("user2")).ReturnsAsync(new IdentityUser { Id = "user2", UserName = "u2", Email = "u2@mail.com" });

        var result = await _service.GetAll();

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Contains(result.Data, e => e.FirstName == "Manager1");
    }

    [Fact]
    public async Task GetAll_ThrowsForbiddenException_WhenUserNotAdmin()
    {
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetAll());
        _managerRepoMock.Verify(r => r.GetAll(), Times.Never);
    }

    [Fact]
    public async Task Add_CreatesManager_WhenUserIsAdmin()
    {
        var request = new CreateManagerRequest
        {
            UserName = "newmanager",
            Email = "new@example.com",
            Password = "Password123!",
            FirstName = "New",
            LastName = "Manager",
            AchievementsSummary = "Hired"
        };

        var manager = new Manager
        {
            Id = Guid.NewGuid(),
            FirstName = "New",
            LastName = "Manager",
            AchievementsSummary = "Test",
            IdentityUserId = "newManagerId"
        };

        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), "Password123!"))
            .ReturnsAsync(true)
            .Callback<IdentityUser, string>((u, p) => u.Id = "newManagerId");
        _userManagerMock.Setup(u => u.AddToRoleAsync("newManagerId", "Manager")).ReturnsAsync(true);
        _managerRepoMock.Setup(r => r.Create(It.IsAny<Manager>())).ReturnsAsync(manager);

        var result = await _service.Add(request);

        Assert.NotNull(result);
        Assert.Equal("New", result.Data.FirstName);
        _userManagerMock.Verify(u => u.AddToRoleAsync("newManagerId", "Manager"), Times.Once);
    }

    [Fact]
    public async Task Add_ThrowsForbiddenException_WhenUserNotAdmin()
    {
        var request = new CreateManagerRequest { UserName = "test", Email = "test@example.com", Password = "Password123!" };

        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.Add(request));
        _managerRepoMock.Verify(r => r.Create(It.IsAny<Manager>()), Times.Never);
    }

    [Fact]
    public async Task Add_ThrowsException_WhenUserCreationFails()
    {
        var request = new CreateManagerRequest { UserName = "fail", Email = "fail@example.com", Password = "Password123!" };

        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.Add(request));
        Assert.Equal("Wystąpił błąd podczas tworzenia użytkownika", ex.Message);
        _managerRepoMock.Verify(r => r.Create(It.IsAny<Manager>()), Times.Never);
    }

    [Fact]
    public async Task Update_UpdatesManager_WhenUserIsAdmin()
    {
        var manId = Guid.NewGuid();
        var existing = new Manager { Id = manId, FirstName = "Old", LastName = "Manager", IdentityUserId = "user123", AchievementsSummary = "Old summary" };
        var identityUser = new IdentityUser { Id = "user123", UserName = "oldname", Email = "old@mail.com" };
        var request = new UpdateManagerRequest
        {
            UserName = "newname",
            Email = "new@mail.com",
            FirstName = "New",
            LastName = "Manager",
            AchievementsSummary = "Updated summary"
        };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(existing);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);
        _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
        _managerRepoMock.Setup(r => r.Update(It.IsAny<Manager>())).ReturnsAsync((Manager e) => e);

        var result = await _service.Update(manId, request);

        Assert.Equal("New", result.Data.FirstName);
        Assert.Equal("Updated summary", result.Data.AchievementsSummary);
        _userManagerMock.Verify(u => u.UpdateAsync(It.Is<IdentityUser>(u => u.UserName == "newname")), Times.Once);
    }

    [Fact]
    public async Task Update_UpdatesManager_WhenUserIsAccountOwner()
    {
        var manId = Guid.NewGuid();
        var existing = new Manager { Id = manId, FirstName = "Old", LastName = "Manager", IdentityUserId = "owner123", AchievementsSummary = "Old summary" };
        var identityUser = new IdentityUser { Id = "owner123" };
        var request = new UpdateManagerRequest
        {
            FirstName = "Self",
            LastName = "Updated",
            UserName = "u",
            Email = "e",
            AchievementsSummary = existing.AchievementsSummary
        };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(existing);
        _userManagerMock.Setup(u => u.FindByIdAsync("owner123")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("owner123")).Returns(true);
        _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
        _managerRepoMock.Setup(r => r.Update(It.IsAny<Manager>())).ReturnsAsync((Manager e) => e);

        var result = await _service.Update(manId, request);

        Assert.Equal("Self", result.Data.FirstName);
    }

    [Fact]
    public async Task Update_ThrowsForbiddenException_WhenUserNotAuthorized()
    {
        var manId = Guid.NewGuid();
        var existing = new Manager { Id = manId, FirstName = "TestName", LastName = "TestLastName", AchievementsSummary = "TestNew1", IdentityUserId = "user123" };
        var identityUser = new IdentityUser { Id = "user123" };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(existing);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);

        var request = new UpdateManagerRequest { FirstName = "Test" };

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.Update(manId, request));
        _managerRepoMock.Verify(r => r.Update(It.IsAny<Manager>()), Times.Never);
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenManagerNotExists()
    {
        var manId = Guid.NewGuid();
        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync((Manager)null!);
        var request = new UpdateManagerRequest { FirstName = "Test" };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(manId, request));
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenIdentityUserNotExists()
    {
        var manId = Guid.NewGuid();
        var existing = new Manager { Id = manId, FirstName = "TestName", LastName = "TestLastName", AchievementsSummary = "TestNew1", IdentityUserId = "user123" };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(existing);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync((IdentityUser)null!);
        var request = new UpdateManagerRequest { FirstName = "Test" };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(manId, request));
    }

    [Fact]
    public async Task Delete_DeletesManager_WhenUserIsAdmin()
    {
        var manId = Guid.NewGuid();
        var manager = new Manager { Id = manId, FirstName = "Jan", LastName = "TestLastName", AchievementsSummary = "TestNew1", IdentityUserId = "user123" };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(manager);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(new IdentityUser { Id = "user123" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);
        _userManagerMock.Setup(u => u.DeleteAsync("user123")).ReturnsAsync(true);
        _managerRepoMock.Setup(r => r.Delete(manId)).Returns(Task.CompletedTask);

        var result = await _service.Delete(manId);

        Assert.NotNull(result);
        Assert.Equal("Jan", result.Data.FirstName);
        _userManagerMock.Verify(u => u.DeleteAsync("user123"), Times.Once);
        _managerRepoMock.Verify(r => r.Delete(manId), Times.Once);
    }

    [Fact]
    public async Task Delete_DeletesManager_WhenUserIsAccountOwner()
    {
        var manId = Guid.NewGuid();
        var manager = new Manager { Id = manId, FirstName = "Self", LastName = "TestLastName", AchievementsSummary = "TestNew1", IdentityUserId = "owner456" };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(manager);
        _userManagerMock.Setup(u => u.FindByIdAsync("owner456")).ReturnsAsync(new IdentityUser { Id = "owner456" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("owner456")).Returns(true);
        _userManagerMock.Setup(u => u.DeleteAsync("owner456")).ReturnsAsync(true);
        _managerRepoMock.Setup(r => r.Delete(manId)).Returns(Task.CompletedTask);

        var result = await _service.Delete(manId);

        Assert.NotNull(result);
        _managerRepoMock.Verify(r => r.Delete(manId), Times.Once);
    }

    [Fact]
    public async Task Delete_ThrowsNotFoundException_WhenManagerNotExists()
    {
        var manId = Guid.NewGuid();
        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync((Manager)null!);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.Delete(manId));
        _userManagerMock.Verify(u => u.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ThrowsForbiddenException_WhenUserNotAuthorized()
    {
        var manId = Guid.NewGuid();
        var manager = new Manager { Id = manId, FirstName = "TestName", LastName = "TestLastName", AchievementsSummary = "TestNew1", IdentityUserId = "user123" };

        _managerRepoMock.Setup(r => r.GetById(manId)).ReturnsAsync(manager);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(new IdentityUser { Id = "user123" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.Delete(manId));
        _userManagerMock.Verify(u => u.DeleteAsync(It.IsAny<string>()), Times.Never);
        _managerRepoMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetCurrent_ReturnsManagerProfile_ForCurrentUser()
    {
        var userId = "user123";
        var manager = new Manager
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Kowalski",
            AchievementsSummary = "Good performance",
            IdentityUserId = userId
        };

        _userServiceMock
            .Setup(s => s.GetCurrentUser())
            .ReturnsAsync(new UserResponse { Data = new UserView { Id = userId } });

        _managerRepoMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(manager);

        _userManagerMock
            .Setup(u => u.FindByIdAsync(userId))
            .ReturnsAsync(new IdentityUser { Id = userId, UserName = "jan", Email = "jan@mail.com" });

        var result = await _service.GetCurrent();

        Assert.NotNull(result);
        Assert.Equal(manager.Id, result.Data.Id);
        Assert.Equal("Jan", result.Data.FirstName);
        Assert.Equal("Kowalski", result.Data.LastName);
        Assert.Equal("Good performance", result.Data.AchievementsSummary);
        Assert.Equal("jan", result.Data.UserName);
        Assert.Equal("jan@mail.com", result.Data.Email);
        Assert.Equal(userId, result.Data.UserId);
    }

    [Fact]
    public async Task GetCurrent_ThrowsNotFoundException_WhenProfileDoesNotExist()
    {
        var userId = "user123";

        _userServiceMock
            .Setup(s => s.GetCurrentUser())
            .ReturnsAsync(new UserResponse { Data = new UserView { Id = userId } });

        _managerRepoMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync((Manager)null!);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCurrent());
        Assert.Equal("Profil menadżera nie istnieje dla tego użytkownika.", ex.Message);
    }
}