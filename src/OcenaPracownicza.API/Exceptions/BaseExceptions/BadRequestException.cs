namespace OcenaPracownicza.API.Exceptions.BaseExceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException() : base("Nieprawidłowe żądanie.")
        {
        }
        public BadRequestException(string message) : base(message)
        {
        }
        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public BadRequestException(string message, string details) : base(message)
        {
        }
    }
}
