// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Es la "propuesta" de un profesional sobre un Service — cada profesional que
//           oferta a un mismo Service tiene su propia fila aquí, con su propio precio,
//           minutos estimados de llegada, y estado (en negociación / aceptada / rechazada /
//           en curso / completada). Cuando el cliente acepta una propuesta, esta fila pasa
//           a Accepted y las demás propuestas del mismo Service se rechazan automáticamente
//           (ver Services/ServiceProfessionalService.cs → AcceptProposalAsync).
// Entidades relacionadas: Professional.cs (N:1), Service.cs (N:1)
// Tablas relacionadas: TBL_SERVICE_PROFESSIONALS
//                       (mapeo en Data/Configurations/ServiceProfessionalConfiguration.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_SERVICE_PROFESSIONALS (relación N:M entre Service y Professional,
/// representa la negociación/postulación de un profesional a un servicio).
/// </summary>
public class ServiceProfessional
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public int ServiceId { get; set; }

    public decimal? NegotiatedPrice { get; set; }

    /// <summary>Minutos que el profesional declara que tardará en llegar, al momento de proponer.</summary>
    public int EstimatedArrivalMinutes { get; set; }

    /// <summary>
    /// Timestamp (generado por el servidor, nunca por el celular del profesional) de cuando
    /// presionó "Estoy en el sitio". Solo aplica al profesional aceptado.
    /// </summary>
    public DateTime? ArrivalConfirmedAt { get; set; }

    public ServiceProfessionalStatus Status { get; set; } = ServiceProfessionalStatus.UnderNegotiation;

    // Navegaciones
    public Professional Professional { get; set; } = null!;
    public Service Service { get; set; } = null!;
}
