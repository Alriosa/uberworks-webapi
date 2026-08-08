// =====================================================================================
// FILE SUMMARY
// What it does: Represents a row in TBL_USERS. It's the "root" entity of the whole system —
//               Client and Professional are really Users with extra data. Data access is via
//               Dapper (see Repositories/UserRepository.cs and Common/Persistence/UserRow.cs),
//               not EF Core — this class is just the in-memory shape the rest of the app
//               works with. FacebookId/IsPasswordSet exist to support signing in via Google
//               or Facebook (see UserService.ExternalLoginAsync): FacebookId links a Facebook
//               account to this User once seen; IsPasswordSet is false for accounts
//               auto-created via Google/Facebook (they get a random, unknown PasswordHash) and
//               becomes true once the person sets a real password (POST /api/users/set-password)
//               — the WebApp shows a "create your password" modal on every login until then.
// Entities connected: Professional.cs (1:1, own profile; 1:N as the owning Company, via
//                      Professional.CompanyUserId), Service.cs (1:N as client),
//                      Review.cs (1:N as client), Chat.cs (1:N as client),
//                      Penalty.cs (1:N), Reward.cs (1:1)
// Tables related: TBL_USERS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_USERS.
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>
    /// Public handle shown to other users (e.g. in a Professional's public profile).
    /// Not used for login — that's still Email. Unique, just like Email.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    /// <summary>
    /// Password hash (CL_PASSWORD). Never stored as plain text.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime RegistrationDate { get; set; }

    /// <summary>
    /// Facebook's own user ID, saved the first time this person signs in with Facebook —
    /// links the Facebook account to this User so future Facebook logins are recognized as
    /// the same person even if their Facebook email ever changes. Null for anyone who has
    /// never signed in with Facebook.
    /// </summary>
    public string? FacebookId { get; set; }

    /// <summary>
    /// False for accounts auto-created via Google/Facebook sign-in (PasswordHash is a random
    /// value nobody knows) until the person sets a real password. Always true for accounts
    /// created through the normal Register form or by an Admin. See
    /// UserService.ExternalLoginAsync/SetPasswordAsync.
    /// </summary>
    public bool IsPasswordSet { get; set; } = true;

    // Navigation properties
    public Professional? Professional { get; set; }

    /// <summary>
    /// Workers this User created while acting as a Company (Role=Company). Empty for
    /// every other role — see Professional.CompanyUserId.
    /// </summary>
    public ICollection<Professional> ManagedWorkers { get; set; } = new List<Professional>();

    public ICollection<Service> ServicesRequested { get; set; } = new List<Service>();
    public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
    public ICollection<Chat> ChatsAsClient { get; set; } = new List<Chat>();
    public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
    public Reward? Reward { get; set; }
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
}
