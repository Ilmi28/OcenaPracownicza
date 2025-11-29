namespace OcenaPracownicza.API.Exceptions.BaseExceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Nie masz odpowiednich uprawnień.")
    {
    }
    public ForbiddenException(string message) : base(message)
    {
    }
    public ForbiddenException(string message, Exception innerException) : base(message, innerException)
    {
    }
    public ForbiddenException(string message, string details) : base(message)
    {
    }
}
