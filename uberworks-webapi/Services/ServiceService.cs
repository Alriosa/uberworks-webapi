// =====================================================================================
// FILE SUMMARY
// What it does: This is where the most important security rule of the business lives:
//               "only the owning client or the accepted professional can see the exact
//               address" (private method CanSeeExactLocationAsync). Every public method
//               builds a ServiceResponse by calling MapToResponseAsync() with
//               includeExactLocation set to true/false as appropriate: GetOpenAsync() always
//               false (public listing), GetMyServicesAsync() always true (the owner sees
//               everything of theirs), GetByIdAsync() decides dynamically.
//               MapToResponseAsync always looks up the client's username/full name too (one
//               extra GetByIdAsync per Service — N+1, acceptable at this app's scale) since
//               ClientUsername/ClientFullName travel on ServiceResponse even in the public
//               GetOpenAsync() listing, per explicit request (a professional browsing job
//               offers needs to see who posted them). GetAllForAdminAsync/
//               UpdateForAdminAsync/DeleteForAdminAsync back the Admin dashboard's job CRUD
//               panel — DeleteForAdminAsync is a soft delete (Status=Cancelled), never a real
//               SQL DELETE, since ServiceProfessional/Review/Payment rows reference this Service.
// Entities connected: Service.cs, WorkType.cs, ServiceProfessional.cs (to know who the
//                      accepted professional is), User.cs (client username/full name)
// Tables related: TBL_SERVICES, TBL_WORKTYPES, TBL_SERVICE_PROFESSIONALS
// =====================================================================================
using uberworks_webapi.Common.Enums;
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
    private readonly IUserRepository _userRepository;
    private readonly IProfessionalRepository _professionalRepository;

    public ServiceService(
        IServiceRepository serviceRepository,
        IWorkTypeRepository workTypeRepository,
        IServiceProfessionalRepository serviceProfessionalRepository,
        IUserRepository userRepository,
        IProfessionalRepository professionalRepository)
    {
        _serviceRepository = serviceRepository;
        _workTypeRepository = workTypeRepository;
        _serviceProfessionalRepository = serviceProfessionalRepository;
        _userRepository = userRepository;
        _professionalRepository = professionalRepository;
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
        return await MapToResponseAsync(created, includeExactLocation: true);
    }

    public async Task<IReadOnlyList<ServiceResponse>> GetOpenAsync()
    {
        var services = await _serviceRepository.GetOpenAsync();

        // Public listing for professionals: the exact address is never included here, but
        // ClientUsername/ClientFullName ARE — see ServiceResponse.cs's FILE SUMMARY for why.
        // One GetByIdAsync per Service (N+1) is acceptable here, same reasoning as
        // GetAllForAdminAsync below — this app's traffic is nowhere near the scale where that
        // would matter.
        var result = new List<ServiceResponse>(services.Count);
        foreach (var service in services)
        {
            result.Add(await MapToResponseAsync(service, includeExactLocation: false));
        }

        return result;
    }

    public async Task<IReadOnlyList<ServiceResponse>> GetMyServicesAsync(int clientId)
    {
        var services = await _serviceRepository.GetByClientIdAsync(clientId);
        var result = new List<ServiceResponse>(services.Count);
        foreach (var service in services)
        {
            result.Add(await MapToResponseAsync(service, includeExactLocation: true));
        }

        return result;
    }

    public async Task<IReadOnlyList<ServiceResponse>> GetMyCompletedJobsAsProfessionalAsync(int professionalUserId)
    {
        var professional = await _professionalRepository.GetByUserIdAsync(professionalUserId)
            ?? throw new NotFoundException("The authenticated user does not have a professional profile.");

        var serviceIds = await _serviceProfessionalRepository.GetAcceptedServiceIdsAsync(professional.Id);
        var result = new List<ServiceResponse>(serviceIds.Count);

        foreach (var serviceId in serviceIds)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service is not null)
            {
                result.Add(await MapToResponseAsync(service, includeExactLocation: true));
            }
        }

        return result;
    }

    public async Task<ServiceResponse> GetByIdAsync(int serviceId, int? callerUserId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        var includeExactLocation = await CanSeeExactLocationAsync(service, callerUserId);
        return await MapToResponseAsync(service, includeExactLocation);
    }

    // Full detail, every status, for the Admin dashboard's job CRUD panel. N+1 lookups
    // (one GetByIdAsync per Service to get the client's username) are acceptable here — this
    // is an internal admin listing, not a public/high-traffic endpoint.
    public async Task<IReadOnlyList<AdminServiceListItemResponse>> GetAllForAdminAsync()
    {
        var services = await _serviceRepository.GetAllAsync();
        var result = new List<AdminServiceListItemResponse>(services.Count);

        foreach (var service in services)
        {
            var client = await _userRepository.GetByIdAsync(service.ClientId);

            result.Add(new AdminServiceListItemResponse
            {
                Id = service.Id,
                WorkTypeId = service.WorkTypeId,
                WorkTypeName = service.WorkType?.Name ?? string.Empty,
                ClientId = service.ClientId,
                ClientUsername = client?.Username ?? string.Empty,
                ClientFullName = client is null ? string.Empty : $"{client.FirstName} {client.LastName}",
                Description = service.Description,
                ImageUrl = service.ImageUrl,
                ProposedPrice = service.ProposedPrice,
                Status = service.Status,
                RequestDate = service.RequestDate,
                Latitude = service.Latitude,
                Longitude = service.Longitude,
                ExactAddress = service.ExactAddress,
                Zone = service.Zone,
                CompletionPhotoUrl = service.CompletionPhotoUrl,
                ClientConfirmedCompletionAt = service.ClientConfirmedCompletionAt,
                ProfessionalConfirmedCompletionAt = service.ProfessionalConfirmedCompletionAt
            });
        }

        return result;
    }

    public async Task<ServiceResponse> UpdateForAdminAsync(int serviceId, UpdateServiceAdminRequest request)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        service.Description = request.Description;
        service.ProposedPrice = request.ProposedPrice;
        service.Status = request.Status;
        service.Zone = request.Zone;

        await _serviceRepository.UpdateAsync(service);

        return await MapToResponseAsync(service, includeExactLocation: true);
    }

    public async Task DeleteForAdminAsync(int serviceId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        service.Status = ServiceStatus.Cancelled;
        await _serviceRepository.UpdateAsync(service);
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

    private async Task<ServiceResponse> MapToResponseAsync(Service service, bool includeExactLocation)
    {
        var client = await _userRepository.GetByIdAsync(service.ClientId);

        var response = new ServiceResponse
        {
            Id = service.Id,
            WorkTypeId = service.WorkTypeId,
            WorkTypeName = service.WorkType?.Name ?? string.Empty,
            ClientId = service.ClientId,
            ClientUsername = client?.Username ?? string.Empty,
            ClientFullName = client is null ? string.Empty : $"{client.FirstName} {client.LastName}",
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
