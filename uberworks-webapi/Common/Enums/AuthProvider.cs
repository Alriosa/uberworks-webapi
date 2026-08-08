// =====================================================================================
// FILE SUMMARY
// What it does: Identifies which external identity provider a login/registration came
//               through. Used by ExternalLoginRequest.cs and UserService.ExternalLoginAsync
//               to tell a Google sign-in apart from a Facebook sign-in — Facebook additionally
//               carries a ProviderUserId (its own numeric user ID) that gets saved on
//               User.FacebookId, since Facebook doesn't always guarantee a stable/verified
//               email the way Google does; Google never populates User.FacebookId.
// Entities connected: User.cs (indirectly, via ExternalLoginAsync)
// Tables related: None — this enum itself isn't stored anywhere
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

public enum AuthProvider
{
    Google,
    Facebook
}
