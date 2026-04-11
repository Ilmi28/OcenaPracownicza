namespace OcenaPracownicza.API.Views;

public class CategoryProgressView
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentCount { get; set; }
    public int RequiredCount { get; set; }
    public bool IsMet => CurrentCount >= RequiredCount;
}