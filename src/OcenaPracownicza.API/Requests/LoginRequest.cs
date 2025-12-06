namespace OcenaPracownicza.API.Requests;

public class LoginRequest
{
    public required string UserNameEmail { get; set; }
    public required string Password { get; set; }
}
