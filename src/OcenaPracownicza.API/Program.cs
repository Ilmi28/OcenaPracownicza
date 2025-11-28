using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.AppProblemDetails;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Repositories;
using OcenaPracownicza.API.Services;
using OcenaPracownicza.API.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Ocenapracownicza.API.Services;
using OcenaPracownicza.API.Extensions;



namespace OcenaPracownicza;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Env.Load();

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddAppDbContext(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwagger();
        builder.Services.AddAuthenticationWithGoogle(builder.Configuration);
        builder.Services.AddServices();
        builder.Services.AddRepositories();
        builder.Services.AddAuthorization();
        builder.Services.AddCorsWithPolicies();
        builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        builder.Services.AddScoped<IEmployeeService, EmployeeService>();

    }

    public static void ConfigureMiddleware(WebApplication app)
    {
        app.UseProblemDetailsExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }

}
