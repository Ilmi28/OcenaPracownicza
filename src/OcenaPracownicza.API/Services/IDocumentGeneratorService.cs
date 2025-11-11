namespace Ocenapracownicza.API.Services
{
    public interface IDocumentGeneratorService
    {
        byte[] GenerateReport(string employeeName, string position, string period, string finalScore, string achievementsSummary);
    }
}
