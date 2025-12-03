using Xunit;
using Moq;
using OcenaPracownicza.API.Services;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OcenaPracownicza.UnitTests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IUserManager> _userManagerMock;
        private readonly Mock<IEmployeeRepository> _employeeRepoMock;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _userManagerMock = new Mock<IUserManager>();
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _service = new EmployeeService(_userManagerMock.Object, _employeeRepoMock.Object);
        }

        [Fact]
        public async Task GetById_ReturnsEmployee_WhenUserIsAdmin()
        {
            var employee = new Employee 
            { 
                Id = 1, 
                FirstName = "Jan", 
                LastName = "Kowalski",
                Position = "Developer",
                Period = "Q1 2024",
                FinalScore = "85.0",
                AchievementsSummary = "Good performance",
                IdentityUserId = "user123"
            };
            
            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(employee);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
            _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).ReturnsAsync(false);

            var result = await _service.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("Jan", result.Data.FirstName);
            Assert.Equal("Kowalski", result.Data.LastName);
        }

        [Fact]
        public async Task GetById_ThrowsForbiddenException_WhenUserNotAuthorized()
        {
            var employee = new Employee 
            { 
                FirstName = "John",
                LastName = "Smith",
                Position = "Teacher",
                Id = 1,
                Period = "Q1 2024",
                FinalScore = "80.0",
                AchievementsSummary = "Average",
                IdentityUserId = "user123"
            };
            
            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(employee);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(false);
            _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).ReturnsAsync(false);

            await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetById(1));
        }

        [Fact]
        public async Task GetById_ThrowsNotFoundException_WhenEmployeeNotExists()
        {
            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync((Employee)null!);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetById(1));
        }

        [Fact]
        public async Task GetAll_ReturnsAllEmployees_WhenUserIsAdmin()
        {
            var employees = new List<Employee>
            {
                new() { IdentityUserId = "user123",Id = 1, FirstName = "Jan", LastName = "Kowalski", Position = "Developer", Period = "Q1", FinalScore = "85", AchievementsSummary = "Good" },
                new() { IdentityUserId = "user1234",Id = 2, FirstName = "Anna", LastName = "Nowak", Position = "Manager", Period = "Q1", FinalScore = "90", AchievementsSummary = "Excellent" }
            };

            _employeeRepoMock.Setup(r => r.GetAll()).ReturnsAsync(employees);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);

            var result = await _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count);
            Assert.Contains(result.Data, e => e.FirstName == "Jan");
            Assert.Contains(result.Data, e => e.FirstName == "Anna");
        }

        [Fact]
        public async Task GetAll_ReturnsAllEmployees_WhenUserIsManager()
        {
            var employees = new List<Employee>
            {
                new() { IdentityUserId = "user123",Position="Teacher",Id = 1, FirstName = "Piotr", LastName = "Zając", Period = "Q2", FinalScore = "87", AchievementsSummary = "Very good" }
            };

            _employeeRepoMock.Setup(r => r.GetAll()).ReturnsAsync(employees);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(true);

            var result = await _service.GetAll();

            Assert.NotNull(result);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetAll_ThrowsForbiddenException_WhenUserNotAuthorized()
        {
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(false);

            await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetAll());
        }

        [Fact]
        public async Task Add_CreatesEmployee_WhenUserIsAdmin()
        {
            var request = new CreateEmployeeRequest
            {
                UserName = "jkowalski",
                Email = "jan@example.com",
                Password = "Password123!",
                FirstName = "Jan",
                LastName = "Kowalski",
                Position = "Developer"
            };

            var identityUser = new IdentityUser { Id = "newUserId", UserName = "jkowalski" };
            var employee = new Employee 
            { 
                Position = "Teacher",
                Id = 1, 
                FirstName = "Jan", 
                LastName = "Kowalski",
                Period = "Q1 2024",
                FinalScore = "85.0",
                AchievementsSummary = "Good performance",
                IdentityUserId = "newUserId"
            };

            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), "Password123!")).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.AddToRoleAsync("newUserId", "Employee")).ReturnsAsync(true);
            _employeeRepoMock.Setup(r => r.Create(It.IsAny<Employee>())).ReturnsAsync(employee);

            var result = await _service.Add(request);

            Assert.NotNull(result);
            Assert.Equal("Jan", result.Data.FirstName);
            Assert.Equal("Kowalski", result.Data.LastName);
        }

        [Fact]
        public async Task Add_CreatesEmployee_WhenUserIsManager()
        {
            var request = new CreateEmployeeRequest
            {
                UserName = "anowak",
                Email = "anna@example.com",
                Password = "Password123!",
                FirstName = "Anna",
                LastName = "Nowak"
            };

            var employee = new Employee 
            { 
                Position = "Teacher",
                IdentityUserId = "user123",
                Id = 1, 
                FirstName = "Anna", 
                LastName = "Nowak",
                Period = "Q2 2024",
                FinalScore = "88.0",
                AchievementsSummary = "Great work"
            };

            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(true);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<string>(), "Employee")).ReturnsAsync(true);
            _employeeRepoMock.Setup(r => r.Create(It.IsAny<Employee>())).ReturnsAsync(employee);

            var result = await _service.Add(request);

            Assert.NotNull(result);
            Assert.Equal("Anna", result.Data.FirstName);
        }

        [Fact]
        public async Task Add_ThrowsForbiddenException_WhenUserNotAuthorized()
        {
            var request = new CreateEmployeeRequest
            {
                UserName = "test",
                Email = "test@example.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(false);

            await Assert.ThrowsAsync<ForbiddenException>(() => _service.Add(request));
        }

        [Fact]
        public async Task Add_ThrowsException_WhenUserCreationFails()
        {
            var request = new CreateEmployeeRequest
            {
                UserName = "test",
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(false);

            await Assert.ThrowsAsync<Exception>(() => _service.Add(request));
        }

        [Fact]
        public async Task Add_MapsAllFieldsCorrectly()
        {
            var request = new CreateEmployeeRequest
            {
                UserName = "pzajac",
                Email = "piotr@example.com",
                Password = "Password123!",
                FirstName = "Piotr",
                LastName = "Zając",
                Position = "Senior Developer",
                Period = "Q1 2024",
                FinalScore = "95.5",
                AchievementsSummary = "Excellent performance"
            };

            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<string>(), "Employee")).ReturnsAsync(true);
            _employeeRepoMock.Setup(r => r.Create(It.IsAny<Employee>())).ReturnsAsync((Employee e) => e);

            var result = await _service.Add(request);

            Assert.Equal("Piotr", result.Data.FirstName);
            Assert.Equal("Zając", result.Data.LastName);
            Assert.Equal("Senior Developer", result.Data.Position);
            Assert.Equal("Q1 2024", result.Data.Period);
            Assert.Equal("95.5", result.Data.FinalScore);
            Assert.Equal("Excellent performance", result.Data.AchievementsSummary);
        }

        [Fact]
        public async Task Update_UpdatesEmployee_WhenUserIsAdmin()
        {
            var existing = new Employee 
            { 
                Position = "Teacher",
                Period = "Q2 2024",
                FinalScore = "88.0",
                Id = 1, 
                FirstName = "Old", 
                LastName = "Name",
                IdentityUserId = "user123",
                AchievementsSummary = "Excellent performance"
            };
            var identityUser = new IdentityUser { Id = "user123", UserName = "oldname" };
            var request = new UpdateEmployeeRequest
            {
                UserName = "newname",
                Email = "new@example.com",
                FirstName = "New",
                LastName = "Name"
            };

            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(identityUser);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
            _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
            _employeeRepoMock.Setup(r => r.Update(It.IsAny<Employee>())).ReturnsAsync((Employee e) => e);

            var result = await _service.Update(1, request);

            Assert.Equal("New", result.Data.FirstName);
            Assert.Equal("Name", result.Data.LastName);
        }

        [Fact]
        public async Task Update_UpdatesEmployee_WhenUserIsManager()
        {
            var existing = new Employee 
            { 
                FirstName = "John",
                LastName = "Smith",
                Position = "Teacher",
                Id = 1,
                Period = "Q1",
                FinalScore = "75.0",
                AchievementsSummary = "Summary",
                IdentityUserId = "user123"
            };
            var identityUser = new IdentityUser { Id = "user123" };
            var request = new UpdateEmployeeRequest { FirstName = "Updated", LastName = "Employee" };

            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(identityUser);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(true);
            _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
            _employeeRepoMock.Setup(r => r.Update(It.IsAny<Employee>())).ReturnsAsync((Employee e) => e);

            var result = await _service.Update(1, request);

            Assert.Equal("Updated", result.Data.FirstName);
        }

        [Fact]
        public async Task Update_UpdatesEmployee_WhenUserIsAccountOwner()
        {
            var existing = new Employee 
            { 
                FirstName = "John",
                LastName = "Smith",
                Position = "Teacher",
                Id = 1,
                Period = "Q3",
                FinalScore = "82.0",
                AchievementsSummary = "Good",
                IdentityUserId = "user123"
            };
            var identityUser = new IdentityUser { Id = "user123" };
            var request = new UpdateEmployeeRequest { FirstName = "Self", LastName = "Updated" };

            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(identityUser);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(false);
            _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);
            _employeeRepoMock.Setup(r => r.Update(It.IsAny<Employee>())).ReturnsAsync((Employee e) => e);

            var result = await _service.Update(1, request);

            Assert.Equal("Self", result.Data.FirstName);
        }

        [Fact]
        public async Task Update_ThrowsNotFoundException_WhenEmployeeNotExists()
        {
            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync((Employee)null!);
            var request = new UpdateEmployeeRequest { FirstName = "Test" };

            await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(1, request));
        }

        [Fact]
        public async Task Update_ThrowsNotFoundException_WhenIdentityUserNotExists()
        {
            var existing = new Employee 
            { 
                FirstName = "John",
                LastName = "Smith",
                Position = "Teacher",
                Id = 1,
                Period = "Q1",
                FinalScore = "70.0",
                AchievementsSummary = "Test",
                IdentityUserId = "user123"
            };
            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync((IdentityUser)null!);
            var request = new UpdateEmployeeRequest { FirstName = "Test" };

            await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(1, request));
        }

        [Fact]
        public async Task Update_ThrowsForbiddenException_WhenUserNotAuthorized()
        {
            var existing = new Employee 
            { 
                FirstName = "John",
                LastName = "Smith",
                Position = "Teacher",
                Id = 1,
                Period = "Q2",
                FinalScore = "78.0",
                AchievementsSummary = "Average",
                IdentityUserId = "user123"
            };
            var identityUser = new IdentityUser { Id = "user123" };

            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(existing);
            _userManagerMock.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(identityUser);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(false);
            _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).ReturnsAsync(false);

            var request = new UpdateEmployeeRequest { FirstName = "Test" };

            await Assert.ThrowsAsync<ForbiddenException>(() => _service.Update(1, request));
        }

        [Fact]
        public async Task Delete_DeletesEmployee_WhenUserIsAdmin()
        {
            var employee = new Employee 
            { 
                LastName = "Smith",
                Position = "Teacher",
                Id = 1, 
                FirstName = "Jan",
                Period = "Q1",
                FinalScore = "85.0",
                AchievementsSummary = "Good",
                IdentityUserId = "user123"
            };

            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(false);
            _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).ReturnsAsync(false);
            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(employee);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(true);
            _userManagerMock.Setup(u => u.DeleteAsync("user123")).ReturnsAsync(true);
            _employeeRepoMock.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _service.Delete(1);

            Assert.NotNull(result);
            Assert.Equal("Jan", result.Data.FirstName);
        }

        [Fact]
        public async Task Delete_DeletesEmployee_WhenUserIsManager()
        {
            var employee = new Employee 
            { 
                LastName = "Smith",
                Position = "Teacher",
                Id = 1, 
                FirstName = "Anna",
                Period = "Q2",
                FinalScore = "90.0",
                AchievementsSummary = "Excellent",
                IdentityUserId = "user456"
            };
            _userManagerMock.Setup(u => u.IsUserAccountOwner("user456")).ReturnsAsync(false);
            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(employee);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(true);
            _userManagerMock.Setup(u => u.DeleteAsync("user456")).ReturnsAsync(true);
            _employeeRepoMock.Setup(r => r.Delete(1)).Returns(Task.CompletedTask);

            var result = await _service.Delete(1);

            Assert.NotNull(result);
        }
        

        [Fact]
        public async Task Delete_ThrowsNotFoundException_WhenEmployeeNotExists()
        {
            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync((Employee)null!);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.Delete(1));
        }

        [Fact]
        public async Task Delete_ThrowsForbiddenException_WhenUserNotAuthorized()
        {
            var employee = new Employee 
            { 
                FirstName = "John",
                LastName = "Smith",
                Position = "Teacher",
                Id = 1,
                Period = "Q1",
                FinalScore = "80.0",
                AchievementsSummary = "Average",
                IdentityUserId = "user123"
            };

            _employeeRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(employee);
            _userManagerMock.Setup(u => u.IsCurrentUserAdmin()).Returns(false);
            _userManagerMock.Setup(u => u.IsCurrentUserManager()).Returns(false);
            _userManagerMock.Setup(u => u.IsUserAccountOwner("user123")).ReturnsAsync(false);

            await Assert.ThrowsAsync<ForbiddenException>(() => _service.Delete(1));
        }
    }
}
