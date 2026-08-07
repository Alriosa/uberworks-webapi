// =====================================================================================
// FILE SUMMARY
// What it does: Keeps Program.cs clean by grouping startup configuration into three
//               extension methods: AddJwtAuthentication() configures how ASP.NET Core
//               validates the JWT tokens that arrive on every request (signature, issuer,
//               audience, expiration); AddApplicationServices() registers IDbConnectionFactory
//               (the one place the connection string is read — see Data/SqlConnectionFactory.cs)
//               plus EVERY Repository/Service in the Dependency Injection (DI) container —
//               it's the only place where "when someone asks for IUserRepository, give them
//               a new UserRepository" gets wired up; AddRateLimiting() caps how many
//               requests a single caller can make per minute, to stop mass scraping/
//               enumeration of public endpoints (see uberworks_webapi.Program for how it's
//               wired into the pipeline).
// Entities connected: All of them (this file wires up the Repository/Service for each one)
// Tables related: None directly (it's startup configuration, not data access)
// =====================================================================================
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using uberworks_webapi.Data;
using uberworks_webapi.Repositories;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey is not configured in appsettings.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Singleton is fine here (unlike EF Core's AddDbContext, which needed Scoped for
        // its per-request change tracking): SqlConnectionFactory.cs holds nothing but a
        // connection string and hands out a brand-new IDbConnection on every call.
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
        services.AddScoped<IProfessionalService, ProfessionalService>();

        services.AddScoped<IWorkTypeRepository, WorkTypeRepository>();
        services.AddScoped<IWorkTypeService, WorkTypeService>();

        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IServiceService, ServiceService>();

        services.AddScoped<IServiceProfessionalRepository, ServiceProfessionalRepository>();
        services.AddScoped<IServiceProfessionalService, ServiceProfessionalService>();

        // Repositories/Services for the remaining entities are registered here
        // as they get implemented.

        return services;
    }

    /// <summary>
    /// Caps requests per caller (100/minute) to make bulk scraping/enumeration impractical
    /// (e.g. someone looping over ids on a public endpoint). Logged-in callers are keyed by
    /// their user id (via the JWT claim), so their limit follows them across IPs; anonymous
    /// callers are keyed by IP address. Exceeding the limit returns HTTP 429.
    /// </summary>
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var partitionKey = httpContext.User.Identity?.IsAuthenticated == true
                    ? $"user:{httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}"
                    : $"ip:{httpContext.Connection.RemoteIpAddress}";

                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });
        });

        return services;
    }
}
