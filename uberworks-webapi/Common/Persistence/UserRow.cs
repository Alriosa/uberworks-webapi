// =====================================================================================
// FILE SUMMARY
// What it does: The "raw" shape of a row read from TBL_USERS — every column exactly as
//               Dapper reads it, with CL_ROLE/CL_STATUS still as plain strings (NOT the
//               UserRole/UserStatus enums). This exists because Dapper cannot be trusted to
//               convert those columns into the real User.cs entity directly — see
//               UserRoleMapper.cs for why. Any Repository that needs a User via a SQL JOIN
//               (ProfessionalRepository.cs, ServiceProfessionalRepository.cs,
//               PasswordResetTokenRepository.cs) selects into this class first, then calls
//               ToUser() to get the real entity with its enums properly converted.
//               UserRepository.cs does the exact same thing for its own direct queries.
// Entities connected: User.cs (ToUser() converts this into one)
// Tables related: TBL_USERS
// =====================================================================================
using uberworks_webapi.Models.Entities;

namespace uberworks_webapi.Common.Persistence;

public class UserRow
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string? FacebookId { get; set; }
    public bool IsPasswordSet { get; set; }

    /// <summary>
    /// Only present when the SELECT that fills this row explicitly includes it
    /// (UserRepository.cs does; the JOIN-based repos that reuse this class don't need it and
    /// simply leave it null/default) — see User.ManagedByCompanyUserId.
    /// </summary>
    public int? ManagedByCompanyUserId { get; set; }

    public User ToUser() => new()
    {
        Id = Id,
        Username = Username,
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        Phone = Phone,
        PasswordHash = PasswordHash,
        Role = UserRoleMapper.FromDb(Role),
        Status = UserStatusMapper.FromDb(Status),
        RegistrationDate = RegistrationDate,
        FacebookId = FacebookId,
        IsPasswordSet = IsPasswordSet,
        ManagedByCompanyUserId = ManagedByCompanyUserId
    };
}
