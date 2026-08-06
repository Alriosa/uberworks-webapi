// =====================================================================================
// FILE SUMMARY
// What it does: A tiny flag registered as a singleton in Program.cs, telling the rest of
//               the app whether Google credentials (GoogleAuth:ClientId/ClientSecret) were
//               actually configured. Program.cs only calls AddGoogle() when this is true —
//               registering it with an empty ClientId crashes the whole site, not just the
//               Google button. Views/Account/Login.cshtml and Register.cshtml inject this to
//               decide whether to render "Continue with Google" at all, and
//               AccountController.GoogleLogin checks it before issuing a Challenge.
// Entities connected: None
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Common;

public class GoogleLoginOptions
{
    public bool IsEnabled { get; }

    public GoogleLoginOptions(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}
