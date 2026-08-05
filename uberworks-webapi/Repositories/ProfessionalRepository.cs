// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Implementación real de IProfessionalRepository.cs. Nota el .Include(p => p.User)
//           en las consultas: le dice a EF Core "cuando traigas el Professional, trae
//           también su User relacionado en la misma consulta" (evita tener que hacer una
//           segunda consulta separada para leer el nombre/email del usuario).
// Entidades relacionadas: Professional.cs, User.cs (vía Include)
// Tablas relacionadas: TBL_PROFESSIONALS, TBL_USERS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class ProfessionalRepository : IProfessionalRepository
{
    private readonly AppDbContext _context;

    public ProfessionalRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Professional?> GetByIdAsync(int id) =>
        _context.Professionals.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);

    public Task<Professional?> GetByUserIdAsync(int userId) =>
        _context.Professionals.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);

    public Task<bool> ExistsByUserIdAsync(int userId) =>
        _context.Professionals.AnyAsync(p => p.UserId == userId);

    public async Task AddAsync(Professional professional)
    {
        _context.Professionals.Add(professional);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Professional professional)
    {
        _context.Professionals.Update(professional);
        await _context.SaveChangesAsync();
    }
}
