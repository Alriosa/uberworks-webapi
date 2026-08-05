// =====================================================================================
// FILE SUMMARY
// What it does: This is where the most important security rule of the business lives:
//               "only the owning client or the accepted professional can see the exact
//               address" (private method CanSeeExactLocationAsync). Every public method
//               builds a ServiceResponse by calling MapToResponse() with includeExactLocation
//               set to true/false as appropriate: GetOpenAsync() always false (public
//               listing), GetMyServicesAsync() always true (the owner sees everything of
//               theirs), GetByIdAsync() decides dynamically.
// Entities connected: Service.cs, WorkType.cs, ServiceProfessional.cs (to know who the
//                      accepted professional is)
// Tables related: TBL_SERVICES, TBL_WORKTYPES, TBL_SERVICE_PROFESSIONALS
// =====================================================================================
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IWorkTypeRepository _workTypeRepository;
    private readonly IServiceProfessionalRepository _serviceProfessionalRepository;

    public ServiceService(
        IServiceRepository serviceRepository,
        IWorkTypeRepository workTypeRepository,
        IServiceProfessionalRepository serviceProfessionalRepository)
    {
        _serviceRepository = serviceRepository;
        _workTypeRepository = workTypeRepository;
        _serviceProfessionalRepository = serviceProfessionalRepository;
    }

    public async Task<ServiceResponse> CreateAsync(int clientId, CreateServiceRequest request)
    {
        _ = await _workTypeRepository.GetByIdAsync(request.WorkTypeId)
            ?? throw new NotFoundException($"Work type with id {request.WorkTypeId} was not found.");

        var service = new Service
        {
            WorkTypeId = request.WorkTypeId,
            ClientId = clientId,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            ProposedPrice = request.ProposedPrice,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ExactAddress = request.ExactAddress,
            Zone = request.Zone
        };

        await _serviceRepository.AddAsync(service);
        var created = await _serviceRepository.GetByIdAsync(service.Id) ?? service;

        // The client who just created it is the owner: they always see the full detail.
        return MapToResponse(created, includeExactLocation: true);
    }

    public async Task<IReadOnlyList<ServiceResponse>> GetOpenAsync()
    {
        var services = await _serviceRepository.GetOpenAsync();

        // Public listing for professionals: the exact address is never included here.
        return services.Select(s => MapToResponse(s, includeExactLocation: false)).ToList();
    }

    public async Task<IReadOnlyList<ServiceResponse>> GetMyServicesAsync(int clientId)
    {
        var services = await _serviceRepository.GetByClientIdAsync(clientId);
        return services.Select(s => MapToResponse(s, includeExactLocation: true)).ToList();
    }

    public async Task<ServiceResponse> GetByIdAsync(int serviceId, int? callerUserId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        var includeExactLocation = await CanSeeExactLocationAsync(service, callerUserId);
        return MapToResponse(service, includeExactLocation);
    }

    private async Task<bool> CanSeeExactLocationAsync(Service service, int? callerUserId)
    {
        if (callerUserId is null)
        {
            return false;
        }

        if (service.ClientId == callerUserId)
        {
            return true;
        }

        var accepted = await _serviceProfessionalRepository.GetAcceptedForServiceAsync(service.Id);
        return accepted is not null && accepted.Professional.UserId == callerUserId;
    }

    private static ServiceResponse MapToResponse(Service service, bool includeExactLocation)
    {
        var response = new ServiceResponse
        {
            Id = service.Id,
            WorkTypeId = service.WorkTypeId,
            WorkTypeName = service.WorkType?.Name ?? string.Empty,
            ClientId = service.ClientId,
            Description = service.Description,
            ImageUrl = service.ImageUrl,
            ProposedPrice = service.ProposedPrice,
            Status = service.Status,
            RequestDate = service.RequestDate,
            Zone = service.Zone,
            CompletionPhotoUrl = service.CompletionPhotoUrl,
            ClientConfirmedCompletionAt = service.ClientConfirmedCompletionAt,
            ProfessionalConfirmedCompletionAt = service.ProfessionalConfirmedCompletionAt
        };

        if (includeExactLocation)
        {
            response.Latitude = service.Latitude;
            response.Longitude = service.Longitude;
            response.ExactAddress = service.ExactAddress;
        }

        return response;
    }
}
