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

public class AdminServiceTests
{
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly Mock<IAdminRepository> _adminRepoMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly AdminService _service;

    public AdminServiceTests()
    {
        _userManagerMock = new Mock<IUserManager>();
        _adminRepoMock = new Mock<IAdminRepository>();
        _userServiceMock = new Mock<IUserService>();
        _service = new AdminService(_userManagerMock.Object, _adminRepoMock.Object, _userServiceMock.Object);
    }

    #region GetById Tests

    [Fact]
    public async Task GetById_ReturnsAdmin_WhenUserIsAdmin()
    {
        var adminId = Guid.NewGuid();
        var admin = new Admin
        {
            Id = adminId,
            FirstName = "Jan",
            LastName = "Kowalski",
            IdentityUserId = "user123"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(new IdentityUser { Id = "user123" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);

        var result = await _service.GetById(adminId);

        Assert.NotNull(result);
        Assert.Equal("Jan", result.Data.FirstName);
        Assert.Equal("Kowalski", result.Data.LastName);
    }

    [Fact]
    public async Task GetById_ReturnsAdmin_WhenUserIsAccountOwner()
    {
        var adminId = Guid.NewGuid();
        var admin = new Admin
        {
            Id = adminId,
            FirstName = "Adam",
            LastName = "Nowak",
            IdentityUserId = "owner456"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("owner456")).ReturnsAsync(new IdentityUser { Id = "owner456" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("owner456")).Returns(true);

        var result = await _service.GetById(adminId);

        Assert.NotNull(result);
        Assert.Equal("Adam", result.Data.FirstName);
    }

    [Fact]
    public async Task GetById_ThrowsForbiddenException_WhenUserNotAuthorized()
    {
        var adminId = Guid.NewGuid();
        var admin = new Admin
        {
            Id = adminId,
            FirstName = "John",
            LastName = "Smith",
            IdentityUserId = "otheruser789"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("otheruser789")).ReturnsAsync(new IdentityUser { Id = "otheruser789" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("otheruser789")).Returns(false);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetById(adminId));
    }

    [Fact]
    public async Task GetById_ThrowsNotFoundException_WhenAdminNotExists()
    {
        var adminId = Guid.NewGuid();
        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync((Admin)null!);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetById(adminId));
    }

    [Fact]
    public async Task GetById_MapsAllFieldsCorrectly()
    {
        var adminId = Guid.NewGuid();
        var admin = new Admin
        {
            Id = adminId,
            FirstName = "Piotr",
            LastName = "Zielinski",
            IdentityUserId = "user999"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("user999")).ReturnsAsync(new IdentityUser { Id = "user999", UserName = "piotr", Email = "piotr@mail.com" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user999")).Returns(false);

        var result = await _service.GetById(adminId);

        Assert.Equal(adminId, result.Data.Id);
        Assert.Equal("Piotr", result.Data.FirstName);
        Assert.Equal("Zielinski", result.Data.LastName);
        Assert.Equal("user999", result.Data.UserId);
        Assert.Equal("piotr", result.Data.UserName);
        Assert.Equal("piotr@mail.com", result.Data.Email);
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsAllAdmins_WhenUserIsAdmin()
    {
        var admins = new List<Admin>
        {
            new() { IdentityUserId = "user1", Id = Guid.NewGuid(), FirstName = "Admin1", LastName = "A" },
            new() { IdentityUserId = "user2", Id = Guid.NewGuid(), FirstName = "Admin2", LastName = "B" }
        };

        _adminRepoMock.Setup(r => r.GetAll()).ReturnsAsync(admins);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.FindByIdAsync("user1")).ReturnsAsync(new IdentityUser { Id = "user1", UserName = "u1", Email = "u1@mail.com" });
        _userManagerMock.Setup(u => u.FindByIdAsync("user2")).ReturnsAsync(new IdentityUser { Id = "user2", UserName = "u2", Email = "u2@mail.com" });

        var result = await _service.GetAll();

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Contains(result.Data, e => e.FirstName == "Admin1");
        Assert.Contains(result.Data, e => e.FirstName == "Admin2");
    }

    [Fact]
    public async Task GetAll_ThrowsForbiddenException_WhenUserNotAdmin()
    {
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetAll());
        _adminRepoMock.Verify(r => r.GetAll(), Times.Never);
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoAdminsExist()
    {
        var admins = new List<Admin>();

        _adminRepoMock.Setup(r => r.GetAll()).ReturnsAsync(admins);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);

        var result = await _service.GetAll();

        Assert.NotNull(result);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetAll_MapsAllFieldsCorrectly()
    {
        var adminId = Guid.NewGuid();
        var admins = new List<Admin>
        {
            new() { Id = adminId, IdentityUserId = "user123", FirstName = "Test", LastName = "Admin" }
        };

        _adminRepoMock.Setup(r => r.GetAll()).ReturnsAsync(admins);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(new IdentityUser { Id = "user123", UserName = "testadmin", Email = "testadmin@mail.com" });

        var result = await _service.GetAll();

        Assert.Single(result.Data);
        var admin = result.Data.First();
        Assert.Equal(adminId, admin.Id);
        Assert.Equal("Test", admin.FirstName);
        Assert.Equal("Admin", admin.LastName);
        Assert.Equal("user123", admin.UserId);
        Assert.Equal("testadmin", admin.UserName);
        Assert.Equal("testadmin@mail.com", admin.Email);
    }

    #endregion

    #region Add Tests

    [Fact]
    public async Task Add_CreatesAdmin_WhenUserIsAdmin()
    {
        var request = new CreateAdminRequest
        {
            UserName = "newadmin",
            Email = "new@example.com",
            Password = "Password123!",
            FirstName = "New",
            LastName = "Admin"
        };

        var admin = new Admin
        {
            Id = Guid.NewGuid(),
            FirstName = "New",
            LastName = "Admin",
            IdentityUserId = "newAdminId"
        };

        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), "Password123!"))
            .ReturnsAsync(true)
            .Callback<IdentityUser, string>((u, p) => u.Id = "newAdminId");
        _userManagerMock.Setup(u => u.AddToRoleAsync("newAdminId", "Admin")).ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Create(It.IsAny<Admin>())).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("newAdminId")).ReturnsAsync(new IdentityUser { Id = "newAdminId", UserName = "newadmin", Email = "new@example.com" });

