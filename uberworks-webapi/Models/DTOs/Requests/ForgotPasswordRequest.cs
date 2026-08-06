// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/users/forgot-password. Just an email —
//               UserService.ForgotPasswordAsync always returns the same generic success
//               response whether or not that email exists in TBL_USERS, so this endpoint
//               can never be used to check which emails are registered.
// Entities connected: None directly
// Tables related: None directly
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}
