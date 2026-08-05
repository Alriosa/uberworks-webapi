// =====================================================================================
// RESUMEN DEL ARCHIVO
// Qué hace: Implementación real de IUserRepository.cs — aquí sí se usa AppDbContext.cs
//           para hablar con SQL Server (ej. _context.Users.FirstOrDefaultAsync(...)).
//           EF Core traduce cada método de aquí a una consulta SQL real.
// Entidades relacionadas: User.cs
// Tablas relacionadas: TBL_USERS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(int id) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<bool> ExistsByEmailAsync(string email) =>
        _context.Users.AnyAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
