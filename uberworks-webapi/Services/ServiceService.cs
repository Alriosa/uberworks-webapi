// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Aquí vive la regla de seguridad más importante del negocio: "solo el cliente
//           dueño o el profesional aceptado ven la dirección exacta" (método privado
//           CanSeeExactLocationAsync). Cada método público arma un ServiceResponse
//           llamando a MapToResponse() con includeExactLocation en true/false según el
//           caso: GetOpenAsync() siempre en false (listado público), GetMyServicesAsync()
//           siempre en true (el dueño ve todo lo suyo), GetByIdAsync() lo decide dinámicamente.
// Entidades relacionadas: Service.cs, WorkType.cs, ServiceProfessional.cs (para saber quién
//                          es el profesional aceptado)
// Tablas relacionadas: TBL_SERVICES, TBL_WORKTYPES, TBL_SERVICE_PROFESSIONALS
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
            ?? throw new NotFoundException($"No se encontró el tipo de trabajo con id {request.WorkTypeId}.");

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

        // El cliente que acaba de crearlo es el dueño: siempre ve el detalle completo.
        return MapToResponse(created, includeExactLocation: true);
    }

    public async Task<IReadOnlyList<ServiceResponse>> GetOpenAsync()
    {
        var services = await _serviceRepository.GetOpenAsync();

        // Listado público para profesionales: la dirección exacta nunca se incluye aquí.
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
            ?? throw new NotFoundException($"No se encontró el servicio con id {serviceId}.");

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
