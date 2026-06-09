using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using OcenaPracownicza.API.Enums;

namespace Ocenapracownicza.API.Services
{
    public class DocumentGeneratorService : IDocumentGeneratorService
    {
        private static string GetActivityLabel(int id)
            => AchievementDictionary.Activities.FirstOrDefault(x => x.Id == id)?.Label ?? "Inny obszar działalności";

        private static string GetDepartmentLabel(int id)
            => AchievementDictionary.Departments.FirstOrDefault(x => x.Id == id)?.Label ?? "Inny dział weryfikujący";

        private static string GetCategoryLabel(int id)
            => AchievementDictionary.Categories.FirstOrDefault(x => x.Id == id)?.Label ?? "Pozostałe osiągnięcia";

        private static string PobierzNazweStatusu(EvaluationStageStatus status) => status switch
        {
            EvaluationStageStatus.Draft => "Szkic",
            EvaluationStageStatus.PendingStage2 => "Oczekujący",
            EvaluationStageStatus.Stage2Approved => "Zatwierdzony",
            _ => status.ToString()
        };

        public byte[] GenerateReport(Employee employee, List<Achievement> achievements, List<AchievementElement> elements, EvaluationPeriod? evaluationPeriod = null)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var selectedPeriodName = evaluationPeriod?.Name
                ?? achievements.FirstOrDefault()?.EvaluationPeriod?.Name ?? "Wszystkie okresy";

            var filtrowaneAchievements = achievements
                .Where(x => x.Stage2Status != EvaluationStageStatus.Draft)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Portrait());
                    page.Margin(1.2f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(8.5f).FontFamily("Arial"));

