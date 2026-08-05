// =====================================================================================
// FILE SUMMARY
// What it does: Stores a sanction applied to a user (temporary or permanent) with its
//               reason and start/end dates. Doesn't have a Repository/Service/Controller
//               built yet — it will likely connect with the Admin permissions we'll discuss
//               later (an Admin might be able to penalize a user).
// Entities connected: User.cs (N:1)
// Tables related: TBL_PENALTIES (mapping in Data/Configurations/PenaltyConfiguration.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_PENALTIES.
/// </summary>
public class Penalty
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public PenaltyType? Type { get; set; }
    public string? Reason { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
