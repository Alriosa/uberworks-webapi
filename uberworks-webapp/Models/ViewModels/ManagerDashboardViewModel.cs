// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Manager.cshtml — same shape as
//               CompanyDashboardViewModel.cs (CompanyName/Workers/Events), since a Manager
//               sees exactly what its Company sees, except it can't create Events (the one
//               explicit difference, enforced by not showing that button/form at all here,
//               and by the API rejecting it anyway if attempted). CompanyName comes from
//               GET /api/users/my-company (a Manager isn't itself the company, so it can't
//               just use its own name like CompanyDashboardViewModel does).
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_USERS/TBL_PROFESSIONALS/TBL_EVENTS only indirectly, through the API
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class ManagerDashboardViewModel : DashboardViewModelBase
{
    public string CompanyName { get; set; } = string.Empty;
    public List<ProfessionalResponse> Workers { get; set; } = new();
    public List<EventResponse> Events { get; set; } = new();
}
