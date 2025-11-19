using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using System.Net.Http.Json;
using Xunit;

namespace OcenaPracownicza.IntegrationTests
{
    public class ExampleTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;

        public ExampleTests(CustomWebApplicationFactory<Program> factory)
        {
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_ID", "test-client-id");
            Environment.SetEnvironmentVariable("GOOGLE_CLIENT_SECRET", "test-client-secret");

            _httpClient = factory.CreateClient();
        }

        private async Task<ExampleListResponse> AddExampleAndGetAll(string name)
        {
            var request = new ExampleRequest
            {
                Name = name,
                Description = "Desc",
                SomeDetail = "Detail"
            };
            var postResponse = await _httpClient.PostAsJsonAsync("/example", request);
            postResponse.EnsureSuccessStatusCode();

            return await _httpClient.GetFromJsonAsync<ExampleListResponse>("/example")!;
        }

        [Fact]
        public async Task GetById_ReturnsSingleEntity()
        {
            var all = await AddExampleAndGetAll("Entity1");
            var last = all.Data.Last();

            var index = all.Data.IndexOf(last);
            var getResponse = await _httpClient.GetAsync($"/example/{index + 1}");
            getResponse.EnsureSuccessStatusCode();

            var result = await getResponse.Content.ReadFromJsonAsync<ExampleResponse>();
            Assert.NotNull(result);
            Assert.Equal("Test Name", result.Data.Name);
        }

        [Fact]
        public async Task Post_AddsEntity_ThenGetReturnsIt()
        {
            var all = await AddExampleAndGetAll("Test Name");
            var last = all.Data.Last();

            Assert.Equal("Test Name", last.Name);
        }

        [Fact]
        public async Task Put_UpdatesEntity()
        {
            var all = await AddExampleAndGetAll("Old Name");
            var lastIndex = all.Data.Count;

            var updateRequest = new ExampleRequest
            {
                Name = "New Name",
                Description = "New Desc",
                SomeDetail = "New Detail"
            };
            var putResponse = await _httpClient.PutAsJsonAsync($"/example/{lastIndex}", updateRequest);
            putResponse.EnsureSuccessStatusCode();

            var updated = await putResponse.Content.ReadFromJsonAsync<ExampleResponse>();
            Assert.NotNull(updated);
            Assert.Equal("New Name", updated.Data.Name);
        }

        [Fact]
        public async Task Delete_RemovesEntity()
        {
            var all = await AddExampleAndGetAll("Delete Me");
            var lastIndex = all.Data.Count;

            var deleteResponse = await _httpClient.DeleteAsync($"/example/{lastIndex}");
            deleteResponse.EnsureSuccessStatusCode();

            var final = await _httpClient.GetFromJsonAsync<ExampleListResponse>("/example");
            Assert.DoesNotContain(final!.Data, e => e.Name == "Delete Me");
        }

        // NOWE TESTY

        [Fact]
        public async Task GetAll_ReturnsMultipleEntities()
        {
            await AddExampleAndGetAll("A");
            await AddExampleAndGetAll("B");
            var response = await _httpClient.GetFromJsonAsync<ExampleListResponse>("/example");

            Assert.NotNull(response);
            Assert.True(response.Data.Count >= 2);
            Assert.Contains(response.Data, e => e.Name == "A");
            Assert.Contains(response.Data, e => e.Name == "B");
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
            var putResponse = await _httpClient.PutAsJsonAsync("/example/9999", updateRequest);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, putResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_NonExistingEntity_ReturnsNotFound()
        {
            var deleteResponse = await _httpClient.DeleteAsync("/example/9999");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task MultiplePosts_PreserveOrder()
        {
            await AddExampleAndGetAll("First");
            await AddExampleAndGetAll("Second");
            var all = await _httpClient.GetFromJsonAsync<ExampleListResponse>("/example");

            Assert.Equal("First", all.Data[all.Data.Count - 2].Name);
            Assert.Equal("Second", all.Data.Last().Name);
        }
    }
}
