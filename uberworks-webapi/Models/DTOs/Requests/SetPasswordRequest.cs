// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/users/set-password. Unlike
//               ResetPasswordRequest.cs (which proves identity via an emailed token, for
//               someone who can't log in), this endpoint requires [Authorize] — the caller
//               already has a valid JWT (they just signed in via Google/Facebook), so their
//               own id comes from that token, not from the request body. Used by
//               UserService.SetPasswordAsync to let a Google/Facebook-created account (see
//               User.IsPasswordSet) set a real password for the first time.
// Entities connected: None directly
// Tables related: None directly
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class SetPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}
