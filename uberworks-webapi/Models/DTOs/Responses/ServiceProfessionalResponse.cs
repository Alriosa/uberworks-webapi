// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe lo que la API devuelve al consultar una propuesta (ServiceProfessional).
//           Incluye el nombre y la calificación promedio del profesional "aplanados" aquí,
//           para que el cliente pueda comparar propuestas sin llamadas adicionales.
// Entidades relacionadas: ServiceProfessional.cs, Professional.cs, User.cs
//                          (ServiceProfessionalService.cs mapea de ahí)
// Tablas relacionadas: Ninguna directamente — es la "forma pública" combinada de
//                       TBL_SERVICE_PROFESSIONALS + TBL_PROFESSIONALS + TBL_USERS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class ServiceProfessionalResponse
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int ProfessionalId { get; set; }
    public string ProfessionalFirstName { get; set; } = string.Empty;
    public string ProfessionalLastName { get; set; } = string.Empty;
    public decimal ProfessionalAverageRating { get; set; }

    public decimal? NegotiatedPrice { get; set; }
    public int EstimatedArrivalMinutes { get; set; }
    public DateTime? ArrivalConfirmedAt { get; set; }
    public ServiceProfessionalStatus Status { get; set; }
}
