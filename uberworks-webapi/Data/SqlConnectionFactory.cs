// =====================================================================================
// FILE SUMMARY
// What it does: The real implementation of IDbConnectionFactory.cs. Reads
//               ConnectionStrings:DefaultConnection from appsettings.json (same setting EF
//               Core used to read) and hands back a plain Microsoft.Data.SqlClient
//               connection — no ORM, no query translation, just the raw ADO.NET connection
//               every Repository runs its own SQL against.
// Entities connected: None — this is plumbing, not a table
// Tables related: None
// =====================================================================================
using System.Data;
using Microsoft.Data.SqlClient;

namespace uberworks_webapi.Data;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured in appsettings.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
