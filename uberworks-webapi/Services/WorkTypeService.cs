// =====================================================================================
// FILE SUMMARY
// What it does: Business logic for the WorkType catalog. Validates duplicate names on
//               create and update, and blocks deletion (ConflictException → HTTP 409) if
//               the WorkType already has Services attached, to avoid leaving orphaned data.
// Entities connected: WorkType.cs, Service.cs (only to check dependencies)
// Tables related: TBL_WORKTYPES, TBL_SERVICES
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
            throw new ConflictException($"A work type named '{request.Name}' already exists.");
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
            ?? throw new NotFoundException($"Work type with id {id} was not found.");

        return MapToResponse(workType);
    }

    public async Task<WorkTypeResponse> UpdateAsync(int id, UpdateWorkTypeRequest request)
    {
        var workType = await _workTypeRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Work type with id {id} was not found.");

        if (await _workTypeRepository.ExistsByNameAsync(request.Name, excludeId: id))
        {
            throw new ConflictException($"A work type named '{request.Name}' already exists.");
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
            ?? throw new NotFoundException($"Work type with id {id} was not found.");

        if (await _workTypeRepository.HasServicesAsync(id))
        {
            throw new ConflictException(
                $"Cannot delete work type '{workType.Name}' because it has services attached to it.");
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
