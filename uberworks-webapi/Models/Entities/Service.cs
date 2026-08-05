// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Es el "Work Post" que describiste — lo que un cliente publica cuando necesita
//           un servicio (ej. "necesito que reparen el refrigerador"). Guarda la ubicación
//           GPS exacta y la dirección (privadas, solo visibles para el dueño y el
//           profesional aceptado), la Zona pública (ej. "Granadilla", visible para todos),
//           y los datos de cierre del trabajo (foto de evidencia + confirmación de ambas
//           partes). Toda la lógica de "quién puede ver qué" vive en Services/ServiceService.cs,
//           no aquí — esta clase solo describe la forma de los datos.
// Entidades relacionadas: WorkType.cs (N:1), User.cs (N:1, como Client),
//                          ServiceProfessional.cs (1:N, las propuestas que recibe),
//                          Review.cs (1:N), Payment.cs (1:N)
// Tablas relacionadas: TBL_SERVICES (mapeo en Data/Configurations/ServiceConfiguration.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_SERVICES (solicitud de servicio creada por un cliente).
/// </summary>
public class Service
{
    public int Id { get; set; }
    public int WorkTypeId { get; set; }
    public int ClientId { get; set; }

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? ProposedPrice { get; set; }
    public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
    public DateTime RequestDate { get; set; }

    // Ubicación: la dirección exacta es privada, solo se expone al dueño (cliente)
    // y al profesional cuya propuesta fue aceptada. La Zona es pública desde el inicio.
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string ExactAddress { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;

    // Cierre del trabajo: evidencia + confirmación mutua.
    public string? CompletionPhotoUrl { get; set; }
    public DateTime? ClientConfirmedCompletionAt { get; set; }
    public DateTime? ProfessionalConfirmedCompletionAt { get; set; }

    // Navegaciones
    public WorkType WorkType { get; set; } = null!;
    public User Client { get; set; } = null!;
    public ICollection<ServiceProfessional> ServiceProfessionals { get; set; } = new List<ServiceProfessional>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
