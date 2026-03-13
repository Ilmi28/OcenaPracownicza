namespace OcenaPracownicza.API.Exceptions.BaseExceptions;

public class ConflictException : Exception
{
    public ConflictException() : base("Wystąpił konflikt w bazie danych.")
    {
    }
    public ConflictException(string message) : base(message)
    {
    }
    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
    public ConflictException(string message, string details) : base(message)
    {
    }
}
