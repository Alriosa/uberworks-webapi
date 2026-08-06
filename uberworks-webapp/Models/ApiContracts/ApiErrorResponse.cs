// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors the JSON shape uberworks-webapi's Middleware/ExceptionHandlingMiddleware.cs
//               produces for every error response (statusCode + message). UsersApiClient.cs
//               deserializes this on non-success HTTP responses so the WebApp can show the
//               API's actual error message (e.g. "Invalid email or password.") instead of a
//               generic failure.
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
}
