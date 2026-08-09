// =====================================================================================
// FILE SUMMARY
// What it does: Stores a sanction applied to a user (temporary or permanent) with its
//               reason and start/end dates. Repositories/PenaltyRepository.cs,
//               Services/PenaltyService.cs, and Controllers/PenaltiesController.cs implement
//               the real CRUD — Admin/MasterAdmin apply a penalty, and the affected user (or
//               an Admin) can look it up. Backs the "Advertencias" panel on the Professional
//               dashboard, per explicit request.
// Entities connected: User.cs (N:1)
// Tables related: TBL_PENALTIES (mapping in Repositories/PenaltyRepository.cs — this app
//                 uses Dapper with raw SQL, not EF Core, so there's no Configurations file)
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

    public PenaltyType Type { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
