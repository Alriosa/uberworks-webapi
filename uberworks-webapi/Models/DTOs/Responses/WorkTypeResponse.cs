// =====================================================================================
// FILE SUMMARY
// What it does: Describes what the API returns when querying a WorkType. Almost identical
//               shape to WorkType.cs since it's a simple entity with no sensitive data to hide.
// Entities connected: WorkType.cs (WorkTypeService.cs maps from one to the other)
// Tables related: None directly — it's the "public shape" of a TBL_WORKTYPES row
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Responses;

public class WorkTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Includes { get; set; }
    public string? NotIncludes { get; set; }
}
