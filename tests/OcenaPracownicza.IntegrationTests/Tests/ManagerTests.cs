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
    public class ManagerTests : BaseTests<ManagerWebApplicationFactory>
    {
        private readonly Guid _man1Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private readonly Guid _man2Id = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private readonly Guid _man3Id = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private readonly Guid _man4Id = Guid.Parse("00000000-0000-0000-0000-000000000004");
        private readonly Guid _man5Id = Guid.Parse("00000000-0000-0000-0000-000000000005");
        private readonly Guid _man6Id = Guid.Parse("00000000-0000-0000-0000-000000000006");
        public ManagerTests(ManagerWebApplicationFactory factory) : base(factory)
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

            var managers = new List<Manager>
            {
                new Manager {Id = _man1Id, FirstName = "Jan", LastName = "Kowalski", AchievementsSummary = "Good", IdentityUserId = "1" },
                new Manager {Id = _man2Id, FirstName = "Anna", LastName = "Nowak", AchievementsSummary = "Solid", IdentityUserId = "2"},
                new Manager {Id = _man3Id, FirstName = "Piotr", LastName = "Zielinski", AchievementsSummary = "Excellent", IdentityUserId = "3"},
                new Manager {Id = _man4Id, FirstName = "To", LastName = "Delete", AchievementsSummary = "Remove me", IdentityUserId = "4"},
                new Manager {Id = _man5Id, FirstName = "A", LastName = "Alpha", AchievementsSummary = "A", IdentityUserId = "5"},
                new Manager {Id = _man6Id, FirstName = "B", LastName = "Beta", AchievementsSummary = "B", IdentityUserId = "6"}
            };

            context.Roles.AddRange(roles);
            context.Users.AddRange(users);
            context.Managers.AddRange(managers);

            context.UserRoles.AddRange(
                new IdentityUserRole<string> { RoleId = "role_manager", UserId = "1" },
                new IdentityUserRole<string> { RoleId = "role_manager", UserId = "2" },
                new IdentityUserRole<string> { RoleId = "role_manager", UserId = "3" },
                new IdentityUserRole<string> { RoleId = "role_manager", UserId = "4" },
                new IdentityUserRole<string> { RoleId = "role_admin", UserId = "7" }
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task GetById_ReturnsManager()
        {
            LoginAsAdmin();
            var response = await client.GetAsync($"/api/manager/{_man1Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<BaseResponse<ManagerView>>();
            Assert.NotNull(result);
            Assert.Equal("Jan", result!.Data.FirstName);
            Assert.Equal("Kowalski", result.Data.LastName);
        }

        [Fact]
        public async Task GetAll_ReturnsManagers()
        {
            LoginAsAdmin();
            var response = await client.GetAsync("/api/manager");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var managers = await response.Content.ReadFromJsonAsync<BaseResponse<List<ManagerView>>>();
            var data = managers!.Data;

            Assert.NotNull(data);
            Assert.Contains(data, e => e.FirstName == "Jan");
            Assert.Contains(data, e => e.FirstName == "Anna");
        }

        [Fact]
        public async Task Post_AddsManager()
        {
            LoginAsAdmin();
            var request = new CreateManagerRequest
            {
                UserName = "newmanager",
                Email = "newmanager@example.com",
                Password = "Pass123!",
                FirstName = "Test",
                LastName = "Manager",
                AchievementsSummary = "Summary"
            };

            var response = await client.PostAsJsonAsync("/api/manager", request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var added = await context.Managers.FirstOrDefaultAsync(e => e.FirstName == "Test" && e.LastName == "Manager");
            Assert.NotNull(added);
            Assert.Equal("Summary", added!.AchievementsSummary);

            Assert.NotEqual(Guid.Empty, added.Id);

            var identityUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "newmanager@example.com");
            Assert.NotNull(identityUser);
        }

        [Fact]
        public async Task Put_UpdatesManager()
        {
            LoginAsAdmin();
            var updateRequest = new UpdateManagerRequest
            {
                UserName = "updated_user",
                Email = "updated@example.com",
                FirstName = "UpdatedFirst",
                LastName = "UpdatedLast",
                AchievementsSummary = "Updated summary"
            };

            var response = await client.PutAsJsonAsync($"/api/manager/{_man2Id}", updateRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedManager = await context.Managers.AsNoTracking().FirstOrDefaultAsync(e => e.Id == _man2Id);
            Assert.Equal("UpdatedFirst", updatedManager!.FirstName);

            var updatedUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == "2");
            Assert.Equal("updated_user", updatedUser!.UserName);
            Assert.Equal("updated@example.com", updatedUser.Email);
        }
        
        [Fact]
        public async Task Put_NonExistingManager_ReturnsNotFound()
        {
            LoginAsAdmin();
            var updateRequest = new UpdateManagerRequest
            {
                UserName = "x",
                Email = "x@example.com",
                FirstName = "X",
                LastName = "X",
                AchievementsSummary = "X"
            };
            var response = await client.PutAsJsonAsync($"/api/manager/{Guid.NewGuid()}", updateRequest);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task Delete_RemovesManager()
        {
            LoginAsAdmin();
            var response = await client.DeleteAsync($"/api/manager/{_man4Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var exists = await context.Managers.AnyAsync(e => e.Id == _man4Id);
            Assert.False(exists);

            var userExists = await context.Users.AnyAsync(u => u.Id == "4");
            Assert.False(userExists);
        }

    
    }
}
