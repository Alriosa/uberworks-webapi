// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the Company Event/invitation business logic. CreateAsync is
// Company-only (never a Manager — the one explicit difference between their dashboards) and
// auto-generates one EventInvitation per professional currently linked to that company.
// GetMyEventsAsync is usable by both Company and Manager (same company-resolution rule used
// throughout — see ProfessionalService.ResolveCompanyUserIdAsync). GetMyInvitationsAsync/
// RespondToInvitationAsync back the Professional-facing side (Views/Dashboard/
// EventInvitations.cshtml) — "Genera una vista de todo esa parte", per explicit request.
// Entities connected: Event.cs, EventInvitation.cs
// Tables related: TBL_EVENTS, TBL_EVENT_INVITATIONS (indirectly, via EventService.cs)
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IEventService
{
    Task<EventResponse> CreateAsync(int companyUserId, string companyUsername, CreateEventRequest request);
    Task<IReadOnlyList<EventResponse>> GetMyEventsAsync(int callerUserId, UserRole callerRole);
    Task<IReadOnlyList<EventInvitationResponse>> GetMyInvitationsAsync(int professionalUserId);
    Task<EventInvitationResponse> RespondToInvitationAsync(int professionalUserId, int invitationId, bool accept);
}
