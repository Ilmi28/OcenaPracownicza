using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using System.Net.Http.Json;
using Xunit;

namespace OcenaPracownicza.IntegrationTests
{
    public class ExampleControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;

        public ExampleControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_ID", "test-client-id");
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_SECRET", "test-client-secret");

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Post_AddsEntity_ThenGetReturnsIt()
        {
            var newRequest = new ExampleRequest
            {
                Name = "Test Name",
                Description = "Test Description",
                SomeDetail = "Test Detail"
            };

            var postResponse = await _httpClient.PostAsJsonAsync("/example", newRequest);
            postResponse.EnsureSuccessStatusCode();

            var getResponse = await _httpClient.GetAsync("/example");
            getResponse.EnsureSuccessStatusCode();

            var result = await getResponse.Content.ReadFromJsonAsync<ExampleListResponse>();

            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal("Test Name", result.Data[0].Name);
        }

        [Fact]
        public async Task GetById_ReturnsSingleEntity()
        {
            // Arrange – dodaj encję
            var newRequest = new ExampleRequest
            {
                Name = "Entity1",
                Description = "Desc1",
                SomeDetail = "Detail1"
            };
            var postResponse = await _httpClient.PostAsJsonAsync("/example", newRequest);
            postResponse.EnsureSuccessStatusCode();

            // Act – pobierz po ID (zakładamy ID=1 w InMemory)
            var getResponse = await _httpClient.GetAsync("/example/1");
            getResponse.EnsureSuccessStatusCode();

            var result = await getResponse.Content.ReadFromJsonAsync<ExampleResponse>();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Entity1", result.Data.Name);
        }

        [Fact]
        public async Task Put_UpdatesEntity()
        {
            // Arrange – dodaj encję
            var newRequest = new ExampleRequest
            {
                Name = "Old Name",
                Description = "Old Desc",
                SomeDetail = "Old Detail"
            };
            var postResponse = await _httpClient.PostAsJsonAsync("/example", newRequest);
            postResponse.EnsureSuccessStatusCode();

            // Act – zaktualizuj encję
            var updateRequest = new ExampleRequest
            {
                Name = "New Name",
                Description = "New Desc",
                SomeDetail = "New Detail"
            };
            var putResponse = await _httpClient.PutAsJsonAsync("/example/1", updateRequest);
            putResponse.EnsureSuccessStatusCode();

            var updated = await putResponse.Content.ReadFromJsonAsync<ExampleResponse>();

            // Assert
            Assert.NotNull(updated);
            Assert.Equal("New Name", updated.Data.Name);
        }

        [Fact]
        public async Task Delete_RemovesEntity()
        {
            // Arrange – dodaj encję
            var newRequest = new ExampleRequest
            {
                Name = "Delete Me",
                Description = "Desc",
                SomeDetail = "Detail"
            };
            var postResponse = await _httpClient.PostAsJsonAsync("/example", newRequest);
            postResponse.EnsureSuccessStatusCode();

            // Act – usuń encję
            var deleteResponse = await _httpClient.DeleteAsync("/example/1");
            deleteResponse.EnsureSuccessStatusCode();

            // Assert – sprawdź, że GET zwraca pustą listę
            var getResponse = await _httpClient.GetAsync("/example");
            getResponse.EnsureSuccessStatusCode();

            var result = await getResponse.Content.ReadFromJsonAsync<ExampleListResponse>();
            Assert.NotNull(result);
            Assert.Empty(result.Data);
        }
    }
}
