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
    public class EmployeeTests : BaseTests<EmployeeWebApplicationFactory>
    {
        public EmployeeTests(EmployeeWebApplicationFactory factory) : base(factory) { }

        protected override void SeedData()
        {
            context.Employees.AddRange(
                new Employee { Id = 1, FirstName = "Jan", LastName = "Kowalski", Position = "Dev", Period = "2024", FinalScore = "8", AchievementsSummary = "Good" },
                new Employee { Id = 2, FirstName = "Anna", LastName = "Nowak", Position = "QA", Period = "2024", FinalScore = "7", AchievementsSummary = "Solid" },
                new Employee { Id = 3, FirstName = "Piotr", LastName = "Zielinski", Position = "PM", Period = "2024", FinalScore = "9", AchievementsSummary = "Excellent" },
                new Employee { Id = 4, FirstName = "To", LastName = "Delete", Position = "Temp", Period = "2024", FinalScore = "5", AchievementsSummary = "Remove me" },
                new Employee { Id = 5, FirstName = "A", LastName = "Alpha", Position = "X", Period = "2024", FinalScore = "6", AchievementsSummary = "A" },
                new Employee { Id = 6, FirstName = "B", LastName = "Beta", Position = "Y", Period = "2024", FinalScore = "6", AchievementsSummary = "B" }
            );
        }

        [Fact]
        public async Task GetById_ReturnsEmployee()
        {
            var response = await client.GetAsync("/employee/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<BaseResponse<EmployeeView>>();
            Assert.NotNull(result);
            Assert.Equal("Jan", result!.Data.FirstName);
            Assert.Equal("Kowalski", result.Data.LastName);
        }

        [Fact]
        public async Task GetAll_ReturnsEmployees()
        {
            var response = await client.GetAsync("/employee");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var employees = await response.Content.ReadFromJsonAsync<BaseResponse<List<EmployeeView>>>();
            var data = employees!.Data;

            Assert.NotNull(data);
            Assert.Equal("Jan", data[0].FirstName);
            Assert.Equal("Anna"!, data[1].FirstName);
            Assert.Equal("Piotr", data[2].FirstName);
            Assert.Equal("To"!, data[3].FirstName);
            Assert.Equal("A", data[4].FirstName);
            Assert.Equal("B"!, data[5].FirstName);
        }

        [Fact]
        public async Task Post_AddsEmployee()
        {
            var request = new EmployeeRequest
            {
                FirstName = "New",
                LastName = "Employee",
                Position = "DevOps",
                Period = "2025",
                FinalScore = "10",
                AchievementsSummary = "Created in test"
            };

            var response = await client.PostAsJsonAsync("/employee", request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var added = await context.Employees.FirstOrDefaultAsync(e => e.FirstName == "New" && e.LastName == "Employee");
            Assert.NotNull(added);
            Assert.Equal("DevOps", added!.Position);
        }

        [Fact]
        public async Task Put_UpdatesEmployee()
        {
            var updateRequest = new EmployeeRequest
            {
                FirstName = "AnnaUpdated",
                LastName = "Xyz",
                Position = "NewPos",
                Period = "2030",
                FinalScore = "15",
                AchievementsSummary = "Updated summary"
            };

            var response = await client.PutAsJsonAsync("/employee/2", updateRequest);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updated = await context.Employees.FindAsync(2);
            await context.Entry(updated!).ReloadAsync();
            Assert.Equal("AnnaUpdated", updated!.FirstName);
            Assert.Equal("Xyz", updated.LastName);
        }

        [Fact]
        public async Task Put_NonExistingEmployee_ReturnsNotFound()
        {
            var updateRequest = new EmployeeRequest
            {
                FirstName = "DoesNot",
                LastName = "Exist",
                Position = "None",
                Period = "2025",
                FinalScore = "0",
                AchievementsSummary = "N/A"
            };

            var response = await client.PutAsJsonAsync("/employee/99999", updateRequest);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_RemovesEmployee()
        {
            var response = await client.DeleteAsync("/employee/4");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var exists = await context.Employees.AnyAsync(e => e.Id == 4);
            Assert.False(exists);
        }
    }
}
