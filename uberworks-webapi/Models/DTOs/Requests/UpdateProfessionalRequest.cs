// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe el body de PUT /api/professionals/{id} — actualiza descripción,
//           experiencia, disponibilidad y ubicación. AverageRating no está aquí a
//           propósito: no es algo que el profesional pueda editar manualmente, se calculará
//           a partir de Review.cs.
// Entidades relacionadas: Professional.cs (indirectamente, vía ProfessionalService.UpdateAsync)
// Tablas relacionadas: Ninguna directamente (TBL_PROFESSIONALS se actualiza desde
//                       ProfessionalService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class UpdateProfessionalRequest
{
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
}
