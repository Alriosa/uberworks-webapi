// =====================================================================================
// FILE SUMMARY
// What it does: Contract for "generate a JWT token from a user". UserService.cs uses it
//               inside LoginAsync() to issue the token the client (webapp/mobile) will use
//               on every future request. See the step-by-step JWT explanation in the chat.
// Entities connected: User.cs (method input)
// Tables related: None directly (doesn't query the database)
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Services.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}
