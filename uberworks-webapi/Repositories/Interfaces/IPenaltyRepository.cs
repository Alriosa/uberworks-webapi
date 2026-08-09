// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for Penalty (a sanction/warning applied to a user).
//               GetByUserIdAsync backs a user's own "Advertencias" view (Professional
//               dashboard today, per explicit request — "también debe generar su modal que
//               abra e indique qué advertencias se le dieron a la persona"). GetAllAsync
//               backs a future Admin-wide sanctions listing. AddAsync is how
//               Admin/MasterAdmin apply a new penalty (PenaltyService.CreateAsync). No
//               update/delete — a penalty is a historical record, not something that gets
//               edited after the fact.
// Entities connected: Penalty.cs
// Tables related: TBL_PENALTIES (indirectly, via PenaltyRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IPenaltyRepository
{
    Task<IReadOnlyList<Penalty>> GetByUserIdAsync(int userId);
    Task<IReadOnlyList<Penalty>> GetAllAsync();
    Task AddAsync(Penalty penalty);
}
