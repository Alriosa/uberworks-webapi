// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/AdminUpdateUserRequest.cs —
//               the body PUT /api/users/{id}/admin-update expects. Backs the Admin
//               dashboard's "Editar Usuario" modal, which edits every field
//               (Username/Email/Role/Status included), unlike UpdateUserRequest.cs
//               (self-service, FirstName/LastName/Phone only).
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class AdminUpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
}