                    page.Header().PaddingBottom(12).Column(col =>
                    {
                        col.Item().AlignRight().Text("Załącznik do Regulaminu oceny okresowej").FontSize(7).Italic().FontColor(Colors.Grey.Darken1);
                        col.Item().Text("ARKUSZ OKRESOWEJ OCENY NAUCZYCIELA AKADEMICKIEGO").FontSize(13).Bold().FontColor(Colors.Blue.Darken4);
                        col.Item().PaddingVertical(2).Text($"Okres oceny: {selectedPeriodName}").FontSize(10).Italic();
                        col.Item().PaddingVertical(4).LineHorizontal(1.5f).LineColor(Colors.Blue.Darken4);

                        col.Item().PaddingTop(6).Border(0.5f).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten5).Padding(8).Grid(grid =>
                        {
                            grid.Columns(2);
                            grid.Item().Text($"Pracownik: {employee?.FirstName} {employee?.LastName}").Bold();

                            var suma = filtrowaneAchievements.Sum(x => decimal.TryParse(x.FinalScore, out var s) ? s : 0).ToString("F2");
                            grid.Item().Text($"Suma przyznanych punktów: {suma}").Bold().FontColor(Colors.Green.Darken3);
                        });
                    });

                    page.Content().Column(col =>
                    {
                        var poDzialalnosciach = filtrowaneAchievements.GroupBy(a => {
                            var el = elements.FirstOrDefault(e => e.Id == a.AchievementElementId);
                            return el?.ActivityId ?? 0;
                        }).OrderBy(g => g.Key);

                        foreach (var grupaDzialalnosci in poDzialalnosciach)
                        {
                            col.Item().PaddingTop(12).PaddingBottom(2).Text(GetActivityLabel(grupaDzialalnosci.Key).ToUpper()).FontSize(11).Bold().FontColor(Colors.Blue.Darken4);
                            col.Item().LineHorizontal(0.5f).LineColor(Colors.Blue.Lighten3);

                            var poDzialach = grupaDzialalnosci.GroupBy(a => {
                                var el = elements.FirstOrDefault(e => e.Id == a.AchievementElementId);
                                return el?.DepartmentId ?? 0;
                            }).OrderBy(g => g.Key);

                            foreach (var grupaDzialu in poDzialach)
                            {
                                col.Item().PaddingTop(8).PaddingLeft(5).Text($"Dział weryfikujący: {GetDepartmentLabel(grupaDzialu.Key)}").FontSize(9.5f).Bold().FontColor(Colors.Blue.Darken2);

                                var poKategoriach = grupaDzialu.GroupBy(a => {
                                    var el = elements.FirstOrDefault(e => e.Id == a.AchievementElementId);
                                    return el?.CategoryId ?? 0;
                                }).OrderBy(g => g.Key);

                                foreach (var grupaKategorii in poKategoriach)
                                {
                                    col.Item().PaddingTop(4).PaddingLeft(10).PaddingBottom(2).Text($"• {GetCategoryLabel(grupaKategorii.Key)}").FontSize(8.5f).Bold().FontColor(Colors.Grey.Darken3);

                                    col.Item().PaddingLeft(10).PaddingBottom(8).Table(table =>
                                    {
                                        table.ColumnsDefinition(cols =>
                                        {
                                            cols.RelativeColumn(0.7f);   
                                            cols.RelativeColumn(4.0f);     
                                            cols.RelativeColumn(0.7f);    
                                            cols.RelativeColumn(0.7f);    
                                            cols.RelativeColumn(1.1f);   
                                            cols.RelativeColumn(1.1f);   
                                        });

                                        table.Header(h =>
                                        {
                                            h.Cell().Element(HeaderStyle).Text("Kod");
                                            h.Cell().Element(HeaderStyle).Text("Nazwa kryterium oraz uzasadnienie pracownika");
                                            h.Cell().Element(HeaderStyle).Text("Pkt").AlignCenter();
                                            h.Cell().Element(HeaderStyle).Text("Max").AlignCenter();
                                            h.Cell().Element(HeaderStyle).Text("Status");
                                            h.Cell().Element(HeaderStyle).Text("Data");
                                        });

                                        foreach (var a in grupaKategorii)
                                        {
                                            var el = elements.FirstOrDefault(e => e.Id == a.AchievementElementId);
                                            string backgroundColor = GetStatusBackgroundColor(a.Stage2Status);

                                            BuildCell(table.Cell(), backgroundColor).Text(el?.Code ?? "-");

                                            BuildCell(table.Cell(), backgroundColor).Column(c =>
                                            {
                                                c.Item().Text(a.Name ?? "-");
                                                if (!string.IsNullOrEmpty(a.Description))
                                                {
                                                    c.Item().Text($"Uzasadnienie: {a.Description}").FontSize(7.5f).Italic().FontColor(Colors.Grey.Darken2);
                                                }
                                            });

                                            BuildCell(table.Cell(), backgroundColor).AlignCenter().Text(a.FinalScore ?? "0").Bold();
                                            BuildCell(table.Cell(), backgroundColor).AlignCenter().Text(el?.BasePoints.ToString("F2") ?? "0");
                                            BuildCell(table.Cell(), backgroundColor).Text(PobierzNazweStatusu(a.Stage2Status)).Bold();
                                            BuildCell(table.Cell(), backgroundColor).Text(a.Date.ToString("dd.MM.yyyy"));
                                        }
                                    });
                                }
                            }
                        }
                    });

                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Strona ").FontSize(7.5f);
                        x.CurrentPageNumber().FontSize(7.5f);
                        x.Span(" z ").FontSize(7.5f);
                        x.TotalPages().FontSize(7.5f);
                    });
                });
            });
            return document.GeneratePdf();
        }

        public byte[] GenerateSummaryReport(List<Achievement> achievements)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.Header().PaddingBottom(10).Text("Raport zbiorczy osiągnięć pracowników").FontSize(15).Bold().AlignCenter().FontColor(Colors.Blue.Darken4);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c => { c.RelativeColumn(3); c.RelativeColumn(2); c.RelativeColumn(2); c.RelativeColumn(1.2f); });
                        table.Header(h => {
                            h.Cell().Element(HeaderStyle).Text("Pracownik");
                            h.Cell().Element(HeaderStyle).Text("Okres");
                            h.Cell().Element(HeaderStyle).Text("Status");
                            h.Cell().Element(HeaderStyle).Text("Wynik pkt").AlignCenter();
                        });

                        foreach (var a in achievements.Where(x => x.Stage2Status != EvaluationStageStatus.Draft))
                        {
                            string backgroundColor = GetStatusBackgroundColor(a.Stage2Status);

                            BuildCell(table.Cell(), backgroundColor).Text($"{a.Employee?.FirstName} {a.Employee?.LastName}");
                            BuildCell(table.Cell(), backgroundColor).Text(a.EvaluationPeriod?.Name ?? "-");
                            BuildCell(table.Cell(), backgroundColor).Text(PobierzNazweStatusu(a.Stage2Status));
                            BuildCell(table.Cell(), backgroundColor).AlignCenter().Text(a.FinalScore ?? "0").Bold();
                        }
                    });
                });
            });
            return document.GeneratePdf();
        }

        private static IContainer BuildCell(IContainer container, string backgroundColor)
        {
            var cell = container.Padding(4).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);
            if (!string.IsNullOrEmpty(backgroundColor)) cell = cell.Background(backgroundColor);
            return cell;
        }

        private static string GetStatusBackgroundColor(EvaluationStageStatus status)
        {
            if (status == EvaluationStageStatus.PendingStage2) return Colors.Yellow.Lighten5;
            if (status == EvaluationStageStatus.Stage2Approved) return Colors.Green.Lighten5;
            return string.Empty;
        }

        private static IContainer HeaderStyle(IContainer container) =>
            container.Background(Colors.Blue.Darken4).Padding(4).DefaultTextStyle(x => x.Bold().FontColor(Colors.White).FontSize(8.0f));


        public byte[] GenerateExcelReport(Employee employee, List<Achievement> achievements, List<AchievementElement> elements, EvaluationPeriod? evaluationPeriod = null)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Pracownik:;{employee?.FirstName} {employee?.LastName};;;;;");
            sb.AppendLine($"Okres ewaluacji:;{(evaluationPeriod?.Name ?? "Wszystkie okresy")};;;;;");
            sb.AppendLine(";;;;;;");    

            sb.AppendLine("Obszar działalności;Dział weryfikujący;Kategoria słownika;Kod;Nazwa kryterium;Uzasadnienie pracownika;Punkty przyznane;Punkty bazowe (Max);Status;Data zaistnienia");

            var filtrowane = achievements.Where(x => x.Stage2Status != EvaluationStageStatus.Draft).ToList();

            var poDzialalnosciach = filtrowane.GroupBy(a => {
                var el = elements.FirstOrDefault(e => e.Id == a.AchievementElementId);
                return el?.ActivityId ?? 0;
            }).OrderBy(g => g.Key);

            foreach (var gActivity in poDzialalnosciach)
            {
                string nazwaObszaru = GetActivityLabel(gActivity.Key);

                var poDzialach = gActivity.GroupBy(a => {
                    var el = elements.FirstOrDefault(e => e.Id == a.AchievementElementId);
                    return el?.DepartmentId ?? 0;
                }).OrderBy(g => g.Key);

                foreach (var gDept in poDzialach)
                {
                    string nazwaDzialu = GetDepartmentLabel(gDept.Key);

                    var poKategoriach = gDept.GroupBy(a => {
                        var el = elements.FirstOrDefault(e => e.Id == a.AchievementElementId);
                        return el?.CategoryId ?? 0;
                    }).OrderBy(g => g.Key);

                    foreach (var gCat in poKategoriach)
                    {
                        string nazwaKategorii = GetCategoryLabel(gCat.Key);

                        foreach (var a in gCat)
                        {
                            var el = elements.FirstOrDefault(e => e.Id == a.AchievementElementId);
                            string statusTekst = PobierzNazweStatusu(a.Stage2Status);

                            string oczyszczonaNazwa = a.Name?.Replace("\r", "").Replace("\n", " ").Replace(";", ",") ?? "";
                            string oczyszczonyOpis = a.Description?.Replace("\r", "").Replace("\n", " ").Replace(";", ",") ?? "";

                            sb.AppendLine($"{nazwaObszaru};{nazwaDzialu};{nazwaKategorii};{el?.Code};{oczyszczonaNazwa};{oczyszczonyOpis};{a.FinalScore};{el?.BasePoints};{statusTekst};{a.Date:dd.MM.yyyy}");
                        }
                    }
                }
            }
            return ConvertToExcelCsvBytes(sb.ToString());
        }

        public byte[] GenerateExcelSummaryReport(List<Achievement> achievements)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Pracownik;Okres;Status;Punkty przyznane");
            foreach (var a in achievements.Where(x => x.Stage2Status != EvaluationStageStatus.Draft))
            {
                sb.AppendLine($"{a.Employee?.FirstName} {a.Employee?.LastName};{a.EvaluationPeriod?.Name};{PobierzNazweStatusu(a.Stage2Status)};{a.FinalScore}");
            }
            return ConvertToExcelCsvBytes(sb.ToString());
        }

        private byte[] ConvertToExcelCsvBytes(string csvContent)
        {
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            return new byte[] { 0xEF, 0xBB, 0xBF }.Concat(bytes).ToArray();
        }

        public byte[] GenerateReport(Employee employee, List<Achievement> achievements) => GenerateReport(employee, achievements, new List<AchievementElement>());
        public byte[] GenerateExcelReport(Employee employee, List<Achievement> achievements) => GenerateExcelReport(employee, achievements, new List<AchievementElement>());
    }
}