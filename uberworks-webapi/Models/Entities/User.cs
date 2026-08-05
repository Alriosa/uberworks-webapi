// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Representa una fila de la tabla de usuarios. Es la entidad "raíz" de todo el
//           sistema — Client y Professional son en realidad Users con datos extra. EF Core
//           usa esta clase para leer/escribir en la base de datos automáticamente (nunca
//           escribimos SQL a mano: EF traduce esta clase a SELECT/INSERT/UPDATE).
// Entidades relacionadas: Professional.cs (1:1), Service.cs (1:N como cliente),
//                          Review.cs (1:N como cliente), Chat.cs (1:N como cliente),
//                          Penalty.cs (1:N), Reward.cs (1:1)
// Tablas relacionadas: TBL_USERS (mapeo completo en Data/Configurations/UserConfiguration.cs)
// =====================================================================================
//
// Sobre los "?" que ves abajo (Phone, Professional?, Reward?): son "nullable" — permiten que
// esa propiedad esté vacía (null) sin que el programa truene. Explicación completa y detallada
// al final de la respuesta del chat donde se agregó este comentario.
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Mapea a TBL_USERS.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    /// <summary>
    /// Hash de la contraseña (CL_PASSWORD). Nunca se guarda en texto plano.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime RegistrationDate { get; set; }

    // Navegaciones
    public Professional? Professional { get; set; }
    public ICollection<Service> ServicesRequested { get; set; } = new List<Service>();
    public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
    public ICollection<Chat> ChatsAsClient { get; set; } = new List<Chat>();
    public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
    public Reward? Reward { get; set; }
}
