// =====================================================================================
// FILE SUMMARY
// What it does: Keeps Program.cs clean by grouping startup configuration into two
//               extension methods: AddJwtAuthentication() configures how ASP.NET Core
//               validates the JWT tokens that arrive on every request (signature, issuer,
//               audience, expiration); AddApplicationServices() registers EVERY
//               Repository/Service in the Dependency Injection (DI) container — it's the
//               only place where "when someone asks for IUserRepository, give them a new
//               UserRepository" gets wired up. Without this registration, the app wouldn't
//               know which concrete class to use behind each interface.
// Entities connected: All of them (this file wires up the Repository/Service for each one)
// Tables related: None directly (it's startup configuration, not data access)
// =====================================================================================
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

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
}
