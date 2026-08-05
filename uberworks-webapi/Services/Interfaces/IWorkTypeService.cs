// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Contrato de la lógica de negocio de WorkType (CRUD del catálogo).
//           WorkTypesController.cs depende de esta interface, no de WorkTypeService.cs
//           directamente.
// Entidades relacionadas: WorkType.cs
// Tablas relacionadas: TBL_WORKTYPES (indirectamente, vía WorkTypeService.cs)
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
