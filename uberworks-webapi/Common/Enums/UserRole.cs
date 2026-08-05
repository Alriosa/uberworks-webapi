// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Define los 4 roles posibles de un usuario en el sistema. C# no permite guardar
//           texto libre "seguro" en una propiedad, así que en vez de usar un string suelto
//           (donde alguien podría escribir "CLiente" o "cliente" y romper todo), se usa un
//           enum: una lista cerrada de opciones válidas que el compilador conoce y valida.
// Entidades relacionadas: User.cs (la propiedad User.Role es de este tipo)
// Tablas relacionadas: TBL_USERS.CL_ROLE (el valor de C# se traduce a texto en
//                       Data/Configurations/UserConfiguration.cs antes de guardarse)
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Mapea al CHECK constraint de TBL_USERS.CL_ROLE.
///
/// MasterAdmin: cuenta única, sembrada directo en la base de datos al arrancar la API
/// (ver Data/Seed/MasterAdminSeeder.cs) — nunca se puede crear vía /api/users/register.
/// Admin: administradores normales con permisos delegados por el MasterAdmin
/// (crear/borrar cuentas, notificaciones globales, etc. — se define más adelante).
/// Tampoco se puede crear vía /register público.
/// </summary>
public enum UserRole
{
    MasterAdmin,
    Admin,
    Client,
    Professional
}
