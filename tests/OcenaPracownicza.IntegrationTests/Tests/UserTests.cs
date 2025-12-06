using Microsoft.AspNetCore.Identity;
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
    public class UserTests : BaseTests<UserWebApplicationFactory>
    {
        private const string AdminUserId = "user-test-admin-id";
        private const string StandardUserId = "user-test-standard-id";
        private const string TargetUserId = "user-test-target-id";

        public UserTests(UserWebApplicationFactory factory) : base(factory)
        {
        }

        private void LoginAsAdmin()
        {
            var user = context.Users.Find(AdminUserId);
            var jwtToken = tokenService.GenerateToken(user, new List<string> { "Admin" });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        private void LoginAsStandardUser()
        {
            var user = context.Users.Find(StandardUserId);
            var jwtToken = tokenService.GenerateToken(user, new List<string> { "User" });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        private void LoginAsTargetUser()
        {
            var user = context.Users.Find(TargetUserId);
            var jwtToken = tokenService.GenerateToken(user, new List<string> { "User" });
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
                    UserName = "UserTestAdmin",
                    NormalizedUserName = "USERTESTADMIN",
                    Email = "admin@user-test.com",
                    NormalizedEmail = "ADMIN@USER-TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new IdentityUser
                {
                    Id = StandardUserId,
                    UserName = "StandardUser",
                    NormalizedUserName = "STANDARDUSER",
                    Email = "standard@user-test.com",
                    NormalizedEmail = "STANDARD@USER-TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new IdentityUser
                {
                    Id = TargetUserId,
                    UserName = "TargetUser",
                    NormalizedUserName = "TARGETUSER",
                    Email = "target@user-test.com",
                    NormalizedEmail = "TARGET@USER-TEST.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };

            foreach (var user in users)
            {
                user.PasswordHash = hasher.HashPassword(user, "Pass123!");
            }

            context.Users.AddRange(users);

            context.UserRoles.AddRange(
                new IdentityUserRole<string> { RoleId = "role_admin", UserId = AdminUserId }
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllUsers_ReturnsList_WhenAuthorizedAsAdmin()
        {
            LoginAsAdmin();

            var response = await client.GetAsync("/api/user");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<UserListResponse>();

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Contains(result.Data, u => u.Id == StandardUserId);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsForbidden_WhenAuthorizedAsStandardUser()
        {
            LoginAsStandardUser();

            var response = await client.GetAsync("/api/user");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_ReturnsUser_WhenAuthorizedAsAdmin()
        {
            LoginAsAdmin();

            var response = await client.GetAsync($"/api/user/{TargetUserId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<UserResponse>();

            Assert.NotNull(result);
            Assert.Equal(TargetUserId, result.Data.Id);
            Assert.Equal("TargetUser", result.Data.UserName);
        }

        [Fact]
        public async Task GetUserById_ReturnsForbidden_WhenAuthorizedAsStandardUser()
        {
            LoginAsStandardUser();

            var response = await client.GetAsync($"/api/user/{TargetUserId}");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            LoginAsAdmin();
            var nonExistentId = Guid.NewGuid().ToString();

            var response = await client.GetAsync($"/api/user/{nonExistentId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsCorrectData_WhenAuthorized()
        {
            LoginAsStandardUser();

            var response = await client.GetAsync("/api/user/current");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<UserResponse>();

            Assert.NotNull(result);
            Assert.Equal(StandardUserId, result.Data.Id);
            Assert.Equal("StandardUser", result.Data.UserName);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsUnauthorized_WhenNotLoggedIn()
        {
            client.DefaultRequestHeaders.Authorization = null;

            var response = await client.GetAsync("/api/user/current");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_ReturnsSuccess_WhenCurrentPasswordIsCorrect()
        {
            LoginAsTargetUser();
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "Pass123!",
                NewPassword = "NewPassword123!",
            };

            var response = await client.PostAsJsonAsync("/api/user/change-password", request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<BaseResponse>();
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ChangePassword_ReturnsError_WhenCurrentPasswordIsWrong()
        {
            LoginAsTargetUser();
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "WrongPassword!",
                NewPassword = "NewPassword123!",
            };

            var response = await client.PostAsJsonAsync("/api/user/change-password", request);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}