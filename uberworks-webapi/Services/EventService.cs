// =====================================================================================
// FILE SUMMARY
// What it does: Holds the Company Event/invitation business logic. CreateAsync fetches
// every Professional currently linked to the company (IProfessionalRepository.
// GetByCompanyUserIdAsync) and creates one Pending EventInvitation per worker in a single
// bulk insert — that's the "notifica a los profesionales enlazados a ellos" requirement,
// done for real (a real row they can see/respond to), though delivery is "check your
// dashboard", not a push notification (no such infra exists in this app yet).
// GetMyEventsAsync/GetMyInvitationsAsync do N+1 lookups to resolve names (company/
// professional usernames) — same accepted trade-off as ServiceService.GetAllForAdminAsync
// and ReportService.cs: small internal listings, not public/high-traffic endpoints.
// Entities connected: Event.cs, EventInvitation.cs, Professional.cs, User.cs
// Tables related: TBL_EVENTS, TBL_EVENT_INVITATIONS, TBL_PROFESSIONALS, TBL_USERS
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventInvitationRepository _invitationRepository;
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogService _auditLogService;

    public EventService(
        IEventRepository eventRepository,
        IEventInvitationRepository invitationRepository,
        IProfessionalRepository professionalRepository,
        IUserRepository userRepository,
        IAuditLogService auditLogService)
    {
        _eventRepository = eventRepository;
        _invitationRepository = invitationRepository;
        _professionalRepository = professionalRepository;
        _userRepository = userRepository;
        _auditLogService = auditLogService;
    }

    public async Task<EventResponse> CreateAsync(int companyUserId, string companyUsername, CreateEventRequest request)
    {
        var @event = new Event
        {
            CompanyUserId = companyUserId,
            Title = request.Title,
            Description = request.Description,
            NotIncluded = request.NotIncluded,
            EventDate = request.EventDate,
            Location = request.Location,
            ProfessionalsNeededCount = request.ProfessionalsNeededCount
        };

        await _eventRepository.AddAsync(@event);

        var workers = await _professionalRepository.GetByCompanyUserIdAsync(companyUserId);
        var invitations = workers.Select(worker => new EventInvitation
        {
            EventId = @event.Id,
            ProfessionalUserId = worker.UserId,
            Status = EventInvitationStatus.Pending
        }).ToList();

        if (invitations.Count > 0)
        {
            await _invitationRepository.AddRangeAsync(invitations);
        }

        await _auditLogService.LogAdminActionAsync(
            actorUserId: companyUserId,
            actorUsername: companyUsername,
            actorRole: UserRole.Company,
            action: "EVENT_CREATED",
            targetEntityType: "Event",
            targetEntityId: @event.Id,
            details: $"Title={@event.Title}, InvitationsSent={invitations.Count}");

        return new EventResponse
        {
            Id = @event.Id,
            CompanyUserId = @event.CompanyUserId,
            Title = @event.Title,
            Description = @event.Description,
            NotIncluded = @event.NotIncluded,
            EventDate = @event.EventDate,
            Location = @event.Location,
            ProfessionalsNeededCount = @event.ProfessionalsNeededCount,
            CreatedAt = @event.CreatedAt,
            TotalInvited = invitations.Count,
            AcceptedCount = 0,
            DeclinedCount = 0,
            PendingCount = invitations.Count
        };
    }

    public async Task<IReadOnlyList<EventResponse>> GetMyEventsAsync(int callerUserId, UserRole callerRole)
    {
        var companyUserId = await ResolveCompanyUserIdAsync(callerUserId, callerRole);
        var events = await _eventRepository.GetByCompanyUserIdAsync(companyUserId);
        var result = new List<EventResponse>(events.Count);

        foreach (var @event in events)
        {
            var invitations = await _invitationRepository.GetByEventIdAsync(@event.Id);

            result.Add(new EventResponse
            {
                Id = @event.Id,
                CompanyUserId = @event.CompanyUserId,
                Title = @event.Title,
                Description = @event.Description,
                NotIncluded = @event.NotIncluded,
                EventDate = @event.EventDate,
                Location = @event.Location,
                ProfessionalsNeededCount = @event.ProfessionalsNeededCount,
                CreatedAt = @event.CreatedAt,
                TotalInvited = invitations.Count,
                AcceptedCount = invitations.Count(i => i.Status == EventInvitationStatus.Accepted),
                DeclinedCount = invitations.Count(i => i.Status == EventInvitationStatus.Declined),
                PendingCount = invitations.Count(i => i.Status == EventInvitationStatus.Pending)
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<EventInvitationResponse>> GetMyInvitationsAsync(int professionalUserId)
    {
        var invitations = await _invitationRepository.GetByProfessionalUserIdAsync(professionalUserId);
        var result = new List<EventInvitationResponse>(invitations.Count);

        foreach (var invitation in invitations)
        {
            result.Add(await MapInvitationToResponseAsync(invitation));
        }

        return result;
    }

    public async Task<EventInvitationResponse> RespondToInvitationAsync(int professionalUserId, int invitationId, bool accept)
    {
        var invitation = await _invitationRepository.GetByIdAsync(invitationId)
            ?? throw new NotFoundException($"Invitation with id {invitationId} was not found.");

        if (invitation.ProfessionalUserId != professionalUserId)
        {
            throw new ForbiddenException("You can only respond to your own invitations.");
        }

        if (invitation.Status != EventInvitationStatus.Pending)
        {
            throw new ConflictException("You already responded to this invitation.");
        }

        invitation.Status = accept ? EventInvitationStatus.Accepted : EventInvitationStatus.Declined;
        invitation.RespondedAt = DateTime.UtcNow;
        await _invitationRepository.UpdateAsync(invitation);

        return await MapInvitationToResponseAsync(invitation);
    }

    private async Task<EventInvitationResponse> MapInvitationToResponseAsync(EventInvitation invitation)
    {
        var @event = await _eventRepository.GetByIdAsync(invitation.EventId)
            ?? throw new NotFoundException($"Event with id {invitation.EventId} was not found.");

        var company = await _userRepository.GetByIdAsync(@event.CompanyUserId);

        return new EventInvitationResponse
        {
            Id = invitation.Id,
            EventId = @event.Id,
            EventTitle = @event.Title,
            EventDescription = @event.Description,
            EventNotIncluded = @event.NotIncluded,
            EventDate = @event.EventDate,
            EventLocation = @event.Location,
            CompanyName = company is null ? string.Empty : $"{company.FirstName} {company.LastName}",
            Status = invitation.Status,
            CreatedAt = invitation.CreatedAt,
            RespondedAt = invitation.RespondedAt
        };
    }

    // Same resolution rule as UserService/ProfessionalService: a Company acts on its own
    // behalf; a Manager acts on behalf of whichever company it belongs to.
    private async Task<int> ResolveCompanyUserIdAsync(int callerUserId, UserRole callerRole)
    {
        if (callerRole == UserRole.Company)
        {
            return callerUserId;
        }

        var caller = await _userRepository.GetByIdAsync(callerUserId)
            ?? throw new NotFoundException($"User with id {callerUserId} was not found.");

        return caller.ManagedByCompanyUserId
            ?? throw new ForbiddenException("This Manager account has no company linked to it.");
    }
}
