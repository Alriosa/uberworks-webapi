// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IProfessionalRepository.cs. Note the
//               .Include(p => p.User) in the queries: it tells EF Core "when you fetch the
//               Professional, also fetch its related User in the same query" (avoids a
//               separate second query just to read the user's name/email).
//               GetByCompanyUserIdAsync backs a Company's "my workers" list.
// Entities connected: Professional.cs, User.cs (via Include)
// Tables related: TBL_PROFESSIONALS, TBL_USERS
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

    public Task<List<Professional>> GetByCompanyUserIdAsync(int companyUserId) =>
        _context.Professionals.Include(p => p.User).Where(p => p.CompanyUserId == companyUserId).ToListAsync();

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
