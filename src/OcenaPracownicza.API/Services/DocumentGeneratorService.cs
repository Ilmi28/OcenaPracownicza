using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace Ocenapracownicza.API.Services
{
    public class DocumentGeneratorService : IDocumentGeneratorService
    {
        public byte[] GenerateReport(string employeeName, string position, string period, string finalScore, string achievementsSummary)
        {
            QuestPDF.Settings.License = LicenseType.Community;

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
                        column.Item().Text($"Imię i nazwisko: {employeeName}");
                        column.Item().Text($"Stanowisko: {position}");
                        column.Item().Text($"Okres oceny: {period}");
                        column.Item().Text($"Ocena końcowa: {finalScore}");

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        column.Item().Text("Podsumowanie osiągnięć:");
                        column.Item().Text(achievementsSummary);
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}");
                });
            });

            return document.GeneratePdf();
        }
    }
}
