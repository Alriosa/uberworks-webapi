// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/ExternalLoginRequest.cs —
//               the JSON shape sent to POST /api/users/external-login, once Google itself
//               has already verified the user's email (see Program.cs's
//               AddGoogle().Events.OnCreatingTicket).
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class ExternalLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
