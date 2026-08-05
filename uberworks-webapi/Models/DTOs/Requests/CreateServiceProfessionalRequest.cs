// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe el body de POST /api/services/{serviceId}/proposals — lo que un
//           profesional manda al ofertar por un Service (precio negociado + minutos
//           estimados de llegada). ServiceId sale de la ruta y ProfessionalId del JWT,
//           ninguno de los dos va en este DTO.
// Entidades relacionadas: ServiceProfessional.cs (indirectamente, vía
//                          ServiceProfessionalService.CreateProposalAsync)
// Tablas relacionadas: Ninguna directamente (TBL_SERVICE_PROFESSIONALS se llena desde
//                       ServiceProfessionalService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

// El ServiceId viene de la ruta y el ProfessionalId se resuelve del usuario autenticado (JWT).
public class CreateServiceProfessionalRequest
{
    public decimal? NegotiatedPrice { get; set; }
    public int EstimatedArrivalMinutes { get; set; }
}
