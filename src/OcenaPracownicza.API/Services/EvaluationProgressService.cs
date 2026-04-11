using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Responses;
using OcenaPracownicza.API.Views;

namespace OcenaPracownicza.API.Services;

public class EvaluationProgressService : IEvaluationProgressService
{
    private readonly IUserManager _userManager;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAchievementRepository _achievementRepository;

    public EvaluationProgressService(
        IUserManager userManager,
        IEmployeeRepository employeeRepository,
        IAchievementRepository achievementRepository)
    {
        _userManager = userManager;
        _employeeRepository = employeeRepository;
        _achievementRepository = achievementRepository;
    }

    public async Task<BaseResponse<EmployeeProgressView>> GetMyProgressAsync()
    {
        var userId = _userManager.GetCurrentUserId()
            ?? throw new UnauthorizedAccessException("Musisz być zalogowany.");

        var employee = await _employeeRepository.GetByUserId(userId);
        if (employee == null)
            throw new NotFoundException("Profil pracownika nie istnieje dla tego konta.");

        var achievements = await _achievementRepository.GetByEmployeeIdAsync(employee.Id);

        // Zdefiniowanie minimów oceny (tzw. reguł biznesowych)
        // Możesz w przyszłości przenieść te wartości do bazy danych lub appsettings.json
        var requirements = new Dictionary<AchievementCategory, int>
        {
            { AchievementCategory.ProjectDelivery, 2 },     // Wymagane min. 2 dowiezione projekty
            { AchievementCategory.TechnicalGrowth, 1 },     // Min. 1 certyfikat/szkolenie
            { AchievementCategory.ProcessImprovement, 1 },  // Min. 1 usprawnienie procesu
            { AchievementCategory.Mentorship, 1 },
            { AchievementCategory.Innovation, 1 },
            { AchievementCategory.Leadership, 1 },
            { AchievementCategory.CustomerSuccess, 1 }
        };

        var categoriesView = new List<CategoryProgressView>();
        int metCategoriesCount = 0;

        foreach (var req in requirements)
        {
            var currentCount = achievements.Count(a => a.Category == req.Key);
            var isMet = currentCount >= req.Value;

            if (isMet) metCategoriesCount++;

            categoriesView.Add(new CategoryProgressView
            {
                CategoryId = (int)req.Key,
                CategoryName = req.Key.ToString(),
                CurrentCount = currentCount,
                RequiredCount = req.Value
            });
        }

        var view = new EmployeeProgressView
        {
            TotalAchievements = achievements.Count(),
            MetCategoriesCount = metCategoriesCount,
            TotalRequiredCategories = requirements.Count,
            Categories = categoriesView
        };

        return new BaseResponse<EmployeeProgressView>
        {
            Data = view,
            Message = "Pobrano podsumowanie postępów."
        };
    }
}