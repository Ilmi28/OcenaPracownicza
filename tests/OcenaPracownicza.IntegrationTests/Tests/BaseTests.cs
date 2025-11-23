using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.IntegrationTests.WebApplicationFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcenaPracownicza.IntegrationTests.Tests
{
    public class BaseTests<TWebApplicationFactory> : IClassFixture<TWebApplicationFactory> where TWebApplicationFactory : BaseWebApplicationFactory
    {
        protected readonly HttpClient client;
        protected readonly ApplicationDbContext context;
        public BaseTests(TWebApplicationFactory factory)
        {
            client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            SeedData();
            context.SaveChanges();
        }

        protected virtual void SeedData()
        {

        }
    }
}
