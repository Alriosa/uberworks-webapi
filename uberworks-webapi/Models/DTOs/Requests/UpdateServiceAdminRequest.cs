// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of PUT /api/services/{id} — what a MasterAdmin/Admin
// sends to edit a "Work Post" from the Admin dashboard's CRUD panel. Deliberately a smaller
// surface than the full Service entity: WorkTypeId/ClientId/location never change here (an
// Admin correcting a job shouldn't be able to reassign it to a different client or move its
// GPS pin), only the fields that are actually reasonable to fix after the fact.
// Entities connected: Service.cs (indirectly, via ServiceService.UpdateForAdminAsync)
// Tables related: None directly (TBL_SERVICES is updated from ServiceService.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Requests;

public class UpdateServiceAdminRequest
{
    public string? Description { get; set; }
    public decimal? ProposedPrice { get; set; }
    public ServiceStatus Status { get; set; }
    public string Zone { get; set; } = string.Empty;
}
