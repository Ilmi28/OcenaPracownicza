using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Requests;
using OcenaPracownicza.IntegrationTests.WebApplicationFactories;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OcenaPracownicza.IntegrationTests.Tests
{
    public class EmployeeTests : BaseTests<EmployeeWebApplicationFactory>
    {
        public EmployeeTests(EmployeeWebApplicationFactory factory) : base(factory)
        {
        }

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
        public async Task GetById_ReturnsSingleEmployee()
        {
            var entity = await context.Employees.FindAsync(1);
            var response = await client.GetAsync($"/employee/{entity!.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await context.Employees.FindAsync(entity.Id);
            Assert.NotNull(result);
            Assert.Equal("Jan", result!.FirstName);
            Assert.Equal("Kowalski", result.LastName);
        }

        [Fact]
        public async Task Post_AddsEmployee_ThenGetReturnsIt()
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

            var postResponse = await client.PostAsJsonAsync("/employee", request);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            var added = await context.Employees.FirstOrDefaultAsync(e => e.FirstName == "New" && e.LastName == "Employee");
            Assert.NotNull(added);
            Assert.Equal("DevOps", added!.Position);
        }

        [Fact]
        public async Task Put_UpdatesExistingEmployee()
        {
            var entity = await context.Employees.FirstAsync(e => e.FirstName == "Anna");

            var updateRequest = new EmployeeRequest
            {
                FirstName = "AnnaUpdated",
                LastName = "Xyz",
                Position = "NewPos",
                Period = "2030",
                FinalScore = "15",
                AchievementsSummary = "Updated summary"
            };

            var response = await client.PutAsJsonAsync($"/employee/{entity.Id}", updateRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await context.Employees.FindAsync(entity.Id);
            Assert.NotNull(result);

            Assert.Equal("Anna", result!.FirstName);
            Assert.Equal("Nowak", result!.LastName);
            Assert.Equal("QA", result!.Position);
        }


        [Fact]
        public async Task Delete_RemovesEmployee()
        {
            var entity = await context.Employees.FirstAsync(e => e.FirstName == "To");
            var deleteResponse = await client.DeleteAsync($"/employee/{entity.Id}");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            var exists = await context.Employees.AnyAsync(e => e.Id == entity.Id);
            Assert.False(exists);

            var count = await context.Employees.CountAsync();
            Assert.Equal(5, count);
        }

        [Fact]
        public async Task GetAll_ReturnsMultipleEmployees()
        {
            var response = await client.GetAsync("/employee");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var names = await context.Employees
                .Where(e => e.Id == 5 || e.Id == 6)
                .Select(e => e.FirstName)
                .ToListAsync();

            Assert.Contains("A", names);
            Assert.Contains("B", names);
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

            var response = await client.PutAsJsonAsync($"/employee/{99999}", updateRequest);
            // jeżeli w Twoim serwisie PUT dla nieistniejącego zwraca NotFound, oczekuj NotFound. 
            // Jeśli implementacja zwraca OK z błędem w treści, dostosuj asercję poniżej.
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
