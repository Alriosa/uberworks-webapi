// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe lo que la API devuelve al consultar un Professional. Incluye
//           FirstName/LastName/Email del User dueño del perfil "aplanados" aquí mismo,
//           para que el cliente (webapp/mobile) no tenga que hacer una segunda llamada a
//           /api/users/{id} solo para mostrar el nombre.
// Entidades relacionadas: Professional.cs, User.cs (ProfessionalService.cs mapea de ahí)
// Tablas relacionadas: Ninguna directamente — es la "forma pública" combinada de
//                       TBL_PROFESSIONALS + TBL_USERS
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class ProfessionalResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Datos básicos del User embebidos, para no obligar al consumidor a hacer un segundo llamado.
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? Availability { get; set; }
    public string? Location { get; set; }
    public decimal AverageRating { get; set; }
}
