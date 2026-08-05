// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Representa el "perfil profesional" que un User con Role=Professional puede
//           crear (relación 1 a 1: cada Professional le pertenece a exactamente un User).
//           Guarda su descripción, experiencia, disponibilidad, ubicación y calificación
//           promedio (que se calculará a partir de Review.cs más adelante).
// Entidades relacionadas: User.cs (1:1, dueño del perfil), ServiceProfessional.cs (1:N,
//                          sus propuestas a distintos Services), Review.cs (1:N),
//                          Chat.cs (1:N)
// Tablas relacionadas: TBL_PROFESSIONALS (mapeo en Data/Configurations/ProfessionalConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_PROFESSIONALS. Extiende a User en relación 1:1.
/// </summary>
public class Professional
{
    public int Id { get; set; }

    /// <summary>
    /// FK 1:1 hacia User (PK_USER_ID en el diagrama).
    /// </summary>
    public int UserId { get; set; }

    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
    public decimal AverageRating { get; set; }

    // Navegaciones
    public User User { get; set; } = null!;
    public ICollection<ServiceProfessional> ServiceProfessionals { get; set; } = new List<ServiceProfessional>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
}
