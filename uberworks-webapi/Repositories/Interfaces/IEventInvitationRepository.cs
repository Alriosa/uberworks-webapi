// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for EventInvitation — get by id, list an event's
// invitations (for the Company/Manager tally), list a professional's own invitations,
// bulk-create (one per professional when an Event is created), update (respond).
// Entities connected: EventInvitation.cs
// Tables related: TBL_EVENT_INVITATIONS (indirectly, via EventInvitationRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IEventInvitationRepository
{
    Task<EventInvitation?> GetByIdAsync(int id);
    Task<IReadOnlyList<EventInvitation>> GetByEventIdAsync(int eventId);
    Task<IReadOnlyList<EventInvitation>> GetByProfessionalUserIdAsync(int professionalUserId);
    Task AddRangeAsync(IEnumerable<EventInvitation> invitations);
    Task UpdateAsync(EventInvitation invitation);
}
