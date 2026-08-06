// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Profile/Client.cshtml. Adds nothing to ProfileViewModelBase
//               yet — a Client has no extra domain data today (Services/Payments/Reviews
//               aren't built out on the WebApp side yet). Kept as its own class/view anyway
//               (not reusing another role's) so this is the one place to extend later
//               without touching anyone else's profile page.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class ClientProfileViewModel : ProfileViewModelBase
{
}
