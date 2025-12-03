using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using System.Linq;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.IntegrationTests.WebApplicationFactories;

namespace OcenaPracownicza.IntegrationTests.Tests
{
    public class AdminTests : BaseTests<AdminWebApplicationFactory>
    {
        private const string AdminUserId = "admin-guid-123";

        public AdminTests(AdminWebApplicationFactory factory) : base(factory)
        {
        }

        protected override void SeedData()
        {
            var hasher = new PasswordHasher<IdentityUser>();
            
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                context.Roles.Add(new IdentityRole { Id = "role_admin", Name = "Admin", NormalizedName = "ADMIN" });
            }
            
            if (!context.Users.Any(u => u.Id == AdminUserId))
            {
                var adminUser = new IdentityUser
                {
                    Id = AdminUserId,
                    UserName = "TestAdmin",
                    NormalizedUserName = "TESTADMIN",
                    Email = "admin@test.com",
                    NormalizedEmail = "ADMIN@TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                adminUser.PasswordHash = hasher.HashPassword(adminUser, "Pass123!");
                
                context.Users.Add(adminUser);
            }
            
            if (!context.UserRoles.Any(ur => ur.UserId == AdminUserId && ur.RoleId == "role_admin"))
            {
                context.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = AdminUserId,
                    RoleId = "role_admin"
                });
            }
        }

        private void LoginAsAdmin()
        {
            var user = context.Users.Find(AdminUserId);
            
            var jwtToken = tokenService.GenerateToken(user, new List<string> { "Admin" });
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        [Fact]
        public async Task CreateAdmin_ReturnsCreated_WhenAuthorized()
        {
            LoginAsAdmin();
            
            var request = new CreateAdminRequest
            {
                UserName = "NewIntegrationAdmin",
                Email = "new@admin.com",
                Password = "Password123!"
            };

            var response = await client.PostAsJsonAsync("/api/admin", request);

            response.EnsureSuccessStatusCode(); 
            var createdAdmin = await response.Content.ReadFromJsonAsync<AdminResponse>();
            Assert.Equal("NewIntegrationAdmin", createdAdmin.UserName);
        }

        [Fact]
        public async Task GetAll_ReturnsList_WhenAuthorized()
        {
            LoginAsAdmin();
            
            var response = await client.GetAsync("/api/admin");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var admins = await response.Content.ReadFromJsonAsync<List<AdminResponse>>();
            Assert.NotNull(admins);
            Assert.Contains(admins, a => a.UserName == "TestAdmin");
        }
        
        [Fact]
        public async Task Delete_RemovesUser_WhenAuthorized()
        {
            LoginAsAdmin();
            
            var createRes = await client.PostAsJsonAsync("/api/admin", 
                new CreateAdminRequest { UserName = "ToDelete", Email = "del@test.com", Password = "Password123!" });
            
            createRes.EnsureSuccessStatusCode(); 
            var userToDelete = await createRes.Content.ReadFromJsonAsync<AdminResponse>();
            
            var deleteResponse = await client.DeleteAsync($"/api/admin/{userToDelete.Id}");
            
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            
            var getResponse = await client.GetAsync($"/api/admin/{userToDelete.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task CreateAdmin_ReturnsUnauthorized_WhenNoToken()
        {
            var request = new CreateAdminRequest { UserName = "Hacker", Email = "h@h.com", Password = "Pass" };

            var response = await client.PostAsJsonAsync("/api/admin", request);

            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden);
        }
    }
}