namespace OcenaPracownicza.API.Views;

public class EmployeeProgressView
{
    public int TotalAchievements { get; set; }
    public int MetCategoriesCount { get; set; }
    public int TotalRequiredCategories { get; set; }

    // Oblicza procent ukończenia wymogów
    public double ProgressPercentage => TotalRequiredCategories == 0 ? 0
        : Math.Round((double)MetCategoriesCount / TotalRequiredCategories * 100, 2);

    public List<CategoryProgressView> Categories { get; set; } = [];
}