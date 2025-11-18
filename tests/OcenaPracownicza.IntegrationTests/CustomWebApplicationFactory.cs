using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OcenaPracownicza.API.Data;
using System.Linq;

namespace OcenaPracownicza.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptors = services
                    .Where(d => d.ServiceType == typeof(ApplicationDbContext) ||
                                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>))
                    .ToList();

                foreach (var d in dbContextDescriptors)
                    services.Remove(d);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        }

    }
}
