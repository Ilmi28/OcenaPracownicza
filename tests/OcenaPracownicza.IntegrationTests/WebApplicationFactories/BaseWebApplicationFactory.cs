using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OcenaPracownicza.API;
using OcenaPracownicza.API.Data;

namespace OcenaPracownicza.IntegrationTests.WebApplicationFactories
{
    public class BaseWebApplicationFactory(string dbName) : WebApplicationFactory<Program>
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                Environment.SetEnvironmentVariable("GOOGLE_CLIENT_ID", "test");
                Environment.SetEnvironmentVariable("GOOGLE_CLIENT_SECRET", "test");

                services.RemoveAll<DbContextOptions>();
                services.RemoveAll<ApplicationDbContext>();

                services.AddDbContext<TestDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });

                services.AddScoped<ApplicationDbContext, TestDbContext>();
            });
        }
    }
}
