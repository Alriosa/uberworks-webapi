// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of PUT /api/worktypes/{id} — same shape as
//               CreateWorkTypeRequest.cs, for editing an existing category.
// Entities connected: WorkType.cs (indirectly, via WorkTypeService.UpdateAsync)
// Tables related: None directly (TBL_WORKTYPES is updated from WorkTypeService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class UpdateWorkTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Includes { get; set; }
    public string? NotIncludes { get; set; }
}
