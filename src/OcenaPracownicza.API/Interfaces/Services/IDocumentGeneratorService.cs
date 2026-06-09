using OcenaPracownicza.API.Entities;
using System.Collections.Generic;

namespace OcenaPracownicza.API.Interfaces.Services
{
    public interface IDocumentGeneratorService
    {
        byte[] GenerateReport(Employee employee, List<Achievement> achievements, List<AchievementElement> elements, EvaluationPeriod? evaluationPeriod = null);

        byte[] GenerateSummaryReport(List<Achievement> achievements);

        byte[] GenerateExcelReport(Employee employee, List<Achievement> achievements, List<AchievementElement> elements, EvaluationPeriod? evaluationPeriod = null);

        byte[] GenerateExcelSummaryReport(List<Achievement> achievements);
    }
}