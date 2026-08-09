// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/AdminUserListItemResponse.cs
//               — one row per user, every non-sensitive attribute, returned by
//               GET /api/users. Backs the "Ver Todos los Usuarios" directory on
//               Views/Dashboard/MasterAdmin.cshtml.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class AdminUserListItemResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string? FacebookId { get; set; }
    public bool IsPasswordSet { get; set; }
}
