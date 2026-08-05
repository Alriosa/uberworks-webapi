// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IUserRepository.cs — this is where
//               AppDbContext.cs is actually used to talk to SQL Server (e.g.
//               _context.Users.FirstOrDefaultAsync(...)). EF Core translates each method
//               here into a real SQL query.
// Entities connected: User.cs
// Tables related: TBL_USERS
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

    public Task<bool> ExistsByUsernameAsync(string username) =>
        _context.Users.AnyAsync(u => u.Username == username);

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
