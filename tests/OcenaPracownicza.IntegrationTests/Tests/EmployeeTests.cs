using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Enums;
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
        private readonly Guid _achievement1Id = Guid.Parse("10000000-0000-0000-0000-000000000001");
        private readonly Guid _achievement2Id = Guid.Parse("10000000-0000-0000-0000-000000000002");

        public EmployeeTests(EmployeeWebApplicationFactory factory) : base(factory)
        {
        }

        private void LoginAsAdmin()
        {
            var user = context.Users.Find("7");
            var jwtToken = tokenService.GenerateToken(user, new List<string> { "Admin" });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        private void LoginAsManager()
        {
            var user = context.Users.Find("3");
            var jwtToken = tokenService.GenerateToken(user, new List<string> { "Manager" });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        private void LoginAsEmployee()
        {
            var user = context.Users.Find("1");
            var jwtToken = tokenService.GenerateToken(user, new List<string> { "Employee" });
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
                new Employee { Id = _emp1Id, FirstName = "Jan", LastName = "Kowalski", Position = "Dev", IdentityUserId = "1" },
                new Employee { Id = _emp2Id, FirstName = "Anna", LastName = "Nowak", Position = "QA", IdentityUserId = "2" },
                new Employee { Id = _emp3Id, FirstName = "Piotr", LastName = "Zielinski", Position = "PM", IdentityUserId = "3" },
                new Employee { Id = _emp4Id, FirstName = "To", LastName = "Delete", Position = "Temp", IdentityUserId = "4" },
                new Employee { Id = _emp5Id, FirstName = "A", LastName = "Alpha", Position = "X", IdentityUserId = "5" },
                new Employee { Id = _emp6Id, FirstName = "B", LastName = "Beta", Position = "Y", IdentityUserId = "6" }
            };

            var achievements = new List<Achievement>
            {
                new Achievement
                {
                    Id = _achievement1Id,
                    Name = "Core API Improvements",
                    Description = "Critical backend improvements",
                    Date = DateTime.UtcNow.AddDays(-7),
                    Category = AchievementCategory.ProcessImprovement,
                    EmployeeId = _emp1Id,
                    Period = "2024",
                    FinalScore = "8",
                    AchievementsSummary = "Good",
                    Stage2Status = EvaluationStageStatus.PendingStage2
                },
                new Achievement
                {
                    Id = _achievement2Id,
                    Name = "QA Automation",
                    Description = "Automation for regression suite",
                    Date = DateTime.UtcNow.AddDays(-6),
                    Category = AchievementCategory.TechnicalGrowth,
                    EmployeeId = _emp2Id,
                    Period = "2024",
                    FinalScore = "7",
                    AchievementsSummary = "Solid",
                    Stage2Status = EvaluationStageStatus.Draft
                }
            };

            context.Roles.AddRange(roles);
            context.Users.AddRange(users);
            context.Employees.AddRange(employees);
            context.Achievements.AddRange(achievements);

            context.UserRoles.AddRange(
                new IdentityUserRole<string> { RoleId = "role_employee", UserId = "1" },
                new IdentityUserRole<string> { RoleId = "role_employee", UserId = "2" },
                new IdentityUserRole<string> { RoleId = "role_employee", UserId = "3" },
                new IdentityUserRole<string> { RoleId = "role_employee", UserId = "4" },
                new IdentityUserRole<string> { RoleId = "role_manager", UserId = "3" },
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
                Position = "DevOps"
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
                Position = "NewPos"
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
                Position = "None"
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

        [Fact]
        public async Task Stage2Pending_ReturnsItems_ForManager()
        {
            LoginAsManager();

            var response = await client.GetAsync("/api/evaluation/stage2/pending");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var payload = await response.Content.ReadFromJsonAsync<BaseResponse<List<Stage2ReviewItemView>>>();
            Assert.NotNull(payload);
            Assert.Contains(payload!.Data, x => x.AchievementId == _achievement1Id);
        }

        [Fact]
        public async Task Stage2Reject_RequiresComment()
        {
            LoginAsManager();

            var response = await client.PostAsJsonAsync($"/api/evaluation/stage2/{_achievement1Id}/reject", new { comment = "" });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Stage2Approve_ChangesStatus()
        {
            LoginAsManager();

            var response = await client.PostAsJsonAsync($"/api/evaluation/stage2/{_achievement1Id}/approve", new { comment = "OK" });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            context.ChangeTracker.Clear();
            var achievement = await context.Achievements.AsNoTracking().FirstAsync(a => a.Id == _achievement1Id);
            Assert.Equal(EvaluationStageStatus.Stage2Approved, achievement.Stage2Status);
        }

        [Fact]
        public async Task Stage2Endpoints_Forbidden_ForEmployeeRole()
        {
            LoginAsEmployee();

            var response = await client.GetAsync("/api/evaluation/stage2/pending");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Stage2Close_ChangesStatus_ForAdmin()
        {
            var achievement = await context.Achievements.FirstAsync(a => a.Id == _achievement1Id);
            achievement.Stage2Status = EvaluationStageStatus.Stage2Approved;
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            LoginAsAdmin();
            var response = await client.PostAsJsonAsync($"/api/evaluation/stage2/{_achievement1Id}/close", new { });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            context.ChangeTracker.Clear();
            var refreshed = await context.Achievements.AsNoTracking().FirstAsync(a => a.Id == _achievement1Id);
            Assert.Equal(EvaluationStageStatus.Closed, refreshed.Stage2Status);
        }

        [Fact]
        public async Task Stage2Archive_ChangesStatus_ForAdmin()
        {
            var achievement = await context.Achievements.FirstAsync(a => a.Id == _achievement1Id);
            achievement.Stage2Status = EvaluationStageStatus.Closed;
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            LoginAsAdmin();
            var response = await client.PostAsJsonAsync($"/api/evaluation/stage2/{_achievement1Id}/archive", new { });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            context.ChangeTracker.Clear();
            var refreshed = await context.Achievements.AsNoTracking().FirstAsync(a => a.Id == _achievement1Id);
            Assert.Equal(EvaluationStageStatus.Archived, refreshed.Stage2Status);
        }

        [Fact]
        public async Task Stage2Close_Forbidden_ForManager()
        {
            var achievement = await context.Achievements.FirstAsync(a => a.Id == _achievement1Id);
            achievement.Stage2Status = EvaluationStageStatus.Stage2Approved;
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            LoginAsManager();
            var response = await client.PostAsJsonAsync($"/api/evaluation/stage2/{_achievement1Id}/close", new { });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Stage2Archived_ReturnsArchivedItems()
        {
            var achievement = await context.Achievements.FirstAsync(a => a.Id == _achievement1Id);
            achievement.Stage2Status = EvaluationStageStatus.Archived;
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            LoginAsAdmin();
            var response = await client.GetAsync("/api/evaluation/stage2/archived");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var payload = await response.Content.ReadFromJsonAsync<BaseResponse<List<Stage2ReviewItemView>>>();
            Assert.NotNull(payload);
            Assert.Contains(payload!.Data, x => x.AchievementId == _achievement1Id);
        }

        [Fact]
        public async Task Stage2Approved_ReturnsApprovedItems_ForAdmin()
        {
            var achievement = await context.Achievements.FirstAsync(a => a.Id == _achievement1Id);
            achievement.Stage2Status = EvaluationStageStatus.Stage2Approved;
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            LoginAsAdmin();
            var response = await client.GetAsync("/api/evaluation/stage2/approved");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var payload = await response.Content.ReadFromJsonAsync<BaseResponse<List<Stage2ReviewItemView>>>();
            Assert.NotNull(payload);
            Assert.Contains(payload!.Data, x => x.AchievementId == _achievement1Id);
        }

        [Fact]
        public async Task Stage2Approved_Forbidden_ForManager()
        {
            LoginAsManager();
            var response = await client.GetAsync("/api/evaluation/stage2/approved");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
