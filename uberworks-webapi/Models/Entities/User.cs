// =====================================================================================
// FILE SUMMARY
// What it does: Represents a row in the users table. It's the "root" entity of the whole
//               system — Client and Professional are really Users with extra data. EF Core
//               uses this class to read/write to the database automatically (we never write
//               raw SQL: EF translates this class into SELECT/INSERT/UPDATE).
// Entities connected: Professional.cs (1:1, own profile; 1:N as the owning Company, via
//                      Professional.CompanyUserId), Service.cs (1:N as client),
//                      Review.cs (1:N as client), Chat.cs (1:N as client),
//                      Penalty.cs (1:N), Reward.cs (1:1)
// Tables related: TBL_USERS (full mapping in Data/Configurations/UserConfiguration.cs)
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
