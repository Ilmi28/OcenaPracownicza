namespace OcenaPracownicza.API.Exceptions.BaseExceptions;

public class InternalServerErrorException : Exception
{
    public InternalServerErrorException(string message) : base(message)
    {
    }
    public InternalServerErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }
    public InternalServerErrorException(string message, string details) : base(message)
    {
    }
}
