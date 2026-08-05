// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Se ejecuta UNA vez, cada vez que la API arranca (llamado desde Program.cs).
//           Revisa si ya existe algún usuario con Role=MasterAdmin en la base de datos;
//           si no existe ninguno, crea uno usando el email/password que estén configurados
//           en appsettings.json (o mejor, en "dotnet user-secrets") bajo la sección
//           "MasterAdmin". Así la única cuenta con máximo privilegio nunca pasa por el
//           endpoint público de registro (que la rechaza explícitamente, ver
//           Services/UserService.cs → RegisterAsync).
// Entidades relacionadas: User.cs (crea una fila con Role = UserRole.MasterAdmin)
// Tablas relacionadas: TBL_USERS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Helpers;
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Data.Seed;

/// <summary>
/// Siembra la única cuenta MasterAdmin al arrancar la API, si todavía no existe ninguna.
/// Nunca se crea vía /api/users/register — las credenciales salen de configuración
/// (appsettings / user secrets / variables de entorno), nunca de código fuente.
/// </summary>
public static class MasterAdminSeeder
{
    public static async Task SeedAsync(AppDbContext context, IConfiguration configuration, ILogger logger)
    {
        var alreadyExists = await context.Users.AnyAsync(u => u.Role == UserRole.MasterAdmin);
        if (alreadyExists)
        {
            return;
        }

        var email = configuration["MasterAdmin:Email"];
        var password = configuration["MasterAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "No existe ninguna cuenta MasterAdmin y no se configuró MasterAdmin:Email / " +
                "MasterAdmin:Password. No se sembró la cuenta maestra.");
            return;
        }

        var masterAdmin = new User
        {
            FirstName = "Master",
            LastName = "Admin",
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            Role = UserRole.MasterAdmin,
            Status = UserStatus.Active
        };

        context.Users.Add(masterAdmin);
        await context.SaveChangesAsync();

        logger.LogInformation("Cuenta MasterAdmin sembrada para {Email}.", email);
    }
}
