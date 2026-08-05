// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Es el corazón del flujo que describiste — negociación completa de un Service:
//           (1) un profesional propone precio+minutos de llegada, (2) el cliente ve las
//           propuestas y acepta una (las demás quedan Rejected automáticamente y la
//           dirección exacta se libera al ganador), (3) el profesional presiona "Estoy en
//           el sitio" (ConfirmArrivalAsync, timestamp del servidor), (4) sube la foto de
//           evidencia (UploadCompletionPhotoAsync, exige que ya haya confirmado llegada),
//           (5) tanto cliente como profesional confirman por separado (ConfirmCompletionAsync)
//           y solo cuando AMBOS confirmaron se cierra el Service.
// Entidades relacionadas: ServiceProfessional.cs, Service.cs, Professional.cs
// Tablas relacionadas: TBL_SERVICE_PROFESSIONALS, TBL_SERVICES, TBL_PROFESSIONALS
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class ServiceProfessionalService : IServiceProfessionalService
{
    private readonly IServiceProfessionalRepository _serviceProfessionalRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IProfessionalRepository _professionalRepository;

    public ServiceProfessionalService(
        IServiceProfessionalRepository serviceProfessionalRepository,
        IServiceRepository serviceRepository,
        IProfessionalRepository professionalRepository)
    {
        _serviceProfessionalRepository = serviceProfessionalRepository;
        _serviceRepository = serviceRepository;
        _professionalRepository = professionalRepository;
    }

    public async Task<ServiceProfessionalResponse> CreateProposalAsync(
        int professionalUserId, int serviceId, CreateServiceProfessionalRequest request)
    {
        var professional = await _professionalRepository.GetByUserIdAsync(professionalUserId)
            ?? throw new NotFoundException("El usuario autenticado no tiene un perfil de profesional.");

        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"No se encontró el servicio con id {serviceId}.");

        if (service.Status != ServiceStatus.Pending)
        {
            throw new ConflictException("Este servicio ya no está aceptando nuevas propuestas.");
        }

        if (await _serviceProfessionalRepository.ExistsProposalAsync(serviceId, professional.Id))
        {
            throw new ConflictException("Ya enviaste una propuesta para este servicio.");
        }

        var proposal = new ServiceProfessional
        {
            ServiceId = serviceId,
            ProfessionalId = professional.Id,
            NegotiatedPrice = request.NegotiatedPrice,
            EstimatedArrivalMinutes = request.EstimatedArrivalMinutes
        };

        await _serviceProfessionalRepository.AddAsync(proposal);
        proposal.Professional = professional;

        return MapToResponse(proposal);
    }

    public async Task<IReadOnlyList<ServiceProfessionalResponse>> GetProposalsAsync(int clientUserId, int serviceId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"No se encontró el servicio con id {serviceId}.");

        if (service.ClientId != clientUserId)
        {
            throw new ConflictException("Solo el cliente dueño del servicio puede ver sus propuestas.");
        }

        var proposals = await _serviceProfessionalRepository.GetByServiceIdAsync(serviceId);
        return proposals.Select(MapToResponse).ToList();
    }

    public async Task<ServiceProfessionalResponse> AcceptProposalAsync(int clientUserId, int serviceId, int proposalId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"No se encontró el servicio con id {serviceId}.");

        if (service.ClientId != clientUserId)
        {
            throw new ConflictException("Solo el cliente dueño del servicio puede aceptar una propuesta.");
        }

        if (service.Status != ServiceStatus.Pending)
        {
            throw new ConflictException("Este servicio ya no está pendiente de aceptación.");
        }

        var proposals = await _serviceProfessionalRepository.GetByServiceIdAsync(serviceId);
        var chosen = proposals.FirstOrDefault(p => p.Id == proposalId)
            ?? throw new NotFoundException($"No se encontró la propuesta {proposalId} para este servicio.");

        chosen.Status = ServiceProfessionalStatus.Accepted;
        foreach (var other in proposals.Where(p => p.Id != proposalId))
        {
            other.Status = ServiceProfessionalStatus.Rejected;
        }
        await _serviceProfessionalRepository.UpdateRangeAsync(proposals);

        // A partir de aquí, ese profesional ya puede ver la dirección exacta (ServiceService.CanSeeExactLocationAsync).
        service.Status = ServiceStatus.Accepted;
        await _serviceRepository.UpdateAsync(service);

        return MapToResponse(chosen);
    }

    public async Task ConfirmArrivalAsync(int professionalUserId, int serviceId)
    {
        var accepted = await GetAcceptedForProfessionalAsync(professionalUserId, serviceId);

        // Timestamp del servidor, nunca del celular del profesional (no se puede manipular).
        accepted.ArrivalConfirmedAt = DateTime.UtcNow;
        await _serviceProfessionalRepository.UpdateAsync(accepted);
    }

    public async Task UploadCompletionPhotoAsync(int professionalUserId, int serviceId, string photoUrl)
    {
        var accepted = await GetAcceptedForProfessionalAsync(professionalUserId, serviceId);
        if (accepted.ArrivalConfirmedAt is null)
        {
            throw new ConflictException("Debes confirmar tu llegada antes de subir evidencia de trabajo terminado.");
        }

        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"No se encontró el servicio con id {serviceId}.");

        service.CompletionPhotoUrl = photoUrl;
        await _serviceRepository.UpdateAsync(service);
    }

    public async Task<CompletionStatusResponse> ConfirmCompletionAsync(int callerUserId, int serviceId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"No se encontró el servicio con id {serviceId}.");

        if (string.IsNullOrEmpty(service.CompletionPhotoUrl))
        {
            throw new ConflictException("Aún no se ha subido evidencia de que el trabajo fue realizado.");
        }

        var accepted = await _serviceProfessionalRepository.GetAcceptedForServiceAsync(serviceId);

        if (service.ClientId == callerUserId)
        {
            service.ClientConfirmedCompletionAt = DateTime.UtcNow;
        }
        else if (accepted is not null && accepted.Professional.UserId == callerUserId)
        {
            service.ProfessionalConfirmedCompletionAt = DateTime.UtcNow;
        }
        else
        {
            throw new ConflictException("No tienes relación con este servicio para confirmarlo.");
        }

        // Solo cuando AMBAS partes confirman se cierra el servicio (ver contexto de negocio:
        // a partir de aquí cesa la responsabilidad legal entre cliente y profesional).
        if (service.ClientConfirmedCompletionAt is not null && service.ProfessionalConfirmedCompletionAt is not null)
        {
            service.Status = ServiceStatus.Completed;
            if (accepted is not null)
            {
                accepted.Status = ServiceProfessionalStatus.Completed;
                await _serviceProfessionalRepository.UpdateAsync(accepted);
            }
        }

        await _serviceRepository.UpdateAsync(service);

        return new CompletionStatusResponse
        {
            ServiceId = service.Id,
            ClientConfirmed = service.ClientConfirmedCompletionAt is not null,
            ProfessionalConfirmed = service.ProfessionalConfirmedCompletionAt is not null,
            IsCompleted = service.Status == ServiceStatus.Completed
        };
    }

    private async Task<ServiceProfessional> GetAcceptedForProfessionalAsync(int professionalUserId, int serviceId)
    {
        var accepted = await _serviceProfessionalRepository.GetAcceptedForServiceAsync(serviceId)
            ?? throw new NotFoundException("Este servicio todavía no tiene un profesional aceptado.");

        if (accepted.Professional.UserId != professionalUserId)
        {
            throw new ConflictException("No eres el profesional aceptado para este servicio.");
        }

        return accepted;
    }

    private static ServiceProfessionalResponse MapToResponse(ServiceProfessional sp) => new()
    {
        Id = sp.Id,
        ServiceId = sp.ServiceId,
        ProfessionalId = sp.ProfessionalId,
        ProfessionalFirstName = sp.Professional.User.FirstName,
        ProfessionalLastName = sp.Professional.User.LastName,
        ProfessionalAverageRating = sp.Professional.AverageRating,
        NegotiatedPrice = sp.NegotiatedPrice,
        EstimatedArrivalMinutes = sp.EstimatedArrivalMinutes,
        ArrivalConfirmedAt = sp.ArrivalConfirmedAt,
        Status = sp.Status
    };
}
