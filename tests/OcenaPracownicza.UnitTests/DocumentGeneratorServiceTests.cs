using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Xunit;
using Ocenapracownicza.API.Services;

namespace Ocenapracownicza.UnitTests
{
    public class DocumentGeneratorServiceTests
    {
        [Fact]
        public void GenerateReport_ShouldReturnPdfBytes()
        {
            var service = new DocumentGeneratorService();
            string employeeName = "Jan Kowalski";
            string position = "Programista";
            string period = "01.2025 - 06.2025";
            string finalScore = "Dobry";
            string achievementsSummary = "Test test.";

            byte[] pdfBytes = service.GenerateReport(employeeName, position, period, finalScore, achievementsSummary);

            Assert.NotNull(pdfBytes);
            Assert.NotEmpty(pdfBytes);
            Assert.True(pdfBytes.Length > 1000);
        }
    }
}
