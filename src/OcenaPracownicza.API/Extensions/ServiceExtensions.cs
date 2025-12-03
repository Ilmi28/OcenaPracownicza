using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocenapracownicza.API.Services;
using OcenaPracownicza.API.AppProblemDetails;
using OcenaPracownicza.API.Data;
using OcenaPracownicza.API.Data.Identity;
using OcenaPracownicza.API.Interfaces.Other;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Repositories;
using OcenaPracownicza.API.Services;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OcenaPracownicza.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IDocumentGeneratorService, DocumentGeneratorService>();
            services.AddScoped<IExampleService, ExampleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAdminService, AdminService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IExampleRepository, ExampleRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IAdminRepository, AdminRepository>();
        }

        public static void AddAppDbContextWithIdentity(this IServiceCollection services, IConfiguration config)
        {
            Console.WriteLine(config.GetConnectionString("DefaultConnection"));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        }

        public static void AddCorsWithPolicies(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
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
        }

        public static void AddAuthenticationWithGoogle(this IServiceCollection services, IConfiguration config)
        {
            var jwtSection = config.GetSection("JwtSettings");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var secret = jwtSection["Secret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
        }
    }
}
