// =====================================================================================
// FILE SUMMARY
// What it does: WebApp's own copy of the API's UserRole enum (Common/Enums/UserRole.cs in
//               uberworks-webapi). Must keep the exact same member order as the API's
//               enum, because by default ASP.NET Core serializes enums as plain integers
//               in JSON — if the order here doesn't match the API's order, values would be
//               silently misread (e.g. a "Client" could get deserialized as "Admin"). This
//               file exists because WebApp talks to the API over plain HTTP/JSON (see the
//               architecture decision: WebApp is a client like Mobile, not a shared
//               codebase with the API), so it needs its own copy, not a project reference.
//               Manager, Company, and Support were appended at the end, never inserted
//               between existing values, for the same reason.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public enum UserRole
{
    MasterAdmin,
    Admin,
    Client,
    Professional,
    Manager,
    Company,
    Support
}
