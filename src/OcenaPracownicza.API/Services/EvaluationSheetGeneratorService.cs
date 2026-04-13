using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;

namespace OcenaPracownicza.API.Services;

public class EvaluationSheetGeneratorService : IEvaluationSheetGeneratorService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAchievementRepository _achievementRepository;
    private readonly ILogger<EvaluationSheetGeneratorService> _logger;

    public EvaluationSheetGeneratorService(
        IEmployeeRepository employeeRepository,
        IAchievementRepository achievementRepository,
        ILogger<EvaluationSheetGeneratorService> logger)
    {
        _employeeRepository = employeeRepository;
        _achievementRepository = achievementRepository;
        _logger = logger;
    }

    public async Task GenerateSheetsForCurrentPeriodAsync()
    {
        var year = DateTime.UtcNow.Year;
        var quarter = (DateTime.UtcNow.Month - 1) / 3 + 1;
        var currentPeriod = $"{year} Q{quarter}";
        
        var employees = await _employeeRepository.GetAll(); 
        
        foreach (var employee in employees)
        {
            var existingAchievements = await _achievementRepository.GetByEmployeeIdAsync(employee.Id);
            
            bool sheetExists = existingAchievements.Any(a => a.Period == currentPeriod && a.Name == "Arkusz Oceny Okresowej");

            if (!sheetExists)
            {
                var evaluationSheet = new Achievement
                {
                    Name = "Arkusz Oceny Okresowej",
                    Description = $"Automatycznie wygenerowany arkusz podsumowujący cele i osiągnięcia na okres {currentPeriod}.",
                    Date = DateTime.UtcNow,
                    Category = AchievementCategory.ProjectDelivery,
                    Period = currentPeriod,
                    FinalScore = "Brak oceny", 
                    AchievementsSummary = "Do wypełnienia przez pracownika...",
                    Stage2Status = EvaluationStageStatus.Draft, 
                    EmployeeId = employee.Id
                };
                
                await _achievementRepository.Create(evaluationSheet);
                _logger.LogInformation("Wygenerowano nowy arkusz oceny dla pracownika {EmployeeId} na okres {Period}", employee.Id, currentPeriod);
            }
        }
    }
}