// =====================================================================================
// FILE SUMMARY
// What it does: Describes exactly what fields uberworks-webapp must send in the body of
//               POST /api/users/external-login, after Google or Facebook has already
//               verified the user's identity and handed the WebApp a verified email. This
//               endpoint is protected by RequireInternalSecretAttribute.cs (not JWT), since
//               there is no JWT yet — the whole point of the call is to obtain one.
//               ProviderUserId is only populated for Facebook (its numeric user ID, saved on
//               User.FacebookId to link the account) — Google sign-in never sets it, since
//               Google matching is done purely by verified email.
// Entities connected: User.cs (UserService.ExternalLoginAsync converts this into a User,
//                      creating one automatically as Role=Client if the email is new)
// Tables related: None directly — only reaches TBL_USERS after passing through UserService.cs
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Requests;

public class ExternalLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public AuthProvider Provider { get; set; } = AuthProvider.Google;
    public string? ProviderUserId { get; set; }
}
