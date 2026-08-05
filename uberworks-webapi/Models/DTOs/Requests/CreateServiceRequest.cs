// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe el body de POST /api/services — lo que un cliente manda al crear un
//           "Work Post". Incluye ubicación tipo "compartir ubicación de WhatsApp"
//           (Latitude/Longitude + ExactAddress) más la Zona pública. El ClientId NO está
//           aquí: se saca del JWT, igual que en CreateProfessionalRequest.cs.
// Entidades relacionadas: Service.cs (indirectamente, vía ServiceService.CreateAsync)
// Tablas relacionadas: Ninguna directamente (TBL_SERVICES se llena desde ServiceService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class CreateServiceRequest
{
    public int WorkTypeId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? ProposedPrice { get; set; }

    // Ubicación tipo "compartir ubicación" (GPS). ExactAddress y las coordenadas
    // nunca se muestran públicamente — solo Zone es visible antes de aceptar una propuesta.
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string ExactAddress { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
}
