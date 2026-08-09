// =====================================================================================
// FILE SUMMARY
// What it does: What the API returns for a Penalty — includes the username (so the WebApp
//               never has to make an extra round-trip just to show a name instead of a raw
//               id) and IsActive, a computed convenience flag (PenaltyService.cs works it out
//               from Type/EndDate so the WebApp doesn't have to duplicate that logic).
//               Backs a user's own "Advertencias" view (Professional dashboard today).
// Entities connected: Penalty.cs, User.cs (PenaltyService.cs maps from there)
// Tables related: None directly — it's the "public shape" of a TBL_PENALTIES row
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.DTOs.Responses;

public class PenaltyResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public PenaltyType Type { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    /// <summary>True when Type is Permanent, or when Type is Temporary and EndDate hasn't passed yet.</summary>
    public bool IsActive { get; set; }
}
