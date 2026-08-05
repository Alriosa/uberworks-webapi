// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Lógica de negocio del catálogo de WorkTypes. Valida nombre duplicado al crear
//           y al actualizar, y bloquea el borrado (ConflictException → HTTP 409) si el
//           WorkType ya tiene Services asociados, para no dejar datos huérfanos.
// Entidades relacionadas: WorkType.cs, Service.cs (solo para verificar dependencias)
// Tablas relacionadas: TBL_WORKTYPES, TBL_SERVICES
// =====================================================================================
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class WorkTypeService : IWorkTypeService
{
    private readonly IWorkTypeRepository _workTypeRepository;

    public WorkTypeService(IWorkTypeRepository workTypeRepository)
    {
        _workTypeRepository = workTypeRepository;
    }

    public async Task<WorkTypeResponse> CreateAsync(CreateWorkTypeRequest request)
    {
        if (await _workTypeRepository.ExistsByNameAsync(request.Name))
        {
            throw new ConflictException($"Ya existe un tipo de trabajo con el nombre '{request.Name}'.");
        }

        var workType = new WorkType
        {
            Name = request.Name,
            Description = request.Description,
            Includes = request.Includes,
            NotIncludes = request.NotIncludes
        };

        await _workTypeRepository.AddAsync(workType);

        return MapToResponse(workType);
    }

    public async Task<IReadOnlyList<WorkTypeResponse>> GetAllAsync()
    {
        var workTypes = await _workTypeRepository.GetAllAsync();
        return workTypes.Select(MapToResponse).ToList();
    }

    public async Task<WorkTypeResponse> GetByIdAsync(int id)
    {
        var workType = await _workTypeRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el tipo de trabajo con id {id}.");

        return MapToResponse(workType);
    }

    public async Task<WorkTypeResponse> UpdateAsync(int id, UpdateWorkTypeRequest request)
    {
        var workType = await _workTypeRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el tipo de trabajo con id {id}.");

        if (await _workTypeRepository.ExistsByNameAsync(request.Name, excludeId: id))
        {
            throw new ConflictException($"Ya existe un tipo de trabajo con el nombre '{request.Name}'.");
        }

        workType.Name = request.Name;
        workType.Description = request.Description;
        workType.Includes = request.Includes;
        workType.NotIncludes = request.NotIncludes;

        await _workTypeRepository.UpdateAsync(workType);

        return MapToResponse(workType);
    }

    public async Task DeleteAsync(int id)
    {
        var workType = await _workTypeRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el tipo de trabajo con id {id}.");

        if (await _workTypeRepository.HasServicesAsync(id))
        {
            throw new ConflictException(
                $"No se puede eliminar el tipo de trabajo '{workType.Name}' porque tiene servicios asociados.");
        }

        await _workTypeRepository.DeleteAsync(workType);
    }

    private static WorkTypeResponse MapToResponse(WorkType workType) => new()
    {
        Id = workType.Id,
        Name = workType.Name,
        Description = workType.Description,
        Includes = workType.Includes,
        NotIncludes = workType.NotIncludes
    };
}
