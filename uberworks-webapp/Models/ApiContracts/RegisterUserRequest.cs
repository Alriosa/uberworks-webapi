// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/RegisterUserRequest.cs —
//               the JSON shape sent to POST /api/users/register. No Role field: this
//               endpoint always creates a Client (see UserRole.cs for the full
//               account-creation pyramid) — every other role can only come from
//               AdminController.CreateUser (Manager/Admin/MasterAdmin) or Google sign-in
//               auto-registration (also always Client).
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class RegisterUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
}
