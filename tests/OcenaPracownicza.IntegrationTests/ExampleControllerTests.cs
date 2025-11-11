using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using OcenaPracownicza.API.Requests;
using Xunit;

namespace OcenaPracownicza.IntegrationTests
{
    public class ExampleControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ExampleControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/example");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/example/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Post_ShouldReturnOk_AndCreateItem()
        {
            var newItem = new ExampleRequest
            {
                Name = "Test Example",
                Description = "Element testowy",
                SomeDetail = "Szczegóły testowe"

            };

            var response = await _client.PostAsJsonAsync("/example", newItem);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Test Example");
        }

        [Fact]
        public async Task Put_ShouldReturnOk_AndUpdateItem()
        {
            var updateItem = new ExampleRequest
            {
                Name = "Zaktualizowany Example",
                Description = "Nowy opis",
                SomeDetail = "Szczegóły testowe"
            };

            var response = await _client.PutAsJsonAsync("/example/1", updateItem);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Zaktualizowany");
        }

        [Fact]
        public async Task Delete_ShouldReturnOk()
        {
            var response = await _client.DeleteAsync("/example/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
