using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;
using OcenaPracownicza.IntegrationTests.WebApplicationFactories;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace OcenaPracownicza.IntegrationTests.Tests
{
    public class EmployeeTests : BaseTests<EmployeeWebApplicationFactory>
    {
        private readonly Guid _emp1Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private readonly Guid _emp2Id = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private readonly Guid _emp3Id = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private readonly Guid _emp4Id = Guid.Parse("00000000-0000-0000-0000-000000000004");
        private readonly Guid _emp5Id = Guid.Parse("00000000-0000-0000-0000-000000000005");
        private readonly Guid _emp6Id = Guid.Parse("00000000-0000-0000-0000-000000000006");

        public EmployeeTests(EmployeeWebApplicationFactory factory) : base(factory)
        {
        }

        private void LoginAsAdmin()
        {
            var user = context.Users.Find("7");
            var jwtToken = tokenService.GenerateToken(user, new List<string> { "Admin" });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        protected override void SeedData()
        {
            var hasher = new PasswordHasher<IdentityUser>();

            var roles = new List<IdentityRole>
            {
                new IdentityRole { Id = "role_admin", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "role_manager", Name = "Manager", NormalizedName = "MANAGER" },
                new IdentityRole { Id = "role_employee", Name = "Employee", NormalizedName = "EMPLOYEE" }
            };

            var users = new List<IdentityUser>
            {
                new IdentityUser
                {
                    Id = "1",
                    UserName = "jan_kowalski",
                    NormalizedUserName = "JAN_KOWALSKI",
                    Email = "jan@test.com",
                    NormalizedEmail = "JAN@TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new IdentityUser
                {
                    Id = "2",
                    UserName = "anna_nowak",
                    NormalizedUserName = "ANNA_NOWAK",
                    Email = "anna@test.com",
                    NormalizedEmail = "ANNA@TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new IdentityUser
                {
                    Id = "3",
                    UserName = "piotr_zielinski",
                    NormalizedUserName = "PIOTR_ZIELINSKI",
                    Email = "piotr@test.com",
                    NormalizedEmail = "PIOTR@TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new IdentityUser
                {
                    Id = "4",
                    UserName = "to_delete",
                    NormalizedUserName = "TO_DELETE",
                    Email = "delete@test.com",
                    NormalizedEmail = "DELETE@TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new IdentityUser
                {
                    Id = "5",
                    UserName = "alpha",
                    NormalizedUserName = "ALPHA",
                    Email = "alpha@test.com",
                    NormalizedEmail = "ALPHA@TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new IdentityUser
                {
                    Id = "6",
                    UserName = "beta",
                    NormalizedUserName = "BETA",
                    Email = "beta@test.com",
                    NormalizedEmail = "BETA@TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new IdentityUser
                {
                    Id = "7",
                    UserName = "admin",
                    Email = "admin@test.com",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };

            foreach (var user in users)
            {
                user.PasswordHash = hasher.HashPassword(user, "Password123!");
            }

            var employees = new List<Employee>
            {
                new Employee { Id = _emp1Id, FirstName = "Jan", LastName = "Kowalski", Position = "Dev", Period = "2024", FinalScore = "8", AchievementsSummary = "Good", IdentityUserId = "1" },
                new Employee { Id = _emp2Id, FirstName = "Anna", LastName = "Nowak", Position = "QA", Period = "2024", FinalScore = "7", AchievementsSummary = "Solid", IdentityUserId = "2" },
                new Employee { Id = _emp3Id, FirstName = "Piotr", LastName = "Zielinski", Position = "PM", Period = "2024", FinalScore = "9", AchievementsSummary = "Excellent", IdentityUserId = "3" },
                new Employee { Id = _emp4Id, FirstName = "To", LastName = "Delete", Position = "Temp", Period = "2024", FinalScore = "5", AchievementsSummary = "Remove me", IdentityUserId = "4" },
                new Employee { Id = _emp5Id, FirstName = "A", LastName = "Alpha", Position = "X", Period = "2024", FinalScore = "6", AchievementsSummary = "A", IdentityUserId = "5" },
                new Employee { Id = _emp6Id, FirstName = "B", LastName = "Beta", Position = "Y", Period = "2024", FinalScore = "6", AchievementsSummary = "B", IdentityUserId = "6" }
            };

            context.Roles.AddRange(roles);
            context.Users.AddRange(users);
            context.Employees.AddRange(employees);

            context.UserRoles.AddRange(
                new IdentityUserRole<string> { RoleId = "role_employee", UserId = "1" },
                new IdentityUserRole<string> { RoleId = "role_employee", UserId = "2" },
                new IdentityUserRole<string> { RoleId = "role_employee", UserId = "3" },
                new IdentityUserRole<string> { RoleId = "role_employee", UserId = "4" },
                new IdentityUserRole<string> { RoleId = "role_admin", UserId = "7" }
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task GetById_ReturnsEmployee()
        {
            LoginAsAdmin();
            var response = await client.GetAsync($"/api/employee/{_emp1Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<BaseResponse<EmployeeView>>();
            Assert.NotNull(result);
            Assert.Equal("Jan", result!.Data.FirstName);
            Assert.Equal("Kowalski", result.Data.LastName);
        }

        [Fact]
        public async Task GetAll_ReturnsEmployees()
        {
            LoginAsAdmin();
            var response = await client.GetAsync("/api/employee");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var employees = await response.Content.ReadFromJsonAsync<BaseResponse<List<EmployeeView>>>();
            var data = employees!.Data;

            Assert.NotNull(data);
            Assert.Contains(data, e => e.FirstName == "Jan");
            Assert.Contains(data, e => e.FirstName == "Anna");
        }

        [Fact]
        public async Task Post_AddsEmployee()
        {
            LoginAsAdmin();
            var request = new CreateEmployeeRequest
            {
                UserName = "newuser_unique",
                Email = "new_unique@example.com",
                Password = "Password123!",
                FirstName = "New",
                LastName = "Employee",
                Position = "DevOps",
                Period = "2025",
                FinalScore = "10",
                AchievementsSummary = "Created in test"
            };

            var response = await client.PostAsJsonAsync("/api/employee", request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var added = await context.Employees.FirstOrDefaultAsync(e => e.FirstName == "New" && e.LastName == "Employee");
            Assert.NotNull(added);
            Assert.Equal("DevOps", added!.Position);
            
            Assert.NotEqual(Guid.Empty, added.Id);

            var identityUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "new_unique@example.com");
            Assert.NotNull(identityUser);
        }

        [Fact]
        public async Task Put_UpdatesEmployee()
        {
            LoginAsAdmin();
            var updateRequest = new UpdateEmployeeRequest
            {
                UserName = "updatedUser",
                Email = "updated@example.com",
                FirstName = "AnnaUpdated",
                LastName = "Xyz",
                Position = "NewPos",
                Period = "2030",
                FinalScore = "15",
                AchievementsSummary = "Updated summary"
            };

            var response = await client.PutAsJsonAsync($"/api/employee/{_emp2Id}", updateRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedEmployee = await context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == _emp2Id);
            Assert.Equal("AnnaUpdated", updatedEmployee!.FirstName);

            var updatedUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == "2");
            Assert.Equal("updatedUser", updatedUser!.UserName);
            Assert.Equal("updated@example.com", updatedUser.Email);
        }

        [Fact]
        public async Task Put_NonExistingEmployee_ReturnsNotFound()
        {
            LoginAsAdmin();
            var updateRequest = new UpdateEmployeeRequest
            {
                UserName = "ghost",
                Email = "ghost@example.com",
                FirstName = "DoesNot",
                LastName = "Exist",
                Position = "None",
                Period = "2025",
                FinalScore = "0",
                AchievementsSummary = "N/A"
            };

            var response = await client.PutAsJsonAsync($"/api/employee/{Guid.NewGuid()}", updateRequest);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_RemovesEmployee()
        {
            LoginAsAdmin();
            var response = await client.DeleteAsync($"/api/employee/{_emp4Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var exists = await context.Employees.AnyAsync(e => e.Id == _emp4Id);
            Assert.False(exists);

            var userExists = await context.Users.AnyAsync(u => u.Id == "4");
            Assert.False(userExists);
        }
    }
}