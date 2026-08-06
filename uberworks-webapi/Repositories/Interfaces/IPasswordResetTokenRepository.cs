// =====================================================================================
// FILE SUMMARY
// What it does: Data-access contract for PasswordResetToken. GetValidByTokenHashAsync is
//               the one that matters most: it only returns a token that is BOTH unused AND
//               not yet expired — UserService.ResetPasswordAsync treats "nothing found" and
//               "found but invalid" identically, so callers can't tell an expired token from
//               a wrong one.
// Entities connected: PasswordResetToken.cs
// Tables related: TBL_PASSWORD_RESET_TOKENS (indirectly, via PasswordResetTokenRepository.cs)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Repositories.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetValidByTokenHashAsync(string tokenHash);
    Task AddAsync(PasswordResetToken token);
    Task UpdateAsync(PasswordResetToken token);
}
