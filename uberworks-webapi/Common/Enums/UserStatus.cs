// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Define los estados posibles de una cuenta de usuario (activa, suspendida,
//           penalizada). Se usa para poder bloquear el acceso de alguien sin borrar su
//           cuenta ni su historial (ej. si un Admin lo suspende por mal comportamiento).
// Entidades relacionadas: User.cs (la propiedad User.Status es de este tipo)
// Tablas relacionadas: TBL_USERS.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Mapea a los valores usados en TBL_USERS.CL_STATUS.
/// </summary>
public enum UserStatus
{
    Active,
    Suspended,
    Penalized
}
