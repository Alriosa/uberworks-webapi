// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Common/Enums/AuthProvider.cs. Serialized by name
//               ("Google"/"Facebook") in ExternalLoginRequest.cs, so both sides must keep the
//               exact same member names for JSON (de)serialization to line up.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public enum AuthProvider
{
    Google,
    Facebook
}
