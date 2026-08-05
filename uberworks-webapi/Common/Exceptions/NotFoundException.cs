// =====================================================================================
// FILE SUMMARY
// What it does: Custom exception. When a Service (business layer) looks something up by id
//               and doesn't find it (e.g. "GetByIdAsync(999)"), instead of returning null and
//               making every Controller remember to check for it, this exception is thrown.
//               Middleware/ExceptionHandlingMiddleware.cs catches it automatically and
//               responds with HTTP 404 to the client (webapp/mobile), without the Controller
//               having to do anything.
// Entities connected: None directly — can be thrown by any Service
//                      (UserService, ProfessionalService, ServiceService, etc.)
// Tables related: None (it's a plain C# class, doesn't touch the database)
// =====================================================================================
namespace uberworks_webapi.Common.Exceptions;

/// <summary>
/// Thrown when a requested resource doesn't exist. The middleware translates it to HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
