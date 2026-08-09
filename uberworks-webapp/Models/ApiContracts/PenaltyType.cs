// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Common/Enums/PenaltyType.cs — same member order
//               required, since enums serialize as plain integers over JSON.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public enum PenaltyType
{
    Temporary,
    Permanent
}
