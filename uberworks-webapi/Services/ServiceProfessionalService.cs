// =====================================================================================
// FILE SUMMARY
// What it does: This is the heart of the flow you described — a Service's full
//               negotiation: (1) a professional proposes a price + arrival minutes, (2) the
//               client sees the proposals and accepts one (the others are automatically
//               Rejected and the exact address is released to the winner), (3) the
//               professional presses "I'm on site" (ConfirmArrivalAsync, server timestamp),
//               (4) they upload the completion photo (UploadCompletionPhotoAsync, requires
//               having already confirmed arrival), (5) both the client and the professional
//               confirm separately (ConfirmCompletionAsync), and only once BOTH have
//               confirmed does the Service close.
// Entities connected: ServiceProfessional.cs, Service.cs, Professional.cs
// Tables related: TBL_SERVICE_PROFESSIONALS, TBL_SERVICES, TBL_PROFESSIONALS
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
            ?? throw new NotFoundException("The authenticated user does not have a professional profile.");

        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        if (service.Status != ServiceStatus.Pending)
        {
            throw new ConflictException("This service is no longer accepting new proposals.");
        }

        if (await _serviceProfessionalRepository.ExistsProposalAsync(serviceId, professional.Id))
        {
            throw new ConflictException("You already submitted a proposal for this service.");
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
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        if (service.ClientId != clientUserId)
        {
            throw new ConflictException("Only the client who owns this service can view its proposals.");
        }

        var proposals = await _serviceProfessionalRepository.GetByServiceIdAsync(serviceId);
        return proposals.Select(MapToResponse).ToList();
    }

    public async Task<ServiceProfessionalResponse> AcceptProposalAsync(int clientUserId, int serviceId, int proposalId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        if (service.ClientId != clientUserId)
        {
            throw new ConflictException("Only the client who owns this service can accept a proposal.");
        }

        if (service.Status != ServiceStatus.Pending)
        {
            throw new ConflictException("This service is no longer pending acceptance.");
        }

        var proposals = await _serviceProfessionalRepository.GetByServiceIdAsync(serviceId);
        var chosen = proposals.FirstOrDefault(p => p.Id == proposalId)
            ?? throw new NotFoundException($"Proposal {proposalId} was not found for this service.");

        chosen.Status = ServiceProfessionalStatus.Accepted;
        foreach (var other in proposals.Where(p => p.Id != proposalId))
        {
            other.Status = ServiceProfessionalStatus.Rejected;
        }
        await _serviceProfessionalRepository.UpdateRangeAsync(proposals);

        // From here on, that professional can see the exact address (ServiceService.CanSeeExactLocationAsync).
        service.Status = ServiceStatus.Accepted;
        await _serviceRepository.UpdateAsync(service);

        return MapToResponse(chosen);
    }

    public async Task ConfirmArrivalAsync(int professionalUserId, int serviceId)
    {
        var accepted = await GetAcceptedForProfessionalAsync(professionalUserId, serviceId);

        // Server timestamp, never the professional's phone (can't be tampered with).
        accepted.ArrivalConfirmedAt = DateTime.UtcNow;
        await _serviceProfessionalRepository.UpdateAsync(accepted);
    }

    public async Task UploadCompletionPhotoAsync(int professionalUserId, int serviceId, string photoUrl)
    {
        var accepted = await GetAcceptedForProfessionalAsync(professionalUserId, serviceId);
        if (accepted.ArrivalConfirmedAt is null)
        {
            throw new ConflictException("You must confirm your arrival before uploading completion evidence.");
        }

        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        service.CompletionPhotoUrl = photoUrl;
        await _serviceRepository.UpdateAsync(service);
    }

    public async Task<CompletionStatusResponse> ConfirmCompletionAsync(int callerUserId, int serviceId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId)
            ?? throw new NotFoundException($"Service with id {serviceId} was not found.");

        if (string.IsNullOrEmpty(service.CompletionPhotoUrl))
        {
            throw new ConflictException("Completion evidence has not been uploaded yet.");
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
            throw new ConflictException("You have no relationship with this service to confirm it.");
        }

        // The service only closes once BOTH parties have confirmed (see the business
        // context: from this point on, legal responsibility between client and
        // professional ends).
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
            ?? throw new NotFoundException("This service doesn't have an accepted professional yet.");

        if (accepted.Professional.UserId != professionalUserId)
        {
            throw new ConflictException("You are not the accepted professional for this service.");
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
