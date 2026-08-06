// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Profile/MasterAdmin.cshtml. Adds nothing to ProfileViewModelBase
//               yet — there's only ever one MasterAdmin account (seeded, see
//               Data/Seed/MasterAdminSeeder.cs in the API) and it has no extra domain data
//               today. Kept as its own class/view so this is the one place to extend later
//               without touching anyone else's profile page.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class MasterAdminProfileViewModel : ProfileViewModelBase
{
}
