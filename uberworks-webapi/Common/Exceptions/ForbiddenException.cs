// =====================================================================================
// FILE SUMMARY
// What it does: Custom exception for when the caller IS authenticated (we know who they
//               are) but isn't allowed to do this specific thing (e.g. viewing or editing
//               someone else's profile). Different from InvalidCredentialsException (which
//               is about failed login) — this is about a valid, logged-in user trying to
//               access something that isn't theirs. Middleware/ExceptionHandlingMiddleware.cs
//               converts it to HTTP 403 Forbidden.
// Entities connected: None directly — can be thrown by any Service that enforces
//                      ownership rules (e.g. UserService)
// Tables related: None (it's a plain C# class, doesn't touch the database)
// =====================================================================================
namespace uberworks_webapi.Common.Exceptions;

/// <summary>
/// Thrown when an authenticated user tries to access or modify a resource they don't own
/// and aren't privileged enough to touch. The middleware translates it to HTTP 403.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
