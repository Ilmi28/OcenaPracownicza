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
    public class AdminTests : BaseTests<AdminWebApplicationFactory>
    {
        private readonly Guid _adminEntityId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        private readonly Guid _adminToDeleteId = Guid.Parse("ffffffff-eeee-dddd-cccc-bbbbbbbbbbbb");

        private const string AdminUserId = "admin-user-1";
        private const string DeleteUserId = "admin-user-2";

        public AdminTests(AdminWebApplicationFactory factory) : base(factory)
        {
        }

        private void LoginAsAdmin()
        {
            var user = context.Users.Find(AdminUserId);
            var jwtToken = tokenService.GenerateToken(user, new List<string> { "Admin" });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        protected override void SeedData()
        {
            var hasher = new PasswordHasher<IdentityUser>();

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                context.Roles.Add(new IdentityRole { Id = "role_admin", Name = "Admin", NormalizedName = "ADMIN" });
            }

            var users = new List<IdentityUser>
            {
                new IdentityUser
                {
                    Id = AdminUserId,
                    UserName = "TestAdmin",
                    NormalizedUserName = "TESTADMIN",
                    Email = "admin@test.com",
                    NormalizedEmail = "ADMIN@TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new IdentityUser
                {
                    Id = DeleteUserId,
                    UserName = "AdminToDelete",
                    NormalizedUserName = "ADMINTODELETE",
                    Email = "delete@admin.com",
                    NormalizedEmail = "DELETE@ADMIN.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };

            foreach (var user in users)
            {
                user.PasswordHash = hasher.HashPassword(user, "Pass123!");
            }

            var adminEntities = new List<Admin>
            {
                new Admin
                {
                    Id = _adminEntityId,
                    FirstName = "System",
                    LastName = "Administrator",
                    IdentityUserId = AdminUserId
                },
                new Admin
                {
                    Id = _adminToDeleteId,
                    FirstName = "To",
                    LastName = "Delete",
                    IdentityUserId = DeleteUserId
                }
            };

            context.Users.AddRange(users);
            context.Admins.AddRange(adminEntities);

            context.UserRoles.AddRange(
                new IdentityUserRole<string> { RoleId = "role_admin", UserId = AdminUserId },
                new IdentityUserRole<string> { RoleId = "role_admin", UserId = DeleteUserId }
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task CreateAdmin_ReturnsCreated_WhenAuthorized()
        {
            LoginAsAdmin();

            var request = new CreateAdminRequest
            {
                UserName = "NewIntegrationAdmin",
                Email = "new@admin.com",
                Password = "Password123!",
                FirstName = "New",
                LastName = "Admin"
            };

            var response = await client.PostAsJsonAsync("/api/admin", request);

            response.EnsureSuccessStatusCode();
            var createdAdmin = await response.Content.ReadFromJsonAsync<AdminResponse>();

            Assert.NotNull(createdAdmin.Data);
            Assert.Equal("New", createdAdmin.Data.FirstName);
            Assert.Equal("Admin", createdAdmin.Data.LastName);
            Assert.NotNull(createdAdmin.Data.UserId);
        }

        [Fact]
        public async Task GetAll_ReturnsList_WhenAuthorized()
        {
            LoginAsAdmin();

            var response = await client.GetAsync("/api/admin");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var admins = await response.Content.ReadFromJsonAsync<AdminListResponse>();

            Assert.NotNull(admins);
            Assert.NotNull(admins.Data);
            Assert.Contains(admins.Data, a => a.FirstName == "System" && a.LastName == "Administrator");
        }

        [Fact]
        public async Task GetById_ReturnsAdmin_WhenAuthorized()
        {
            LoginAsAdmin();

            var response = await client.GetAsync($"/api/admin/{_adminEntityId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<AdminResponse>();

            Assert.NotNull(result);
            Assert.Equal("System", result.Data.FirstName);
            Assert.Equal("Administrator", result.Data.LastName);
        }

        [Fact]
        public async Task Delete_RemovesUser_WhenAuthorized()
        {
            LoginAsAdmin();

            var response = await client.DeleteAsync($"/api/admin/{_adminToDeleteId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var exists = await context.Admins.AnyAsync(a => a.Id == _adminToDeleteId);
            Assert.False(exists);

            var userExists = await context.Users.AnyAsync(u => u.Id == DeleteUserId);
            Assert.False(userExists);
        }

        [Fact]
        public async Task CreateAdmin_ReturnsUnauthorized_WhenNoToken()
        {
            var request = new CreateAdminRequest
            {
                UserName = "Hacker",
                Email = "h@h.com",
                Password = "Pass",
                FirstName = "Hacker",
                LastName = "Test"
            };

            var response = await client.PostAsJsonAsync("/api/admin", request);

            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }
    }
}