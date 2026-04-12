namespace OcenaPracownicza.API.Requests;

public class UpdateEmployeeByManagerRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Position { get; set; }
    public string? Comment { get; set; } // komentarz przełożonego
}
