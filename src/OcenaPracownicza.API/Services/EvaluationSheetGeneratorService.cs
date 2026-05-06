using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace OcenaPracownicza.API.Services;

public class EvaluationSheetGeneratorService : IEvaluationSheetGeneratorService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAchievementRepository _achievementRepository;
    private readonly IEvaluationPeriodService _evaluationPeriodService;
    private readonly ILogger<EvaluationSheetGeneratorService> _logger;

    public EvaluationSheetGeneratorService(
        IEmployeeRepository employeeRepository,
        IAchievementRepository achievementRepository,
        IEvaluationPeriodService evaluationPeriodService,    
        ILogger<EvaluationSheetGeneratorService> logger)
    {
        _employeeRepository = employeeRepository;
        _achievementRepository = achievementRepository;
        _evaluationPeriodService = evaluationPeriodService;
        _logger = logger;
    }

    public async Task GenerateSheetsForCurrentPeriodAsync()
    {
        var now = DateTime.UtcNow;
        var currentPeriod = await _evaluationPeriodService.GetPeriodByDateAsync(now);

        if (currentPeriod == null)
        {
            _logger.LogWarning("Generator arkuszy przerwany: Brak zdefiniowanego okresu ocen dla daty {Date}", now);
            return;
        }

        var employees = await _employeeRepository.GetAll();

        foreach (var employee in employees)
        {
            var existingAchievements = await _achievementRepository.GetByEmployeeIdAsync(employee.Id);

            bool sheetExists = existingAchievements.Any(a =>
                a.EvaluationPeriodId == currentPeriod.Id &&
                a.Name == "Arkusz Oceny Okresowej");

            if (!sheetExists)
            {
                var evaluationSheet = new Achievement
                {
                    Name = "Arkusz Oceny Okresowej",
                    Description = $"Automatycznie wygenerowany arkusz podsumowujący cele i osiągnięcia na okres {currentPeriod.Name}.",
                    Date = now,
                    Category = AchievementCategory.ProjectDelivery,

                    EvaluationPeriodId = currentPeriod.Id,

                    FinalScore = "Brak oceny",
                    AchievementsSummary = "Do wypełnienia przez pracownika...",
                    Stage2Status = EvaluationStageStatus.Draft,
                    EmployeeId = employee.Id
                };

                await _achievementRepository.Create(evaluationSheet);
                _logger.LogInformation("Wygenerowano nowy arkusz oceny dla pracownika {EmployeeId} na okres {PeriodName}",
                    employee.Id, currentPeriod.Name);
            }
        }
    }
}