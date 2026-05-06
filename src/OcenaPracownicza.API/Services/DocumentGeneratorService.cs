using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

namespace Ocenapracownicza.API.Services
{
    public class DocumentGeneratorService : IDocumentGeneratorService
    {

        public byte[] GenerateReport(Employee employee)
        {
            return GenerateReport(employee, new List<Achievement>());
        }
        public byte[] GenerateReport(Employee employee, List<Achievement> achievements)
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
                        column.Item().Text($"Imię i nazwisko: {employee.FirstName} {employee.LastName}");
                        column.Item().Text($"Stanowisko: {employee.Position}");

                        column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        foreach (var a in achievements)
                        {
                            column.Item().Text($"Okres: {a.Period}");
                            column.Item().Text($"Ocena: {a.FinalScore}");
                            column.Item().Text($"Podsumowanie: {a.AchievementsSummary}");
                            column.Item().Text($"Osiągnięcie: {a.Name}");
                            column.Item().Text($"Opis: {a.Description}");

                            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}");
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateSummaryReport(List<Achievement> achievements)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .Text("Raport zbiorczy pracowników")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Imię i nazwisko").Bold();
                            header.Cell().Text("Stanowisko").Bold();
                            header.Cell().Text("Okres").Bold();
                            header.Cell().Text("Ocena").Bold();
                        });

                        foreach (var a in achievements)
                        {
                            table.Cell().Text($"{a.Employee.FirstName} {a.Employee.LastName}");
                            table.Cell().Text(a.Employee.Position);
                            table.Cell().Text(a.Period);
                            table.Cell().Text(a.FinalScore);
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}