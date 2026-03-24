namespace OcenaPracownicza.API.Requests;

public class UpdateEmployeeByManagerRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Position { get; set; }
    public string? Period { get; set; }
    public string? FinalScore { get; set; }
    public string? AchievementsSummary { get; set; }
    public string? Comment { get; set; } // komentarz przełożonego
}