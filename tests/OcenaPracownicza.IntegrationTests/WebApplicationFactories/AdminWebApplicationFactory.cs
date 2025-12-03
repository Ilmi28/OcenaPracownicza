using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OcenaPracownicza.IntegrationTests.WebApplicationFactories
{
    public class AdminWebApplicationFactory : BaseWebApplicationFactory
    {
        public AdminWebApplicationFactory() : base("AdminIntegrationDb")
        {
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
            });
        }
        
        public class FakePolicyEvaluator : IPolicyEvaluator
        {
            public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
            {
                var principal = new ClaimsPrincipal();
                
                principal.AddIdentity(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "admin-id-123"),
                    new Claim(ClaimTypes.Name, "TestAdmin"),
                    new Claim(ClaimTypes.Role, "Admin")
                }, "FakeScheme"));

                return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal,
                    new AuthenticationProperties(), "FakeScheme")));
            }

            public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
                AuthenticateResult authenticationResult, HttpContext context, object resource)
            {
                return await Task.FromResult(PolicyAuthorizationResult.Success());
            }
        }
    }
}