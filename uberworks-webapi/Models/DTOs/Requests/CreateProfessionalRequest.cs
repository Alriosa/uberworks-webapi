// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe el body de POST /api/professionals. A propósito NO incluye UserId:
//           ese dato se saca del JWT del usuario que hace la petición (vía
//           ICurrentUserService), nunca de lo que el cliente mande, para que nadie pueda
//           crear un perfil profesional a nombre de otra persona.
// Entidades relacionadas: Professional.cs (indirectamente, vía ProfessionalService.CreateAsync)
// Tablas relacionadas: Ninguna directamente (TBL_PROFESSIONALS se llena desde
//                       ProfessionalService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

// El UserId ya NO va aquí: se toma del usuario autenticado (JWT), nunca de lo que
// mande el cliente en el body, para que nadie pueda crear un perfil a nombre de otro.
public class CreateProfessionalRequest
{
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
}
