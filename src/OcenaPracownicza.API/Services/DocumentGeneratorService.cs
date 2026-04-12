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
                        column.Item().Text("Oceny i podsumowania są prowadzone per osiągnięcie w tabeli osiągnięć.");
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
