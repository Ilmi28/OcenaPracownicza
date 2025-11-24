using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OcenaPracownicza.API.AppProblemDetails;
using OcenaPracownicza.API.Exceptions.BaseExceptions;

namespace OcenaPracownicza.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseProblemDetailsExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                    ProblemDetails problemDetails = exception switch
                    {
                        ConflictException => new ConflictProblemDetails(exception.Message),
                        NotFoundException => new NotFoundProblemDetails(exception.Message),
                        UnauthorizedAccessException => new UnauthorizedProblemDetails(exception.Message),
                        ArgumentNullException => new BadRequestProblemDetails(exception.Message),
                        InvalidOperationException => new BadRequestProblemDetails(exception.Message),
                        ForbiddenException => new ForbiddenProblemDetails(exception.Message),
                        _ => new InternalServerErrorProblemDetails(exception!.Message)
                    };

                    context.Response.StatusCode = problemDetails.Status!.Value;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                });
            });
        }
    }
}
