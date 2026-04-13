using Microsoft.Extensions.Hosting;
using OcenaPracownicza.API.Interfaces.Services;

namespace OcenaPracownicza.API.Services;

public class EvaluationPeriodBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EvaluationPeriodBackgroundService> _logger;

    public EvaluationPeriodBackgroundService(
        IServiceProvider serviceProvider, 
        ILogger<EvaluationPeriodBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Uruchomiono usługę w tle do sprawdzania okresów oceny.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try 
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var generatorService = scope.ServiceProvider.GetRequiredService<IEvaluationSheetGeneratorService>();
                    await generatorService.GenerateSheetsForCurrentPeriodAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas automatycznego generowania arkuszy oceny.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}