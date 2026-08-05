// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IServiceProfessionalRepository.cs. Query()
//               includes Professional and, within it, its User (".ThenInclude(p => p.User)")
//               because almost every time a proposal is read, the professional's name or
//               UserId is also needed (to compare against who's making the request).
//               GetAcceptedForServiceAsync() is key for the "exact address" rule (see
//               Services/ServiceService.cs).
// Entities connected: ServiceProfessional.cs, Professional.cs, User.cs (via Include)
// Tables related: TBL_SERVICE_PROFESSIONALS, TBL_PROFESSIONALS, TBL_USERS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class ServiceProfessionalRepository : IServiceProfessionalRepository
{
    private readonly AppDbContext _context;

    public ServiceProfessionalRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<ServiceProfessional> Query() =>
        _context.ServiceProfessionals
            .Include(sp => sp.Professional)
            .ThenInclude(p => p.User);

    public Task<ServiceProfessional?> GetByIdAsync(int id) =>
        Query().FirstOrDefaultAsync(sp => sp.Id == id);

    public async Task<IReadOnlyList<ServiceProfessional>> GetByServiceIdAsync(int serviceId) =>
        await Query().Where(sp => sp.ServiceId == serviceId).ToListAsync();

    public Task<ServiceProfessional?> GetAcceptedForServiceAsync(int serviceId) =>
        Query().FirstOrDefaultAsync(sp =>
            sp.ServiceId == serviceId && sp.Status == ServiceProfessionalStatus.Accepted);

    public Task<bool> ExistsProposalAsync(int serviceId, int professionalId) =>
        _context.ServiceProfessionals.AnyAsync(sp =>
            sp.ServiceId == serviceId && sp.ProfessionalId == professionalId);

    public async Task AddAsync(ServiceProfessional serviceProfessional)
    {
        _context.ServiceProfessionals.Add(serviceProfessional);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServiceProfessional serviceProfessional)
    {
        _context.ServiceProfessionals.Update(serviceProfessional);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<ServiceProfessional> serviceProfessionals)
    {
        _context.ServiceProfessionals.UpdateRange(serviceProfessionals);
        await _context.SaveChangesAsync();
    }
}
