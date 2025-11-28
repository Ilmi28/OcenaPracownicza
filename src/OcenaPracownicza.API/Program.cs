using DotNetEnv;
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
