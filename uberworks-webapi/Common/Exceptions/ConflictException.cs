// =====================================================================================
// FILE SUMMARY
// What it does: Custom exception for when the requested operation clashes with the current
//               state of the data (e.g. registering an email that already exists, or trying
//               to accept a proposal on a Service that is no longer pending).
//               Middleware/ExceptionHandlingMiddleware.cs converts it to HTTP 409 Conflict.
// Entities connected: None directly — thrown by several Services
//                      (UserService, WorkTypeService, ServiceProfessionalService, etc.)
// Tables related: None (it's a plain C# class, doesn't touch the database)
// =====================================================================================
namespace uberworks_webapi.Common.Exceptions;

/// <summary>
/// Thrown when the operation clashes with the current state of the data (e.g. duplicate email).
/// The middleware translates it to HTTP 409.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
