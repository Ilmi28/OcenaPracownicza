namespace OcenaPracownicza.API.Interfaces.Services;

public interface IEvaluationSheetGeneratorService
{
    Task GenerateSheetsForCurrentPeriodAsync();
}