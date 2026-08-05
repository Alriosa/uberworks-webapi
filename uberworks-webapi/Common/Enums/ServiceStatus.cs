// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Define en qué etapa está un "Work Post" (Service): recién creado (Pending),
//           con un profesional ya aceptado (Accepted), cancelado, o cerrado (Completed,
//           que solo se alcanza cuando cliente y profesional confirman ambos por separado).
// Entidades relacionadas: Service.cs (la propiedad Service.Status es de este tipo)
// Tablas relacionadas: TBL_SERVICES.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Mapea a los valores usados en TBL_SERVICES.CL_STATUS.
/// </summary>
public enum ServiceStatus
{
    Pending,
    Accepted,
    Cancelled,
    Completed
}
