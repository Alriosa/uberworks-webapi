// =====================================================================================
// FILE SUMMARY
// What it does: Backs Views/Dashboard/Company.cshtml. CompanyName is the Company account's
//               own FirstName+LastName (this app has no separate "business name" field — a
//               Company account's User row doubles as its business identity, same as every
//               other role). Workers/Events are REAL — GET /api/professionals/my-workers and
//               GET /api/events/mine.
// Entities connected: None — this project has no database entities
// Tables related: None — reaches TBL_PROFESSIONALS/TBL_EVENTS only indirectly, through the API
// =====================================================================================
using uberworks_webapp.Models.ApiContracts;

namespace uberworks_webapp.Models.ViewModels;

public class CompanyDashboardViewModel : DashboardViewModelBase
{
    public string CompanyName { get; set; } = string.Empty;
    public List<ProfessionalResponse> Workers { get; set; } = new();
    public List<EventResponse> Events { get; set; } = new();
}
