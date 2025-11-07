using Microsoft.AspNetCore.Mvc;

namespace OcenaPracownicza.API.AppProblemDetails;

public class BadRequestProblemDetails : ProblemDetails
{
    public BadRequestProblemDetails(string? detail)
    {
        Title = "Conflict";
        Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10";
        Status = StatusCodes.Status409Conflict;
        Detail = detail;
    }
}
