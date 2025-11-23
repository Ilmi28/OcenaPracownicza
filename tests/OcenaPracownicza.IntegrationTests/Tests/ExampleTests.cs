using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.IntegrationTests.WebApplicationFactories;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OcenaPracownicza.IntegrationTests.Tests
{
    public class ExampleTests : BaseTests<ExampleWebApplicationFactory>
    {
        public ExampleTests(ExampleWebApplicationFactory factory) : base(factory)
        {
        }

        protected override void SeedData()
        {
            context.ExampleEntities.AddRange(
                new ExampleEntity { Name = "Entity1", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Name = "Old Name", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Name = "To Delete", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Name = "A", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Name = "B", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Name = "First", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Name = "Second", Description = "Desc", SomeDetail = "Detail" }
            );
        }

        [Fact]
        public async Task GetById_ReturnsSingleEntity()
        {
            var entity = await context.ExampleEntities.FindAsync(1);
            var response = await client.GetAsync($"/example/{entity!.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await context.ExampleEntities.FindAsync(entity.Id);
            Assert.NotNull(result);
            Assert.Equal("Entity1", result!.Name);
        }

        [Fact]
        public async Task Post_AddsEntity_ThenGetReturnsIt()
        {
            var request = new ExampleRequest
            {
                Name = "Test Name",
                Description = "Test Desc",
                SomeDetail = "Test Detail"
            };

            var postResponse = await client.PostAsJsonAsync("/example", request);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var added = await context.ExampleEntities
                .FirstOrDefaultAsync(e => e.Name == "Test Name");

            Assert.NotNull(added);
            Assert.Equal("Test Name", added!.Name);
        }

        [Fact]
        public async Task Put_UpdatesEntity()
        {
            var entity = await context.ExampleEntities.FirstAsync(e => e.Name == "Old Name");

            var updateRequest = new ExampleRequest
            {
                Name = "New Name",
                Description = "New Desc",
                SomeDetail = "New Detail"
            };

            var putResponse = await client.PutAsJsonAsync($"/example/{entity.Id}", updateRequest);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            var updated = await context.ExampleEntities.FindAsync(entity.Id);
            await context.Entry(updated!).ReloadAsync();

            Assert.NotNull(updated);
            Assert.Equal("New Name", updated.Name);
            Assert.Equal("New Desc", updated.Description);
        }

        [Fact]
        public async Task Delete_RemovesEntity()
        {
            var entity = await context.ExampleEntities.FirstAsync(e => e.Name == "To Delete");

            var deleteResponse = await client.DeleteAsync($"/example/{entity.Id}");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            var count = context.ExampleEntities.Count();
            Assert.Equal(6, count);
        }

        [Fact]
        public async Task GetAll_ReturnsMultipleEntities()
        {
            var response = await client.GetAsync("/example");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var names = await context.ExampleEntities
                .Where(e => e.Id == 4 || e.Id == 5)
                .Select(e => e.Name)
                .ToListAsync();

            Assert.Contains("A", names);
            Assert.Contains("B", names);
        }

        [Fact]
        public async Task Put_NonExistingEntity_ReturnsNotFound()
        {
            var updateRequest = new ExampleRequest
            {
                Name = "NonExist",
                Description = "Desc",
                SomeDetail = "Detail"
            };

            var putResponse = await client.PutAsJsonAsync("/example/9999", updateRequest);
            Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
            Assert.False(context.ExampleEntities.Any(e => e.Name == "NonExist"));
        }

        [Fact]
        public async Task Delete_NonExistingEntity_ReturnsNotFound()
        {
            var deleteResponse = await client.DeleteAsync("/example/9999");
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task MultiplePosts_PreserveOrder()
        {
            var response = await client.GetAsync("/example");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var ordered = await context.ExampleEntities
                .Where(e => e.Id == 6 || e.Id == 7)
                .OrderBy(e => e.Id)
                .Select(e => e.Name)
                .ToListAsync();

            Assert.Equal("First", ordered[0]);
            Assert.Equal("Second", ordered[1]);
        }
    }
}
