using Microsoft.AspNetCore.Authentication;
using OcenaPracownicza.API.Requests;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IAuthService
{
    Task<string> Login(LoginRequest request);
    Task<string> LoginWithGoogle(AuthenticateResult result);
    Task<string> Register(RegisterRequest request);
}
