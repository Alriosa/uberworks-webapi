// =====================================================================================
// FILE SUMMARY
// What it does: WebApp's own copy of the API's UserStatus enum (Common/Enums/UserStatus.cs
//               in uberworks-webapi). Same member-order rule as UserRole.cs in this folder
//               applies — enums serialize as plain integers by default. Deleted was
//               appended at the end for the same reason.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public enum UserStatus
{
    Active,
    Suspended,
    Penalized,
    Deleted
}
