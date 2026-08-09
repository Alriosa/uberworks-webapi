// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/penalties — Admin/MasterAdmin applying a
//               sanction/warning to a user's account. EndDate is required when Type is
//               Temporary (when the sanction lifts); it's ignored when Type is Permanent —
//               PenaltyService.CreateAsync fills in a far-future sentinel instead, since
//               TBL_PENALTIES.CL_END_DATE is NOT NULL.
// Entities connected: Penalty.cs (indirectly, via PenaltyService.CreateAsync)
// Tables related: None directly (TBL_PENALTIES is filled in from PenaltyService.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Requests;

public class CreatePenaltyRequest
{
    public int UserId { get; set; }
    public PenaltyType Type { get; set; }
    public string Reason { get; set; } = string.Empty;

    /// <summary>Required when Type is Temporary; ignored when Type is Permanent.</summary>
    public DateTime? EndDate { get; set; }
}
