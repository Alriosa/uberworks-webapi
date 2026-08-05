// =====================================================================================
// FILE SUMMARY
// What it does: Custom exception specifically for failed login (email doesn't exist or
//               wrong password). Thrown from UserService.LoginAsync, and
//               Middleware/ExceptionHandlingMiddleware.cs converts it to HTTP 401 Unauthorized.
// Entities connected: User.cs (indirectly, via UserService.LoginAsync)
// Tables related: None (it's a plain C# class, doesn't touch the database)
// =====================================================================================
namespace uberworks_webapi.Common.Exceptions;

/// <summary>
/// Thrown when the login email or password is invalid.
/// The middleware translates it to HTTP 401.
/// </summary>
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message) : base(message)
    {
    }
}
