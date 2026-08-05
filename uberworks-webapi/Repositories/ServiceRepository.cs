// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IServiceRepository.cs. The private Query()
//               method centralizes ".Include(s => s.WorkType)" (so the category name can be
//               shown without a separate query), and the other methods reuse it.
// Entities connected: Service.cs, WorkType.cs (via Include)
// Tables related: TBL_SERVICES, TBL_WORKTYPES
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;

    public ServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<Service> Query() =>
        _context.Services.Include(s => s.WorkType);

    public Task<Service?> GetByIdAsync(int id) =>
        Query().FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IReadOnlyList<Service>> GetOpenAsync() =>
        await Query()
            .Where(s => s.Status == ServiceStatus.Pending)
            .OrderByDescending(s => s.RequestDate)
            .ToListAsync();

    public async Task<IReadOnlyList<Service>> GetByClientIdAsync(int clientId) =>
        await Query()
            .Where(s => s.ClientId == clientId)
            .OrderByDescending(s => s.RequestDate)
            .ToListAsync();

    public async Task AddAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Service service)
    {
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
    }
}
