namespace OcenaPracownicza.API.Responses
{
    public class LoginResponse : BaseResponse
    {
        public string Token { get; set; }

        public LoginResponse(string token, string message = "Login successful", bool success = true)
            : base(message, success)
        {
            Token = token;
        }
    }
}
