// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe lo que la API devuelve al consultar un Service. Es EL DTO donde vive
//           la regla de privacidad de ubicación: Latitude/Longitude/ExactAddress son
//           nullable (?) y ServiceService.cs decide en tiempo real si llenarlos o dejarlos
//           en null según quién esté preguntando (ver Services/ServiceService.cs). Zone
//           siempre viaja llena, incluso en el listado público.
// Entidades relacionadas: Service.cs, WorkType.cs (ServiceService.cs mapea de ahí)
// Tablas relacionadas: Ninguna directamente — es la "forma pública" (con reglas de
//                       privacidad) de una fila de TBL_SERVICES
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class ServiceResponse
{
    public int Id { get; set; }
    public int WorkTypeId { get; set; }
    public string WorkTypeName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? ProposedPrice { get; set; }
    public ServiceStatus Status { get; set; }
    public DateTime RequestDate { get; set; }

    /// <summary>Siempre visible, incluso en el listado público de servicios abiertos.</summary>
    public string Zone { get; set; } = string.Empty;

    /// <summary>
    /// Solo se llenan cuando quien pide el recurso es el cliente dueño o el profesional
    /// cuya propuesta fue aceptada. En cualquier otro caso quedan en null.
    /// </summary>
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ExactAddress { get; set; }

    public string? CompletionPhotoUrl { get; set; }
    public DateTime? ClientConfirmedCompletionAt { get; set; }
    public DateTime? ProfessionalConfirmedCompletionAt { get; set; }
}
