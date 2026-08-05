// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Guarda el cobro asociado a un Service (método de pago, monto, estado de
//           retención/liberación). Todavía no tiene Repository/Service/Controller
//           construidos — es de las piezas pendientes del ciclo de vida de un Service.
// Entidades relacionadas: Service.cs (N:1)
// Tablas relacionadas: TBL_PAYMENTS (mapeo en Data/Configurations/PaymentConfiguration.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_PAYMENTS.
/// </summary>
public class Payment
{
    public int Id { get; set; }
    public int ServiceId { get; set; }

    public PaymentMethod? Method { get; set; }
    public decimal? Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime PaymentDate { get; set; }

    // Navegaciones
    public Service Service { get; set; } = null!;
}
