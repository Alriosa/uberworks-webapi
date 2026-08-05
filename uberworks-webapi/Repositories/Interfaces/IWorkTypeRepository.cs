// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato de acceso a datos para WorkType — CRUD completo (crear, listar todos,
//           obtener por id, verificar nombre duplicado, verificar si tiene Services
//           asociados antes de dejar borrarlo, actualizar, eliminar).
// Entidades relacionadas: WorkType.cs
// Tablas relacionadas: TBL_WORKTYPES (indirectamente, vía WorkTypeRepository.cs)
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
