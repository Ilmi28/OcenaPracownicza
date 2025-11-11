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



namespace OcenaPracownicza;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddScoped<IDocumentGeneratorService, DocumentGeneratorService>();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "OcenaPracownicza.API",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Podaj token JWT bez prefiksu 'Bearer', np.: eyJhbGciOi...",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        Env.Load();

        var jwtSection = builder.Configuration.GetSection("JwtSettings");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var secret = jwtSection["Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/login-google";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            })
            .AddGoogle(options =>
            {
                var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");

                if (string.IsNullOrEmpty(clientId))
                    throw new ArgumentNullException(nameof(clientId), "Google ClientId is missing in configuration");
                if (string.IsNullOrEmpty(clientSecret))
                    throw new ArgumentNullException(nameof(clientSecret), "Google ClientSecret is missing in configuration");

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.CallbackPath = "/signin-google";

                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.SaveTokens = true;

                options.Events.OnCreatingTicket = async context =>
                {
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                        var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);

                        if (!response.IsSuccessStatusCode)
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"[Google OAuth] Błąd pobierania danych: {response.StatusCode}");
                            Console.WriteLine($"[Google OAuth] Treść błędu: {errorContent}");
                            return;
                        }

                        var user = await response.Content.ReadFromJsonAsync<JsonElement>();
                        Console.WriteLine($"[Google OAuth] Dane użytkownika: {user}");

                        if (user.TryGetProperty("email", out var emailElement) && emailElement.ValueKind == JsonValueKind.String)
                        {
                            var email = emailElement.GetString();
                            if (!string.IsNullOrEmpty(email))
                            {
                                context.Identity?.AddClaim(new Claim(ClaimTypes.Email, email));
                                Console.WriteLine($"[Google OAuth] Email claim dodany: {email}");
                            }
                        }

                        if (user.TryGetProperty("name", out var nameElement) && nameElement.ValueKind == JsonValueKind.String)
                        {
                            var name = nameElement.GetString();
                            if (!string.IsNullOrEmpty(name))
                            {
                                context.Identity?.AddClaim(new Claim(ClaimTypes.Name, name));
                                Console.WriteLine($"[Google OAuth] Name claim dodany: {name}");
                            }
                        }

                        if (user.TryGetProperty("picture", out var pictureElement) && pictureElement.ValueKind == JsonValueKind.String)
                        {
                            var picture = pictureElement.GetString();
                            if (!string.IsNullOrEmpty(picture))
                            {
                                context.Identity?.AddClaim(new Claim("picture", picture));
                            }
                        }

                        if (user.TryGetProperty("sub", out var subElement) && subElement.ValueKind == JsonValueKind.String)
                        {
                            var sub = subElement.GetString();
                            if (!string.IsNullOrEmpty(sub))
                            {
                                context.Identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Google OAuth] Wyjątek w OnCreatingTicket: {ex.Message}");
                        Console.WriteLine($"[Google OAuth] Stack trace: {ex.StackTrace}");
                    }
                };
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(
                            new UnauthorizedProblemDetails("Musisz być zalogowany.")
                        );
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(
                            new ForbiddenProblemDetails("Nie masz dostępu do zasobu.")
                        );
                    },
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("jwt"))
                            context.Token = context.Request.Cookies["jwt"];
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IExampleRepository, ExampleRepository>();
        builder.Services.AddScoped<IExampleService, ExampleService>();

        builder.Services.AddAuthorization();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowCredentials()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

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

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGet("/", context =>
        {
            context.Response.Redirect("/swagger");
            return Task.CompletedTask;
        });

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
