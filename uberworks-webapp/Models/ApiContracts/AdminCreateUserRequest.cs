// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/AdminCreateUserRequest.cs —
//               the JSON shape sent to POST /api/users/admin-create. Unlike RegisterUserRequest.cs,
//               Role here can be Admin (never MasterAdmin — the API rejects that regardless
//               of what gets sent). No Password field — the API creates the account with
//               IsPasswordSet=false and emails the new user a "set your password" link
//               instead (see UserService.SendPasswordSetupLinkAsync on the API side).
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class AdminCreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
}
