// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Requests/SetPasswordRequest.cs — the
//               JSON body sent to POST /api/users/set-password, which requires a Bearer
//               token (the caller's own JWT) rather than any identifying field in the body.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class SetPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}
