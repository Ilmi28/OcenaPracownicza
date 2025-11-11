using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace Ocenapracownicza.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        [HttpGet("generate")]
        public IActionResult GenerateReport()
        {
            // Inicjalizacja środowiska QuestPDF (zabezpieczenie przed błędem)
            QuestPDF.Settings.License = LicenseType.Community;

            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(12));
                        page.DefaultTextStyle(x => x.FontFamily("Arial"));

                        page.Header()
                            .Text("Raport oceny pracownika")
                            .FontSize(20)
                            .Bold()
                            .AlignCenter();

                        page.Content().Column(column =>
                        {
                            column.Item().Text("Imię i nazwisko: Jan Kowalski");
                            column.Item().Text("Stanowisko: Adiunkt");
                            column.Item().Text("Okres oceny: 2023/2024");
                            column.Item().Text("Ocena końcowa: Pozytywna");

                            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            column.Item().Text("Podsumowanie osiągnięć:");
                            column.Item().Text("• Publikacje naukowe: 3");
                            column.Item().Text("• Zajęcia dydaktyczne: 120 godzin");
                            column.Item().Text("• Udział w konferencjach: 2");
                        });

                        page.Footer()
                            .AlignCenter()
                            .Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", "Raport.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Błąd podczas generowania raportu: {ex.Message}");
            }
        }
    }
}
