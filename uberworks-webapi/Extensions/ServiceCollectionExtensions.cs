// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Mantiene Program.cs limpio agrupando la configuración de arranque en dos
//           métodos de extensión: AddJwtAuthentication() configura cómo ASP.NET Core
//           valida los tokens JWT que llegan en cada petición (firma, emisor, audiencia,
//           expiración); AddApplicationServices() registra en el contenedor de Inyección
//           de Dependencias (DI) TODOS los Repository/Service de la app — es el único
//           lugar donde se conecta "cuando alguien pida IUserRepository, dale un
//           UserRepository nuevo". Sin este registro, la app no sabría qué clase concreta
//           usar detrás de cada interface.
// Entidades relacionadas: Todas (este archivo cablea Repository/Service de cada una)
// Tablas relacionadas: Ninguna directamente (es configuración de arranque, no acceso a datos)
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
            ?? throw new InvalidOperationException("Falta configurar Jwt:SecretKey en appsettings.");

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

        // Repositories/Services de las demás entidades se registran aquí
        // conforme se vayan implementando.

        return services;
    }
}
