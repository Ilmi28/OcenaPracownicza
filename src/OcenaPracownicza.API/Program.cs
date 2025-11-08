using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OcenaPracownicza.API.AppProblemDetails;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Interfaces.Repositories;
using OcenaPracownicza.API.Interfaces.Services;
using OcenaPracownicza.API.Repositories;
using OcenaPracownicza.API.Services;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OcenaPracownicza;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Swagger
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

        // JWT config
        var jwtSection = builder.Configuration.GetSection("JwtSettings");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var secret = jwtSection["Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        // Authentication (JWT + Cookie + Google)
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            // Cookie - MUSI BYĆ PRZED Google
            .AddCookie(options =>
            {
                options.LoginPath = "/login-google";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            })
            // Google OAuth
            .AddGoogle(options =>
            {
                var clientId = builder.Configuration["Authentication:Google:ClientId"];
                var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

                if (string.IsNullOrEmpty(clientId))
                    throw new ArgumentNullException(nameof(clientId), "Google ClientId is missing in configuration");
                if (string.IsNullOrEmpty(clientSecret))
                    throw new ArgumentNullException(nameof(clientSecret), "Google ClientSecret is missing in configuration");

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.CallbackPath = "/signin-google";

                // Scope - WAŻNA KOLEJNOŚĆ
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                options.SaveTokens = true;

                // Event do pobrania danych użytkownika
                options.Events.OnCreatingTicket = async context =>
                {
                    try
                    {
                        // Pobierz dane z Google UserInfo endpoint
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

                        // Dodaj claims
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
            // JWT Bearer - dla API endpoints
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

        // DI
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IExampleRepository, ExampleRepository>();
        builder.Services.AddScoped<IExampleService, ExampleService>();

        // Authorization
        builder.Services.AddAuthorization();

        // CORS
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

        // Global error handler
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

        // Swagger
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

        // ===== GOOGLE OAUTH ENDPOINTS =====

        // Endpoint inicjujący logowanie przez Google
        app.MapGet("/login-google", async (HttpContext context) =>
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = "/google-success",
                IsPersistent = true
            };
            await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, props);
        });

        // Endpoint debugowania (USUŃ W PRODUKCJI)
        app.MapGet("/google-debug", async (HttpContext ctx) =>
        {
            Console.WriteLine($"[Debug] IsAuthenticated: {ctx.User.Identity?.IsAuthenticated}");

            if (!ctx.User.Identity?.IsAuthenticated ?? true)
            {
                return Results.Json(new
                {
                    Error = "Not authenticated",
                    Message = "Użytkownik nie jest zalogowany"
                });
            }

            var accessToken = await ctx.GetTokenAsync("access_token");
            var idToken = await ctx.GetTokenAsync("id_token");
            var refreshToken = await ctx.GetTokenAsync("refresh_token");

            var claims = ctx.User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            }).ToList();

            Console.WriteLine($"[Debug] Claims count: {claims.Count}");
            foreach (var claim in claims)
            {
                Console.WriteLine($"[Debug] Claim: {claim.Type} = {claim.Value}");
            }

            return Results.Json(new
            {
                IsAuthenticated = ctx.User.Identity.IsAuthenticated,
                AuthenticationType = ctx.User.Identity.AuthenticationType,
                HasAccessToken = !string.IsNullOrEmpty(accessToken),
                HasIdToken = !string.IsNullOrEmpty(idToken),
                HasRefreshToken = !string.IsNullOrEmpty(refreshToken),
                ClaimsCount = claims.Count,
                Claims = claims,
                AccessTokenPreview = accessToken?.Substring(0, Math.Min(30, accessToken.Length)) + "..."
            });
        });

        // Endpoint po udanym logowaniu
        app.MapGet("/google-success", async (HttpContext ctx) =>
        {
            Console.WriteLine($"[Google Success] IsAuthenticated: {ctx.User.Identity?.IsAuthenticated}");

            if (!ctx.User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("[Google Success] Użytkownik NIE jest uwierzytelniony");
                return Results.Unauthorized();
            }

            // Pobierz claims
            var email = ctx.User.FindFirst(ClaimTypes.Email)?.Value;
            var name = ctx.User.FindFirst(ClaimTypes.Name)?.Value;
            var picture = ctx.User.FindFirst("picture")?.Value;

            Console.WriteLine($"[Google Success] Email z claims: {email}");
            Console.WriteLine($"[Google Success] Name z claims: {name}");
            Console.WriteLine($"[Google Success] Picture z claims: {picture}");

            // Fallback: Jeśli claims są puste, pobierz z API
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                Console.WriteLine("[Google Success] Claims są puste - próba pobrania z API");

                var accessToken = await ctx.GetTokenAsync("access_token");
                Console.WriteLine($"[Google Success] AccessToken exists: {!string.IsNullOrEmpty(accessToken)}");

                if (!string.IsNullOrEmpty(accessToken))
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    try
                    {
                        var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                        response.EnsureSuccessStatusCode();

                        var user = await response.Content.ReadFromJsonAsync<JsonElement>();
                        Console.WriteLine($"[Google Success] User info z API: {user}");

                        email ??= user.TryGetProperty("email", out var e) ? e.GetString() : null;
                        name ??= user.TryGetProperty("name", out var n) ? n.GetString() : null;
                        picture ??= user.TryGetProperty("picture", out var p) ? p.GetString() : null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Google Success] Błąd pobierania z API: {ex.Message}");
                    }
                }
            }

            return Results.Ok(new
            {
                Email = email,
                Name = name,
                Picture = picture
            });
        });

        app.MapControllers();

        app.Run();
    }
}
