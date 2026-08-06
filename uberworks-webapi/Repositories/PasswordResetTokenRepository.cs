// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IPasswordResetTokenRepository.cs.
//               GetValidByTokenHashAsync filters by Used == false AND ExpiresAt > UtcNow
//               directly in the query — an expired or already-used token simply doesn't
//               come back, same as if it never existed.
// Entities connected: PasswordResetToken.cs, User.cs (via Include)
// Tables related: TBL_PASSWORD_RESET_TOKENS
// =====================================================================================
using Microsoft.EntityFrameworkCore;
using uberworks_webapi.Data;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;

namespace uberworks_webapi.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly AppDbContext _context;

    public PasswordResetTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<PasswordResetToken?> GetValidByTokenHashAsync(string tokenHash) =>
        _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.Used && t.ExpiresAt > DateTime.UtcNow);

    public async Task AddAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Update(token);
        await _context.SaveChangesAsync();
    }
}
