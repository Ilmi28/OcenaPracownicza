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
        private readonly Guid _id1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private readonly Guid _id2 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private readonly Guid _id3 = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private readonly Guid _id4 = Guid.Parse("00000000-0000-0000-0000-000000000004");
        private readonly Guid _id5 = Guid.Parse("00000000-0000-0000-0000-000000000005");
        private readonly Guid _id6 = Guid.Parse("00000000-0000-0000-0000-000000000006");
        private readonly Guid _id7 = Guid.Parse("00000000-0000-0000-0000-000000000007");

        public ExampleTests(ExampleWebApplicationFactory factory) : base(factory) { }

        protected override void SeedData()
        {
            context.ExampleEntities.AddRange(
                new ExampleEntity { Id = _id1, Name = "Entity1", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = _id2, Name = "Old Name", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = _id3, Name = "To Delete", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = _id4, Name = "A", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = _id5, Name = "B", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = _id6, Name = "First", Description = "Desc", SomeDetail = "Detail" },
                new ExampleEntity { Id = _id7, Name = "Second", Description = "Desc", SomeDetail = "Detail" }
            );
        }

        [Fact]
        public async Task GetById_ReturnsEntity()
        {
            var response = await client.GetAsync($"/example/{_id1}");
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
            Assert.Contains(data, e => e.Name == "Entity1");
            Assert.Contains(data, e => e.Name == "Old Name");
            Assert.Contains(data, e => e.Name == "To Delete");
            Assert.Contains(data, e => e.Name == "A");
            Assert.Contains(data, e => e.Name == "B");
            Assert.Contains(data, e => e.Name == "First");
            Assert.Contains(data, e => e.Name == "Second");
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
            Assert.NotEqual(Guid.Empty, added.Id);
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

            var response = await client.PutAsJsonAsync($"/example/{_id2}", updateRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updated = await context.ExampleEntities.FindAsync(_id2);
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

            var response = await client.PutAsJsonAsync($"/example/{Guid.NewGuid()}", updateRequest);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.False(context.ExampleEntities.Any(e => e.Name == "NonExist"));
        }

        [Fact]
        public async Task Delete_RemovesEntity()
        {
            var response = await client.DeleteAsync($"/example/{_id3}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var exists = await context.ExampleEntities.AnyAsync(e => e.Id == _id3);
            Assert.False(exists);

            var count = await context.ExampleEntities.CountAsync();
            Assert.Equal(6, count);
        }

        [Fact]
        public async Task Delete_NonExistingEntity_ReturnsNotFound()
        {
            var response = await client.DeleteAsync($"/example/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}