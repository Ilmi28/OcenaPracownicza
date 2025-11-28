using System.Net;
using System.Net.Http.Json;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.IntegrationTests.WebApplicationFactories;
using Xunit;

namespace OcenaPracownicza.IntegrationTests.Tests;

public class EmployeeTests : BaseTests<OcenaPracowniczaWebApplicationFactory>
{
    public EmployeeTests(OcenaPracowniczaWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await client.GetAsync("/employee");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Add_And_GetById_Works()
    {
        // Arrange
        var request = new EmployeeRequest
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Position = "Developer",
            Period = "2024",
            FinalScore = "5",
            AchievementsSummary = "Wzorowa praca"
        };

        // Act
        var postResponse = await client.PostAsJsonAsync("/employee", request);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

        // Assert
        Assert.NotNull(created);
        Assert.Equal("Jan", created.FirstName);

        // Get by id
        var getResponse = await client.GetAsync($"/employee/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
        Assert.NotNull(fetched);
        Assert.Equal("Jan", fetched.FirstName);
    }

    [Fact]
    public async Task Update_Works()
    {
        // Najpierw dodaj pracownika
        var request = new EmployeeRequest
        {
            FirstName = "Anna",
            LastName = "Nowak",
            Position = "Tester",
            Period = "2024",
            FinalScore = "4",
            AchievementsSummary = "Dobra praca"
        };
        var postResponse = await client.PostAsJsonAsync("/employee", request);
        var created = await postResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

        // Zaktualizuj dane
        var updateRequest = new EmployeeRequest
        {
            FirstName = "Anna",
            LastName = "Nowak",
            Position = "Senior Tester",
            Period = "2024",
            FinalScore = "5",
            AchievementsSummary = "Awans"
        };
        var putResponse = await client.PutAsJsonAsync($"/employee/{created.Id}", updateRequest);
        putResponse.EnsureSuccessStatusCode();
        var updated = await putResponse.Content.ReadFromJsonAsync<EmployeeResponse>();
        Assert.Equal("Senior Tester", updated.Position);
        Assert.Equal("5", updated.FinalScore);
    }

    [Fact]
    public async Task Delete_Works()
    {
        // Dodaj pracownika
        var request = new EmployeeRequest
        {
            FirstName = "Piotr",
            LastName = "Zieliński",
            Position = "Manager",
            Period = "2024",
            FinalScore = "3",
            AchievementsSummary = "Poprawna praca"
        };
        var postResponse = await client.PostAsJsonAsync("/employee", request);
        var created = await postResponse.Content.ReadFromJsonAsync<EmployeeResponse>();

        // Usuń pracownika
        var deleteResponse = await client.DeleteAsync($"/employee/{created.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        // Sprawdź, że nie istnieje
        var getResponse = await client.GetAsync($"/employee/{created.Id}");
        Assert.Equal(HttpStatusCode.InternalServerError, getResponse.StatusCode); // lub NotFound jeśli masz własną obsługę
    }
}
