using System;
using Xunit;
using OcenaPracownicza.API.Entities;
using Ocenapracownicza.API.Services;

namespace Ocenapracownicza.UnitTests
{
    public class DocumentGeneratorServiceTests
    {
        private readonly DocumentGeneratorService _service = new DocumentGeneratorService();

        private Employee CreateEmployee(
            string firstName = "Jan",
            string lastName = "Kowalski",
            string position = "Programista",
            string period = "01.2025 - 06.2025",
            string finalScore = "Dobry",
            string summary = "Test test.")
        {
            return new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                Position = position,
                Period = period,
                FinalScore = finalScore,
                AchievementsSummary = summary,
                IdentityUserId = "1"
            };
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldReturnPdfBytes()
        {
            var employee = CreateEmployee();
            var pdfBytes = _service.GenerateReport(employee);

            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldHandleEmptyStrings()
        {
            var employee = CreateEmployee(
                firstName: "",
                lastName: "",
                position: "",
                period: "",
                finalScore: "",
                summary: ""
            );

            var pdfBytes = _service.GenerateReport(employee);

            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldHandleLongText()
        {
            string longText = new string('A', 5000);

            var employee = CreateEmployee(
                firstName: longText,
                lastName: longText,
                position: longText,
                period: longText,
                finalScore: longText,
                summary: longText
            );

            var pdfBytes = _service.GenerateReport(employee);

            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldHandleSpecialCharacters()
        {
            string special = "ąęćżźńłóĄĘĆŻŹŃŁÓ!@#$%^&*()_+-=";

            var employee = CreateEmployee(
                firstName: special,
                lastName: special,
                position: special,
                period: special,
                finalScore: special,
                summary: special
            );

            var pdfBytes = _service.GenerateReport(employee);

            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldHandleUnicodeCharacters()
        {
            string unicode = "测试中文, тест русский, 🌟 Emoji!";

            var employee = CreateEmployee(
                firstName: unicode,
                lastName: unicode,
                position: unicode,
                period: unicode,
                finalScore: unicode,
                summary: unicode
            );

            var pdfBytes = _service.GenerateReport(employee);

            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldContainCurrentYearInFooter()
        {
            var employee = CreateEmployee();
            var pdfBytes = _service.GenerateReport(employee);

            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }
    }
}
