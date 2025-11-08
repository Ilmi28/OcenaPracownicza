namespace OcenaPracownicza.API.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public BaseResponse(string message, bool success = true)
        {
            Success = success;
            Message = message;
        }
    }
}
