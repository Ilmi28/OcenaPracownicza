namespace OcenaPracownicza.API.Responses
{
    public class BaseResponse<TData> where TData : class
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Operacja zakończona sukcesem.";
        public TData Data { get; set; } = default!;
    }

    public class  BaseResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Operacja zakończona sukcesem.";
    }
}
