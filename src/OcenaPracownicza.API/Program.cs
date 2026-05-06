using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Entities;
using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Extensions;
using OcenaPracownicza.API.Interfaces.Services;

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

        var currentPeriod = await SeedEvaluationPeriodsAsync(db);

        await SeedSampleDataAsync(db, userManager, currentPeriod);
    }

    private static async Task<EvaluationPeriod> SeedEvaluationPeriodsAsync(ApplicationDbContext db)
    {
        var period = await db.EvaluationPeriods.FirstOrDefaultAsync(p => p.Name == "Okres 2026-2028");

        if (period == null)
        {
            period = new EvaluationPeriod
            {
                Name = "Okres 2026-2028",
                StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2027, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                RegulationVersion = "Zarządzenie 114/2024",
                IsClosed = false
            };
            db.EvaluationPeriods.Add(period);
            await db.SaveChangesAsync();
        }
        return period;
    }

    private static async Task SeedSampleDataAsync(ApplicationDbContext db, UserManager<IdentityUser> userManager, EvaluationPeriod period)
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

        var employeeUser1 = await EnsureUserWithRole(userManager, "employee.demo1", "employee.demo1@mail.com", "Employee123!", "Employee");
        var employeeUser2 = await EnsureUserWithRole(userManager, "employee.demo2", "employee.demo2@mail.com", "Employee123!", "Employee");

        var emp1 = await GetOrCreateEmployee(db, employeeUser1.Id, "Jan", "Testowy", "Programista");
        var emp2 = await GetOrCreateEmployee(db, employeeUser2.Id, "Anna", "Przykładowa", "Tester");

        await db.SaveChangesAsync();

        if (!await db.Achievements.AnyAsync(a => a.EmployeeId == emp1.Id && a.Name == "Refaktoryzacja API"))
        {
            db.Achievements.Add(new Achievement
            {
                Name = "Refaktoryzacja API",
                Description = "Usprawnienie endpointów i walidacji danych.",
                Date = DateTime.UtcNow.AddDays(-20),
                Category = AchievementCategory.ProcessImprovement,
                EmployeeId = emp1.Id,
                EvaluationPeriodId = period.Id,     
                FinalScore = "8.5",
                AchievementsSummary = "Dowiezione kluczowe funkcje backendu.",
                Stage2Status = EvaluationStageStatus.PendingStage2
            });
        }

        if (!await db.Achievements.AnyAsync(a => a.EmployeeId == emp2.Id && a.Name == "Automatyzacja testów"))
        {
            db.Achievements.Add(new Achievement
            {
                Name = "Automatyzacja testów",
                Description = "Dodanie testów integracyjnych dla krytycznych ścieżek.",
                Date = DateTime.UtcNow.AddDays(-14),
                Category = AchievementCategory.TechnicalGrowth,
                EmployeeId = emp2.Id,
                EvaluationPeriodId = period.Id,     
                FinalScore = "7.9",
                AchievementsSummary = "Zwiększenie pokrycia testami regresji.",
                Stage2Status = EvaluationStageStatus.Stage2Approved,
                Stage2Comment = "Zatwierdzone wraz z oceną.",
                Stage2ReviewedByUserId = managerUser.Id,
                Stage2ReviewedAtUtc = DateTime.UtcNow.AddDays(-2)
            });
        }

        await db.SaveChangesAsync();
    }

    private static async Task<Employee> GetOrCreateEmployee(ApplicationDbContext db, string userId, string fn, string ln, string pos)
    {
        var emp = await db.Employees.FirstOrDefaultAsync(e => e.IdentityUserId == userId);
        if (emp == null)
        {
            emp = new Employee { FirstName = fn, LastName = ln, Position = pos, IdentityUserId = userId };
            db.Employees.Add(emp);
        }
        return emp;
    }

    private static async Task EnsureRoleExists(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task<IdentityUser> EnsureUserWithRole(UserManager<IdentityUser> userManager, string userName, string email, string password, string role)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            user = new IdentityUser { UserName = userName, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded) throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
        return user;
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