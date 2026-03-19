using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Enums;
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

        await SeedAsync(app);

        await app.RunAsync();
    }

    private static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var db = services.GetRequiredService<ApplicationDbContext>();

        if (db.Database.IsRelational())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        await EnsureRoleExists(roleManager, "Admin");
        await EnsureRoleExists(roleManager, "Manager");
        await EnsureRoleExists(roleManager, "Employee");

        var adminUser = await EnsureUserWithRole(
            userManager,
            userName: "admin",
            email: "admin@mail.com",
            password: "Admin123!",
            role: "Admin");

        if (!await db.Admins.AnyAsync(a => a.IdentityUserId == adminUser.Id))
        {
            db.Admins.Add(new Admin
            {
                FirstName = "Super",
                LastName = "Admin",
                IdentityUserId = adminUser.Id
            });

            await db.SaveChangesAsync();
        }

        await SeedSampleDataAsync(db, userManager);
    }

    private static async Task EnsureRoleExists(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task<IdentityUser> EnsureUserWithRole(
        UserManager<IdentityUser> userManager,
        string userName,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join("; ", addRoleResult.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
        }

        return user;
    }

    private static async Task SeedSampleDataAsync(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        var managerUser = await EnsureUserWithRole(
            userManager,
            userName: "manager.demo",
            email: "manager.demo@mail.com",
            password: "Manager123!",
            role: "Manager");

        if (!await db.Managers.AnyAsync(m => m.IdentityUserId == managerUser.Id))
        {
            db.Managers.Add(new Manager
            {
                FirstName = "Marta",
                LastName = "Kierownik",
                IdentityUserId = managerUser.Id,
                AchievementsSummary = "Zarządza procesem oceny."
            });
        }

        var employeeUser1 = await EnsureUserWithRole(
            userManager,
            userName: "employee.demo1",
            email: "employee.demo1@mail.com",
            password: "Employee123!",
            role: "Employee");

        var employeeUser2 = await EnsureUserWithRole(
            userManager,
            userName: "employee.demo2",
            email: "employee.demo2@mail.com",
            password: "Employee123!",
            role: "Employee");

        var employee1 = new Employee
        {
            FirstName = "Jan",
            LastName = "Testowy",
            Position = "Programista",
            Period = "2026-Q1",
            FinalScore = "8.5",
            AchievementsSummary = "Dowiezione kluczowe funkcje backendu.",
            IdentityUserId = employeeUser1.Id,
            Stage2Status = EvaluationStageStatus.PendingStage2
        };

        var employee2 = new Employee
        {
            FirstName = "Anna",
            LastName = "Przykładowa",
            Position = "Tester",
            Period = "2026-Q1",
            FinalScore = "7.9",
            AchievementsSummary = "Zwiększenie pokrycia testami regresji.",
            IdentityUserId = employeeUser2.Id,
            Stage2Status = EvaluationStageStatus.Stage2Approved,
            Stage2ReviewedByUserId = managerUser.Id,
            Stage2ReviewedAtUtc = DateTime.UtcNow.AddDays(-2),
            Stage2Comment = "Ocena zatwierdzona przez komisję."
        };

        var existingEmployee1 = await db.Employees.FirstOrDefaultAsync(e => e.IdentityUserId == employeeUser1.Id);
        if (existingEmployee1 == null)
        {
            db.Employees.Add(employee1);
        }
        else
        {
            existingEmployee1.FirstName = employee1.FirstName;
            existingEmployee1.LastName = employee1.LastName;
            existingEmployee1.Position = employee1.Position;
            existingEmployee1.Period = employee1.Period;
            existingEmployee1.FinalScore = employee1.FinalScore;
            existingEmployee1.AchievementsSummary = employee1.AchievementsSummary;
            existingEmployee1.Stage2Status = EvaluationStageStatus.PendingStage2;
            existingEmployee1.Stage2Comment = null;
            existingEmployee1.Stage2ReviewedByUserId = null;
            existingEmployee1.Stage2ReviewedAtUtc = null;
        }

        var existingEmployee2 = await db.Employees.FirstOrDefaultAsync(e => e.IdentityUserId == employeeUser2.Id);
        if (existingEmployee2 == null)
        {
            db.Employees.Add(employee2);
        }
        else
        {
            existingEmployee2.FirstName = employee2.FirstName;
            existingEmployee2.LastName = employee2.LastName;
            existingEmployee2.Position = employee2.Position;
            existingEmployee2.Period = employee2.Period;
            existingEmployee2.FinalScore = employee2.FinalScore;
            existingEmployee2.AchievementsSummary = employee2.AchievementsSummary;
            existingEmployee2.Stage2Status = employee2.Stage2Status;
            existingEmployee2.Stage2Comment = employee2.Stage2Comment;
            existingEmployee2.Stage2ReviewedByUserId = employee2.Stage2ReviewedByUserId;
            existingEmployee2.Stage2ReviewedAtUtc = employee2.Stage2ReviewedAtUtc;
        }

        await db.SaveChangesAsync();

        var employee1Id = existingEmployee1?.Id ?? employee1.Id;
        var employee2Id = existingEmployee2?.Id ?? employee2.Id;

        if (!await db.Achievements.AnyAsync(a => a.EmployeeId == employee1Id))
        {
            db.Achievements.Add(new Achievement
            {
                Name = "Refaktoryzacja API",
                Description = "Usprawnienie endpointów i walidacji danych.",
                Date = DateTime.UtcNow.AddDays(-20),
                Category = AchievementCategory.ProcessImprovement,
                EmployeeId = employee1Id,
                Stage2Status = EvaluationStageStatus.PendingStage2
            });
        }

        if (!await db.Achievements.AnyAsync(a => a.EmployeeId == employee2Id))
        {
            db.Achievements.Add(new Achievement
            {
                Name = "Automatyzacja testów",
                Description = "Dodanie testów integracyjnych dla krytycznych ścieżek.",
                Date = DateTime.UtcNow.AddDays(-14),
                Category = AchievementCategory.TechnicalGrowth,
                EmployeeId = employee2Id,
                Stage2Status = EvaluationStageStatus.Stage2Approved,
                Stage2Comment = "Zatwierdzone wraz z oceną."
            });
        }

        await db.SaveChangesAsync();
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