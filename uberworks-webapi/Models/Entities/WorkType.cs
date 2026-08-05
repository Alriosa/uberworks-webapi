// =====================================================================================
// FILE SUMMARY
// What it does: Represents a work category in the admin-managed catalog (e.g. "Plumbing",
//               "Electrical"). An Admin/MasterAdmin can create, edit, or delete these
//               categories from the API without a developer having to touch code (see
//               Controllers/WorkTypesController.cs).
// Entities connected: Service.cs (1:N — each Service belongs to a WorkType)
// Tables related: TBL_WORKTYPES (mapping in Data/Configurations/WorkTypeConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_WORKTYPES (work categories).
/// </summary>
public class WorkType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Includes { get; set; }
    public string? NotIncludes { get; set; }

    // Navigation properties
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
