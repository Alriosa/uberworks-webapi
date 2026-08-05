// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Describe el body de PUT /api/users/{id} — solo permite cambiar nombre,
//           apellido y teléfono (no email, password ni rol, esos requieren flujos aparte).
// Entidades relacionadas: User.cs (indirectamente, vía UserService.UpdateAsync)
// Tablas relacionadas: Ninguna directamente (TBL_USERS se actualiza desde UserService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
