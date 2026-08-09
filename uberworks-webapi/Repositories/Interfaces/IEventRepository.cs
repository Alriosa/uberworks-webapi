// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for Event — get by id, list a company's own events,
// create. No update/delete yet — an Event is fire-and-forget once invitations go out.
// Entities connected: Event.cs
// Tables related: TBL_EVENTS (indirectly, via EventRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id);
    Task<IReadOnlyList<Event>> GetByCompanyUserIdAsync(int companyUserId);
    Task AddAsync(Event @event);
}
