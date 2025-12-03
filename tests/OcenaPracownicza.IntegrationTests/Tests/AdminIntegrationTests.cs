using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.IntegrationTests.WebApplicationFactories;

namespace OcenaPracownicza.IntegrationTests.Tests
{
    public class AdminIntegrationTests : IClassFixture<AdminWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly AdminWebApplicationFactory _factory;

        public AdminIntegrationTests(AdminWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                context.Database.EnsureCreated();
                
                if (!roleManager.RoleExistsAsync("Admin").Result)
                {
                    roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                }
                
                if (userManager.FindByIdAsync("admin-id-123").Result == null)
                {
                    var adminUser = new IdentityUser 
                    { 
                        Id = "admin-id-123",
                        UserName = "TestAdmin", 
                        Email = "admin@test.com"
                    };
                    userManager.CreateAsync(adminUser, "Pass123!").Wait();
                    userManager.AddToRoleAsync(adminUser, "Admin").Wait();
                }
            }
        }

        [Fact]
        public async Task CreateAdmin_ReturnsCreated()
        {
            var request = new CreateAdminRequest
            {
                UserName = "NewIntegrationAdmin",
                Email = "new@admin.com",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/admin", request);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_ReturnsList()
        {
            var response = await _client.GetAsync("/api/admin");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var admins = await response.Content.ReadFromJsonAsync<List<AdminResponse>>();
            Assert.NotNull(admins);
            Assert.Contains(admins, a => a.Id == "admin-id-123");
        }
    }
}