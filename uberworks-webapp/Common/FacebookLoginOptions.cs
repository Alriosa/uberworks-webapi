// =====================================================================================
// FILE SUMMARY
// What it does: A tiny flag registered as a singleton in Program.cs, telling the rest of
//               the app whether Facebook credentials (FacebookAuth:AppId/AppSecret) were
//               actually configured — same pattern as GoogleLoginOptions.cs. Program.cs only
//               calls AddFacebook() when this is true — registering it with an empty AppId
//               crashes the whole site, not just the Facebook button. Views/Account/
//               Login.cshtml and Register.cshtml inject this to decide whether to render
//               "Continue with Facebook" at all, and AccountController.FacebookLogin checks
//               it before issuing a Challenge.
// Entities connected: None
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Common;

public class FacebookLoginOptions
{
    public bool IsEnabled { get; }

    public FacebookLoginOptions(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}
