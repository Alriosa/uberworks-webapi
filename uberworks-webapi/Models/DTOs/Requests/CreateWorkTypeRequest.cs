// =====================================================================================
// FILE SUMMARY
// What it does: Describes the body of POST /api/worktypes — what an Admin/MasterAdmin
//               sends to create a new catalog category (e.g. "Plumbing").
// Entities connected: WorkType.cs (indirectly, via WorkTypeService.CreateAsync)
// Tables related: None directly (TBL_WORKTYPES is filled in from WorkTypeService.cs)
// =====================================================================================
namespace uberworks_webapi.Models.DTOs.Requests;

public class CreateWorkTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Includes { get; set; }
    public string? NotIncludes { get; set; }
}
