// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/ExternalLoginRequest.cs —
//               the JSON shape sent to POST /api/users/external-login, once Google or
//               Facebook has already verified the user's email (see Program.cs's
//               AddGoogle()/AddFacebook().Events.OnCreatingTicket). ProviderUserId is only
//               set for Facebook (its numeric user ID) — Google sign-in leaves it null.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class ExternalLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public AuthProvider Provider { get; set; } = AuthProvider.Google;
    public string? ProviderUserId { get; set; }
}
