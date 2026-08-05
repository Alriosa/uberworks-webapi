// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Define en qué estado está un pago (pendiente, retenido en garantía, liberado
//           al profesional). Igual que PaymentMethod, existe desde el diagrama original
//           pero todavía no tiene Repository/Service/Controller construidos.
// Entidades relacionadas: Payment.cs (pendiente de implementar)
// Tablas relacionadas: TBL_PAYMENTS.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Mapea a los valores usados en TBL_PAYMENTS.CL_STATUS.
/// </summary>
public enum PaymentStatus
{
    Pending,
    Held,
    Released
}
