// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Profile/Company.cshtml. Adds WorkerCount — the number of
//               Professionals this Company has created (Professional.CompanyUserId),
//               computed from GET /api/professionals/my-workers, on top of the plain User
//               data in ProfileViewModelBase.
// Entities connected: None — this project has no database entities
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ViewModels;

public class CompanyProfileViewModel : ProfileViewModelBase
{
    public int WorkerCount { get; set; }
}
