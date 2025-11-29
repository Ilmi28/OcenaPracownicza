using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;
using OcenaPracownicza.IntegrationTests.WebApplicationFactories;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OcenaPracownicza.IntegrationTests.Tests
{
    public class ExampleTests : BaseTests<ExampleWebApplicationFactory>
    {
        public ExampleTests(ExampleWebApplicationFactory factory) : base(factory) { }

        protected override void SeedData()
        {
            context.ExampleEntities.AddRange(
                new ExampleEntity { Id = 1, Name = "Entity1", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = 2, Name = "Old Name", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = 3, Name = "To Delete", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = 4, Name = "A", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = 5, Name = "B", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = 6, Name = "First", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = 7, Name = "Second", Description = "Desc", SomeDetail = "Detail" }
            );
        }

        [Fact]
        public async Task GetById_ReturnsEntity()
        {
            var response = await client.GetAsync("/example/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<BaseResponse<ExampleView>>();
            Assert.NotNull(result);
            Assert.Equal("Entity1", result!.Data.Name);
            Assert.Equal("Desc", result.Data.Description);
            Assert.Equal("Detail", result.Data.SomeDetail);
        }

        [Fact]
        public async Task GetAll_ReturnsEntities()
        {
            var response = await client.GetAsync("/example");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var payload = await response.Content.ReadFromJsonAsync<BaseResponse<List<ExampleView>>>();
            var data = payload!.Data;

            Assert.NotNull(data);
            Assert.Equal("Entity1", data[0].Name);
            Assert.Equal("Old Name", data[1].Name);
            Assert.Equal("To Delete", data[2].Name);
            Assert.Equal("A", data[3].Name);
            Assert.Equal("B", data[4].Name);
            Assert.Equal("First", data[5].Name);
            Assert.Equal("Second", data[6].Name);
        }

        [Fact]
        public async Task Post_AddsEntity()
        {
            var request = new ExampleRequest
            {
                Name = "Test Name",
                Description = "Test Desc",
                SomeDetail = "Test Detail"
            };

            var response = await client.PostAsJsonAsync("/example", request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var added = await context.ExampleEntities.FirstOrDefaultAsync(e => e.Name == "Test Name");
            Assert.NotNull(added);
            Assert.Equal("Test Desc", added!.Description);
            Assert.Equal("Test Detail", added.SomeDetail);
        }

        [Fact]
        public async Task Put_UpdatesEntity()
        {
            var updateRequest = new ExampleRequest
            {
                Name = "New Name",
                Description = "New Desc",
                SomeDetail = "New Detail"
            };

            var response = await client.PutAsJsonAsync("/example/2", updateRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updated = await context.ExampleEntities.FindAsync(2);
            await context.Entry(updated!).ReloadAsync();
            Assert.NotNull(updated);
            Assert.Equal("New Name", updated!.Name);
            Assert.Equal("New Desc", updated.Description);
            Assert.Equal("New Detail", updated.SomeDetail);
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

            var response = await client.PutAsJsonAsync("/example/9999", updateRequest);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.False(context.ExampleEntities.Any(e => e.Name == "NonExist"));
        }

        [Fact]
        public async Task Delete_RemovesEntity()
        {
            var response = await client.DeleteAsync("/example/3");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var exists = await context.ExampleEntities.AnyAsync(e => e.Id == 3);
            Assert.False(exists);

            var count = await context.ExampleEntities.CountAsync();
            Assert.Equal(6, count);
        }

        [Fact]
        public async Task Delete_NonExistingEntity_ReturnsNotFound()
        {
            var response = await client.DeleteAsync("/example/9999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
