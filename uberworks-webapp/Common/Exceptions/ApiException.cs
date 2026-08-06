// =====================================================================================
// FILE SUMMARY
// What it does: Thrown by Services/ApiClient/*.cs whenever the API responds with a
//               non-success status code. Carries the real HTTP status code and the actual
//               error message the API sent back (see Models/ApiContracts/ApiErrorResponse.cs),
//               so Controllers can show the user something meaningful ("Invalid email or
//               password.") instead of a generic "something went wrong".
// Entities connected: None — WebApp has no database entities
// Tables related: None
// =====================================================================================
using System.Net;

namespace uberworks_webapp.Common.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
