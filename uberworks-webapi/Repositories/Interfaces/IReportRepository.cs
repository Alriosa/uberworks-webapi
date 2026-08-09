// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for Report — get by id, list all (Admin/Support
// dashboards do their own status-bucket filtering on the returned list, since the volume of
// reports is small enough that filtering in C# is simpler than three separate queries),
// create, update. No hard delete — Cancel is a status change, see ReportService.CancelAsync.
// Entities connected: Report.cs
// Tables related: TBL_REPORTS (indirectly, via ReportRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(int id);
    Task<IReadOnlyList<Report>> GetAllAsync();
    Task AddAsync(Report report);
    Task UpdateAsync(Report report);
}
