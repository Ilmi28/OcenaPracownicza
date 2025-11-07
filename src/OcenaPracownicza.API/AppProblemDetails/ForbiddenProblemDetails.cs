using Microsoft.AspNetCore.Mvc;

namespace OcenaPracownicza.API.AppProblemDetails;

public class ForbiddenProblemDetails : ProblemDetails
{
    public ForbiddenProblemDetails(string? detail)
    {
        Title = "Forbidden";
        Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4";
        Status = StatusCodes.Status403Forbidden;
        Detail = detail;
    }
}
