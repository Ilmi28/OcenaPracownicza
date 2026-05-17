using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header()
                        .Text("Raport oceny pracownika")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();

                    page.Content().Column(column =>
                    {
                        column.Spacing(5);
                        column.Item().Text($"Imię i nazwisko: {employee.FirstName} {employee.LastName}");
                        column.Item().Text($"Stanowisko: {employee.Position}");

                        column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        foreach (var a in achievements)
                        {
                            // Używamy danych z powiązanej encji EvaluationPeriod
                            column.Item().Text($"Okres: {a.EvaluationPeriod?.Name ?? "Brak danych"}").Bold();
                            column.Item().Text($"Ocena końcowa: {a.FinalScore}");
                            column.Item().Text($"Kategoria: {a.Category}");
                            column.Item().Text($"Osiągnięcie: {a.Name}");
                            column.Item().Text($"Opis: {a.Description}");
                            column.Item().Text($"Podsumowanie: {a.AchievementsSummary}");

                            if (!string.IsNullOrEmpty(a.Stage2Comment))
                                column.Item().Text($"Komentarz przełożonego: {a.Stage2Comment}").Italic();

                            column.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Wygenerowano: ");
                            x.Span($"{DateTime.Now:dd.MM.yyyy HH:mm}");
                        });
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
                    page.Margin(1, Unit.Centimetre); // Mniejszy margines dla tabeli zbiorczej
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header()
                        .Text("Raport zbiorczy pracowników")
                        .FontSize(18)
                        .Bold()
                        .AlignCenter();

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Imię i nazwisko
                            columns.RelativeColumn(2); // Stanowisko
                            columns.RelativeColumn(2); // Okres
                            columns.RelativeColumn(1); // Ocena
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Imię i nazwisko");
                            header.Cell().Element(CellStyle).Text("Stanowisko");
                            header.Cell().Element(CellStyle).Text("Okres");
                            header.Cell().Element(CellStyle).Text("Ocena");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.Bold())
                                                .PaddingVertical(5)
                                                .BorderBottom(1)
                                                .BorderColor(Colors.Black);
                            }
                        });

                        foreach (var a in achievements)
                        {
                            table.Cell().Element(ContentStyle).Text($"{a.Employee?.FirstName} {a.Employee?.LastName}");
                            table.Cell().Element(ContentStyle).Text(a.Employee?.Position ?? "-");
                            table.Cell().Element(ContentStyle).Text(a.EvaluationPeriod?.Name ?? "-");
                            table.Cell().Element(ContentStyle).Text(a.FinalScore);

                            static IContainer ContentStyle(IContainer container)
                            {
                                return container.PaddingVertical(2).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);
                            }
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        }
        public byte[] GenerateExcelReport(Employee employee, List<Achievement> achievements)
        {
            var sb = new StringBuilder();

            sb.AppendLine("RAPORT OCENY PRACOWNIKA;;;;;;");
            sb.AppendLine($"Pracownik:;{employee.FirstName} {employee.LastName};;;;;");
            sb.AppendLine($"Stanowisko:;{employee.Position};;;;;");
            sb.AppendLine($"Data wygenerowania:;{DateTime.Now:dd.MM.yyyy HH:mm};;;;;");
            sb.AppendLine(";;;;;;"); 

            sb.AppendLine("Okres oceny;Kategoria osiągnięcia;Nazwa osiągnięcia;Ocena końcowa;Podsumowanie celów;Komentarz przełożonego;Data wpisu");

            foreach (var a in achievements)
            {
                var period = a.EvaluationPeriod?.Name ?? "Brak danych";
                var category = a.Category.ToString();

                var name = (a.Name ?? "").Replace(";", ",").Replace("\r", " ").Replace("\n", " ");
                var score = a.FinalScore ?? "Brak oceny";
                var summary = (a.AchievementsSummary ?? "").Replace(";", ",").Replace("\r", " ").Replace("\n", " ");
                var comment = (a.Stage2Comment ?? "").Replace(";", ",").Replace("\r", " ").Replace("\n", " ");
                var date = a.Date.ToString("dd.MM.yyyy");

                sb.AppendLine($"{period};{category};{name};{score};{summary};{comment};{date}");
            }

            return ConvertToExcelCsvBytes(sb.ToString());
        }

        public byte[] GenerateExcelSummaryReport(List<Achievement> achievements)
        {
            var sb = new StringBuilder();

            // Nagłówki kolumn tabeli zbiorczej
            sb.AppendLine("Imię i nazwisko pracownika;Stanowisko;Okres oceny;Kategoria osiągnięcia;Nazwa osiągnięcia;Status etapu 2;Ocena końcowa");

            foreach (var a in achievements)
            {
                var fullName = $"{a.Employee?.FirstName} {a.Employee?.LastName}";
                var position = a.Employee?.Position ?? "-";
                var period = a.EvaluationPeriod?.Name ?? "-";
                var category = a.Category.ToString();
                var name = (a.Name ?? "").Replace(";", ",").Replace("\r", " ").Replace("\n", " ");
                var status = a.Stage2Status.ToString();
                var score = a.FinalScore ?? "-";

                sb.AppendLine($"{fullName};{position};{period};{category};{name};{status};{score}");
            }

            return ConvertToExcelCsvBytes(sb.ToString());
        }

        private byte[] ConvertToExcelCsvBytes(string csvContent)
        {
            var csvBytes = Encoding.UTF8.GetBytes(csvContent);
            var bom = new byte[] { 0xEF, 0xBB, 0xBF }; 

            var finalBytes = new byte[bom.Length + csvBytes.Length];
            Buffer.BlockCopy(bom, 0, finalBytes, 0, bom.Length);
            Buffer.BlockCopy(csvBytes, 0, finalBytes, bom.Length, csvBytes.Length);

            return finalBytes;
        }
    }
}