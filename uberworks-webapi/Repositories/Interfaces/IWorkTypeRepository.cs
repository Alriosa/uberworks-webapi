// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for WorkType — full CRUD (create, list all, get by
//               id, check for a duplicate name, check if it has Services attached before
//               allowing deletion, update, delete).
// Entities connected: WorkType.cs
// Tables related: TBL_WORKTYPES (indirectly, via WorkTypeRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IWorkTypeRepository
{
    Task<WorkType?> GetByIdAsync(int id);
    Task<IReadOnlyList<WorkType>> GetAllAsync();
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<bool> HasServicesAsync(int id);
    Task AddAsync(WorkType workType);
    Task UpdateAsync(WorkType workType);
    Task DeleteAsync(WorkType workType);
}
