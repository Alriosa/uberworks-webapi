// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Implementación real de IWorkTypeRepository.cs. HasServicesAsync() es la
//           consulta que usa WorkTypeService.cs para bloquear el borrado de una categoría
//           que ya está siendo usada por algún Service (evitaría dejar Services huérfanos).
// Entidades relacionadas: WorkType.cs, Service.cs (para verificar dependencias antes de borrar)
// Tablas relacionadas: TBL_WORKTYPES, TBL_SERVICES
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class WorkTypeRepository : IWorkTypeRepository
{
    private readonly AppDbContext _context;

    public WorkTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<WorkType?> GetByIdAsync(int id) =>
        _context.WorkTypes.FirstOrDefaultAsync(w => w.Id == id);

    public async Task<IReadOnlyList<WorkType>> GetAllAsync() =>
        await _context.WorkTypes.OrderBy(w => w.Name).ToListAsync();

    public Task<bool> ExistsByNameAsync(string name, int? excludeId = null) =>
        _context.WorkTypes.AnyAsync(w => w.Name == name && (excludeId == null || w.Id != excludeId));

    public Task<bool> HasServicesAsync(int id) =>
        _context.Services.AnyAsync(s => s.WorkTypeId == id);

    public async Task AddAsync(WorkType workType)
    {
        _context.WorkTypes.Add(workType);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WorkType workType)
    {
        _context.WorkTypes.Update(workType);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(WorkType workType)
    {
        _context.WorkTypes.Remove(workType);
        await _context.SaveChangesAsync();
    }
}
