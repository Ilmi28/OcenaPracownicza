using Microsoft.AspNetCore.Authentication;
using OcenaPracownicza.API.Requests;

namespace OcenaPracownicza.API.Interfaces.Services;

public interface IAuthService
{
    string Login(LoginRequest request);
    string LoginWithGoogle(AuthenticateResult result);
}
