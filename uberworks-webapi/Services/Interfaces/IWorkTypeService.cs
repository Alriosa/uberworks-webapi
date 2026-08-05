// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the WorkType business logic (catalog CRUD).
//               WorkTypesController.cs depends on this interface, not on WorkTypeService.cs
//               directly.
// Entities connected: WorkType.cs
// Tables related: TBL_WORKTYPES (indirectly, via WorkTypeService.cs)
// =====================================================================================
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IWorkTypeService
{
    Task<WorkTypeResponse> CreateAsync(CreateWorkTypeRequest request);
    Task<IReadOnlyList<WorkTypeResponse>> GetAllAsync();
    Task<WorkTypeResponse> GetByIdAsync(int id);
    Task<WorkTypeResponse> UpdateAsync(int id, UpdateWorkTypeRequest request);
    Task DeleteAsync(int id);
}
