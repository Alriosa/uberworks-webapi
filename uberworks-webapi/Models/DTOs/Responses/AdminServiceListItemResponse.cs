// =====================================================================================
// FILE SUMMARY
// What it does: What GET /api/services returns to a MasterAdmin/Admin caller — one row per
//               "Work Post" (Service), with the exact location always included (unlike the
//               public ServiceResponse, where that depends on who's asking — an Admin can
//               always see everything) plus the requesting client's username/full name for
//               readability. Backs the "Ver Todos los Trabajos" CRUD panel on the WebApp's
//               Admin dashboard (Views/Dashboard/Admin.cshtml), same idea as
//               AdminUserListItemResponse.cs for the users panel.
// Entities connected: Service.cs, WorkType.cs, User.cs (ServiceService.GetAllForAdminAsync
//                      maps a list of these)
// Tables related: None directly — it's the "admin shape" of a TBL_SERVICES row
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class AdminServiceListItemResponse
{
    public int Id { get; set; }
    public int WorkTypeId { get; set; }
    public string WorkTypeName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string ClientUsername { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? ProposedPrice { get; set; }
    public ServiceStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string ExactAddress { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string? CompletionPhotoUrl { get; set; }
    public DateTime? ClientConfirmedCompletionAt { get; set; }
    public DateTime? ProfessionalConfirmedCompletionAt { get; set; }
}
