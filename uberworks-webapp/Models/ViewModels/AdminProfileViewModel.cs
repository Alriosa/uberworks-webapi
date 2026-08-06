// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Profile/Admin.cshtml. Adds nothing to ProfileViewModelBase
//               yet — Admins have no extra domain data today. Kept as its own class/view
//               (not shared with Manager/MasterAdmin) so this is the one place to extend
//               later without touching anyone else's profile page.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class AdminProfileViewModel : ProfileViewModelBase
{
}