        var result = await _service.Add(request);

        Assert.NotNull(result);
        Assert.Equal("New", result.Data.FirstName);
        Assert.Equal("Admin", result.Data.LastName);
        _userManagerMock.Verify(u => u.AddToRoleAsync("newAdminId", "Admin"), Times.Once);
    }

    [Fact]
    public async Task Add_ThrowsForbiddenException_WhenUserNotAdmin()
    {
        var request = new CreateAdminRequest
        {
            UserName = "test",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.Add(request));
        _adminRepoMock.Verify(r => r.Create(It.IsAny<Admin>()), Times.Never);
    }

    [Fact]
    public async Task Add_ThrowsException_WhenUserCreationFails()
    {
        var request = new CreateAdminRequest
        {
            UserName = "fail",
            Email = "fail@example.com",
            Password = "Password123! ",
            FirstName = "Test",
            LastName = "User"
        };

        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<Exception>(() => _service.Add(request));
        Assert.Equal("Wystąpił błąd podczas tworzenia użytkownika", ex.Message);
        _adminRepoMock.Verify(r => r.Create(It.IsAny<Admin>()), Times.Never);
    }

    [Fact]
    public async Task Add_MapsAllFieldsCorrectly()
    {
        var request = new CreateAdminRequest
        {
            UserName = "superadmin",
            Email = "super@example.com",
            Password = "Password123!",
            FirstName = "Super",
            LastName = "Administrator"
        };

        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(true)
            .Callback<IdentityUser, string>((u, p) => u.Id = "superAdminId");
        _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<string>(), "Admin")).ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Create(It.IsAny<Admin>())).ReturnsAsync((Admin a) => a);
        _userManagerMock.Setup(u => u.FindByIdAsync("superAdminId")).ReturnsAsync(new IdentityUser { Id = "superAdminId", UserName = "superadmin", Email = "super@example.com" });

        var result = await _service.Add(request);

        Assert.Equal("Super", result.Data.FirstName);
        Assert.Equal("Administrator", result.Data.LastName);
        Assert.Equal("superAdminId", result.Data.UserId);
    }

    [Fact]
    public async Task Add_CreatesIdentityUserWithCorrectData()
    {
        var request = new CreateAdminRequest
        {
            UserName = "testadmin",
            Email = "testadmin@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "Admin"
        };

        IdentityUser capturedUser = null!;

        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), "Password123!"))
            .Callback<IdentityUser, string>((u, p) =>
            {
                capturedUser = u;
                u.Id = "capturedId";
            })
            .ReturnsAsync(true);
        _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<string>(), "Admin")).ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Create(It.IsAny<Admin>())).ReturnsAsync((Admin a) => a);
        _userManagerMock.Setup(u => u.FindByIdAsync("capturedId")).ReturnsAsync(new IdentityUser { Id = "capturedId", UserName = "testadmin", Email = "testadmin@example.com" });

        await _service.Add(request);

        Assert.Equal("testadmin", capturedUser.UserName);
        Assert.Equal("testadmin@example.com", capturedUser.Email);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_UpdatesAdmin_WhenUserIsAdmin()
    {
        var adminId = Guid.NewGuid();
        var existing = new Admin
        {
            Id = adminId,
            FirstName = "Old",
            LastName = "Admin",
            IdentityUserId = "user123"
        };
        var identityUser = new IdentityUser { Id = "user123", UserName = "oldname", Email = "old@mail.com" };
        var request = new UpdateAdminRequest
        {
            UserName = "newname",
            Email = "new@mail.com",
            FirstName = "New",
            LastName = "Admin"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(existing);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);
        _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Update(It.IsAny<Admin>())).ReturnsAsync((Admin e) => e);

        var result = await _service.Update(adminId, request);

        Assert.Equal("New", result.Data.FirstName);
        Assert.Equal("Admin", result.Data.LastName);
        _userManagerMock.Verify(u => u.UpdateAsync(It.Is<IdentityUser>(u => u.UserName == "newname")), Times.Once);
    }

    [Fact]
    public async Task Update_UpdatesAdmin_WhenUserIsAccountOwner()
    {
        var adminId = Guid.NewGuid();
        var existing = new Admin
        {
            Id = adminId,
            FirstName = "Old",
            LastName = "Admin",
            IdentityUserId = "owner123"
        };
        var identityUser = new IdentityUser { Id = "owner123" };
        var request = new UpdateAdminRequest
        {
            FirstName = "Self",
            LastName = "Updated",
            UserName = "selfupdated",
            Email = "self@mail.com"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(existing);
        _userManagerMock.Setup(u => u.FindByIdAsync("owner123")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("owner123")).Returns(true);
        _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Update(It.IsAny<Admin>())).ReturnsAsync((Admin e) => e);

        var result = await _service.Update(adminId, request);

        Assert.Equal("Self", result.Data.FirstName);
        Assert.Equal("Updated", result.Data.LastName);
    }

    [Fact]
    public async Task Update_ThrowsForbiddenException_WhenUserNotAuthorized()
    {
        var adminId = Guid.NewGuid();
        var existing = new Admin
        {
            Id = adminId,
            FirstName = "TestName",
            LastName = "TestLastName",
            IdentityUserId = "user123"
        };
        var identityUser = new IdentityUser { Id = "user123" };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(existing);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);

        var request = new UpdateAdminRequest { FirstName = "Test" };

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.Update(adminId, request));
        _adminRepoMock.Verify(r => r.Update(It.IsAny<Admin>()), Times.Never);
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenAdminNotExists()
    {
        var adminId = Guid.NewGuid();
        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync((Admin)null!);
        var request = new UpdateAdminRequest { FirstName = "Test" };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(adminId, request));
    }

    [Fact]
    public async Task Update_ThrowsNotFoundException_WhenIdentityUserNotExists()
    {
        var adminId = Guid.NewGuid();
        var existing = new Admin
        {
            Id = adminId,
            FirstName = "TestName",
            LastName = "TestLastName",
            IdentityUserId = "user123"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(existing);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync((IdentityUser)null!);
        var request = new UpdateAdminRequest { FirstName = "Test" };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(adminId, request));
    }

    [Fact]
    public async Task Update_UpdatesIdentityUserData()
    {
        var adminId = Guid.NewGuid();
        var existing = new Admin
        {
            Id = adminId,
            FirstName = "Old",
            LastName = "Admin",
            IdentityUserId = "user123"
        };
        var identityUser = new IdentityUser { Id = "user123", UserName = "oldname", Email = "old@mail.com" };
        var request = new UpdateAdminRequest
        {
            UserName = "updatedname",
            Email = "updated@mail.com",
            FirstName = "Updated",
            LastName = "Admin"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(existing);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(identityUser);
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);
        _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Update(It.IsAny<Admin>())).ReturnsAsync((Admin e) => e);

        await _service.Update(adminId, request);

        _userManagerMock.Verify(u => u.UpdateAsync(It.Is<IdentityUser>(
            user => user.UserName == "updatedname" && user.Email == "updated@mail.com"
        )), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_DeletesAdmin_WhenUserIsAdmin()
    {
        var adminId = Guid.NewGuid();
        var admin = new Admin
        {
            Id = adminId,
            FirstName = "Jan",
            LastName = "TestLastName",
            IdentityUserId = "user123"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(new IdentityUser { Id = "user123" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);
        _userManagerMock.Setup(u => u.DeleteAsync("user123")).ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Delete(adminId)).Returns(Task.CompletedTask);

        var result = await _service.Delete(adminId);

        Assert.NotNull(result);
        Assert.Equal("Jan", result.Data.FirstName);
        _userManagerMock.Verify(u => u.DeleteAsync("user123"), Times.Once);
        _adminRepoMock.Verify(r => r.Delete(adminId), Times.Once);
    }

    [Fact]
    public async Task Delete_DeletesAdmin_WhenUserIsAccountOwner()
    {
        var adminId = Guid.NewGuid();
        var admin = new Admin
        {
            Id = adminId,
            FirstName = "Self",
            LastName = "TestLastName",
            IdentityUserId = "owner456"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("owner456")).ReturnsAsync(new IdentityUser { Id = "owner456" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("owner456")).Returns(true);
        _userManagerMock.Setup(u => u.DeleteAsync("owner456")).ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Delete(adminId)).Returns(Task.CompletedTask);

        var result = await _service.Delete(adminId);

        Assert.NotNull(result);
        Assert.Equal("Self", result.Data.FirstName);
        _adminRepoMock.Verify(r => r.Delete(adminId), Times.Once);
    }

    [Fact]
    public async Task Delete_ThrowsNotFoundException_WhenAdminNotExists()
    {
        var adminId = Guid.NewGuid();
        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync((Admin)null!);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.Delete(adminId));
        _userManagerMock.Verify(u => u.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Delete_ThrowsForbiddenException_WhenUserNotAuthorized()
    {
        var adminId = Guid.NewGuid();
        var admin = new Admin
        {
            Id = adminId,
            FirstName = "TestName",
            LastName = "TestLastName",
            IdentityUserId = "user123"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(new IdentityUser { Id = "user123" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).Returns(false);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.Delete(adminId));
        _userManagerMock.Verify(u => u.DeleteAsync(It.IsAny<string>()), Times.Never);
        _adminRepoMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Delete_DeletesIdentityUserFirst()
    {
        var adminId = Guid.NewGuid();
        var admin = new Admin
        {
            Id = adminId,
            FirstName = "Test",
            LastName = "Admin",
            IdentityUserId = "userToDelete"
        };

        var callOrder = new List<string>();

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("userToDelete")).ReturnsAsync(new IdentityUser { Id = "userToDelete" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("userToDelete")).Returns(false);
        _userManagerMock.Setup(u => u.DeleteAsync("userToDelete"))
            .Callback(() => callOrder.Add("DeleteIdentityUser"))
            .ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Delete(adminId))
            .Callback(() => callOrder.Add("DeleteAdmin"))
            .Returns(Task.CompletedTask);

        await _service.Delete(adminId);

        Assert.Equal(2, callOrder.Count);
        Assert.Equal("DeleteIdentityUser", callOrder[0]);
        Assert.Equal("DeleteAdmin", callOrder[1]);
    }

    [Fact]
    public async Task Delete_ReturnsDeletedAdminData()
    {
        var adminId = Guid.NewGuid();
        var admin = new Admin
        {
            Id = adminId,
            FirstName = "Deleted",
            LastName = "Admin",
            IdentityUserId = "deletedUser"
        };

        _adminRepoMock.Setup(r => r.GetById(adminId)).ReturnsAsync(admin);
        _userManagerMock.Setup(u => u.FindByIdAsync("deletedUser")).ReturnsAsync(new IdentityUser { Id = "deletedUser" });
        _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
        _userManagerMock.Setup(u => u.IsUserAccountOwner("deletedUser")).Returns(false);
        _userManagerMock.Setup(u => u.DeleteAsync("deletedUser")).ReturnsAsync(true);
        _adminRepoMock.Setup(r => r.Delete(adminId)).Returns(Task.CompletedTask);

        var result = await _service.Delete(adminId);

        Assert.Equal(adminId, result.Data.Id);
        Assert.Equal("Deleted", result.Data.FirstName);
        Assert.Equal("Admin", result.Data.LastName);
        Assert.Equal("deletedUser", result.Data.UserId);
    }

    #endregion

    #region GetCurrent Tests

    [Fact]
    public async Task GetCurrent_ReturnsAdminProfile_ForCurrentUser()
    {
        var userId = "user123";
        var admin = new Admin
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Kowalski",
            IdentityUserId = userId
        };

        _userServiceMock
            .Setup(s => s.GetCurrentUser())
            .ReturnsAsync(new UserResponse { Data = new UserView { Id = userId } });

        _adminRepoMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync(admin);

        _userManagerMock
            .Setup(u => u.FindByIdAsync(userId))
            .ReturnsAsync(new IdentityUser { Id = userId, UserName = "jan", Email = "jan@mail.com" });

        var result = await _service.GetCurrent();

        Assert.NotNull(result);
        Assert.Equal(admin.Id, result.Data.Id);
        Assert.Equal("Jan", result.Data.FirstName);
        Assert.Equal("Kowalski", result.Data.LastName);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Equal("jan", result.Data.UserName);
        Assert.Equal("jan@mail.com", result.Data.Email);
    }

    [Fact]
    public async Task GetCurrent_ThrowsNotFoundException_WhenProfileDoesNotExist()
    {
        var userId = "user123";

        _userServiceMock
            .Setup(s => s.GetCurrentUser())
            .ReturnsAsync(new UserResponse { Data = new UserView { Id = userId } });

        _adminRepoMock.Setup(r => r.GetByUserId(userId)).ReturnsAsync((Admin)null!);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCurrent());
        Assert.Equal("Profil menadżera nie istnieje dla tego użytkownika.", ex.Message);
    }

    #endregion
}