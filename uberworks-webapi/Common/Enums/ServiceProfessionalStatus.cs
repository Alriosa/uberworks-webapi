// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Define en qué punto está la propuesta/negociación de UN profesional específico
//           sobre UN Service específico (cada profesional que ofertó tiene su propia fila
//           con su propio estado: en negociación, aceptada, rechazada, en curso, completada).
// Entidades relacionadas: ServiceProfessional.cs (la propiedad Status es de este tipo)
// Tablas relacionadas: TBL_SERVICE_PROFESSIONALS.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Mapea a los valores usados en TBL_SERVICE_PROFESSIONALS.CL_STATUS.
/// </summary>
public enum ServiceProfessionalStatus
{
    UnderNegotiation,
    Accepted,
    Rejected,
    InProgress,
    Completed
}
