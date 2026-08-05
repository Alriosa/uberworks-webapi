// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Guarda la calificación mutua después de un Service — el cliente califica al
//           profesional y viceversa (1 a 5), más un comentario. Todavía no tiene
//           Repository/Service/Controller construidos (es de las últimas piezas
//           pendientes del ciclo de vida de un Service).
// Entidades relacionadas: Professional.cs (N:1), Service.cs (N:1), User.cs (N:1, como Client)
// Tablas relacionadas: TBL_REVIEWS (mapeo en Data/Configurations/ReviewConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_REVIEWS.
/// </summary>
public class Review
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public int ServiceId { get; set; }
    public int ClientId { get; set; }

    /// <summary>Calificación que el cliente le da al profesional (1-5).</summary>
    public byte? ClientRating { get; set; }

    /// <summary>Calificación que el profesional le da al cliente (1-5).</summary>
    public byte? ProfessionalRating { get; set; }

    public string? Comment { get; set; }
    public DateTime ReviewDate { get; set; }

    // Navegaciones
    public Professional Professional { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public User Client { get; set; } = null!;
}
