// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Implementación real de IServiceProfessionalRepository.cs. Query() incluye
//           Professional y, dentro de este, su User (".ThenInclude(p => p.User)") porque
//           casi siempre que se lee una propuesta también se necesita el nombre del
//           profesional o su UserId (para comparar contra quién está haciendo la petición).
//           GetAcceptedForServiceAsync() es clave para la regla de "dirección exacta"
//           (ver Services/ServiceService.cs).
// Entidades relacionadas: ServiceProfessional.cs, Professional.cs, User.cs (vía Include)
// Tablas relacionadas: TBL_SERVICE_PROFESSIONALS, TBL_PROFESSIONALS, TBL_USERS
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
