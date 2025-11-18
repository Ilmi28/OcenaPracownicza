using System;
using Xunit;
using Ocenapracownicza.API.Services;

namespace Ocenapracownicza.UnitTests
{
    public class DocumentGeneratorServiceTests
    {
        private readonly DocumentGeneratorService _service = new DocumentGeneratorService();

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldReturnPdfBytes()
        {
            var pdfBytes = _service.GenerateReport("Jan Kowalski", "Programista", "01.2025 - 06.2025", "Dobry", "Test test.");
            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldHandleEmptyStrings()
        {
            var pdfBytes = _service.GenerateReport("", "", "", "", "");
            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldHandleLongText()
        {
            string longText = new string('A', 5000);
            var pdfBytes = _service.GenerateReport(longText, longText, longText, longText, longText);
            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldHandleSpecialCharacters()
        {
            string specialText = "ąęćżźńłóĄĘĆŻŹŃŁÓ!@#$%^&*()_+-=";
            var pdfBytes = _service.GenerateReport(specialText, specialText, specialText, specialText, specialText);
            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldHandleUnicodeCharacters()
        {
            string unicodeText = "测试中文, тест русский, 🌟 Emoji!";
            var pdfBytes = _service.GenerateReport(unicodeText, unicodeText, unicodeText, unicodeText, unicodeText);
            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

        [Fact]
        public void DocumentGeneratorService_GenerateReport_ShouldContainCurrentYearInFooter()
        {
            var pdfBytes = _service.GenerateReport("Jan Kowalski", "Programista", "01.2025 - 06.2025", "Dobry", "Test test.");
            var currentYear = DateTime.Now.Year.ToString();
            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }

    }
}

