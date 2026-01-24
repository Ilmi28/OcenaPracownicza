using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Extensions;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Requests;



namespace OcenaPracownicza.API;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Env.Load();

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var appDbContext = services.GetRequiredService<ApplicationDbContext>();
            var adminService = services.GetRequiredService<IAdminService>();

            await appDbContext.Database.MigrateAsync();

            var adminCount = await appDbContext.Admins.CountAsync();
            if (adminCount == 0)
            {
                await adminService.Add(new CreateAdminRequest
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "admin",
                    Email = "admin@mail.com",
                    Password = "Admin123!"
                }, true);
            }
        }

        await app.RunAsync();
    }

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddAppDbContextWithIdentity(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwagger();
        builder.Services.AddAuthenticationWithGoogle(builder.Configuration);
        builder.Services.AddServices();
        builder.Services.AddRepositories();
        builder.Services.AddAuthorization();
        builder.Services.AddCorsWithPolicies();
        builder.Services.AddHttpContextAccessor();
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
