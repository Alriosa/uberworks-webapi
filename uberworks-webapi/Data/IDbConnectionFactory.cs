// =====================================================================================
// FILE SUMMARY
// What it does: Contract for getting a database connection. Every Repository depends on
//               this instead of building a SqlConnection itself, so the connection string
//               lives in exactly one place (SqlConnectionFactory.cs) and Repositories are
//               easy to test later (a fake factory could return an in-memory connection).
// Entities connected: None — this is plumbing, not a table
// Tables related: None
// =====================================================================================
using System.Data;

namespace uberworks_webapi.Data;

public interface IDbConnectionFactory
{
    /// <summary>
    /// Returns a NEW, unopened connection every time. Callers are responsible for opening
    /// it (Dapper's extension methods do this automatically) and disposing it (a `using`
    /// block) — connections are cheap to open/close because ADO.NET pools them internally,
    /// so there's no need to share one connection across requests.
    /// </summary>
    IDbConnection CreateConnection();
}
