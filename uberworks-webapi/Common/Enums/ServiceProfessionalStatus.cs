// =====================================================================================
// FILE SUMMARY
// What it does: Defines what stage ONE professional's specific proposal/negotiation on ONE
//               specific Service is at (each professional who bid has their own row with
//               its own status: under negotiation, accepted, rejected, in progress, completed).
// Entities connected: ServiceProfessional.cs (the Status property is of this type)
// Tables related: TBL_SERVICE_PROFESSIONALS.CL_STATUS
// =====================================================================================
namespace uberworks_webapi.Common.Enums;

/// <summary>
/// Maps to the values used in TBL_SERVICE_PROFESSIONALS.CL_STATUS.
/// </summary>
public enum ServiceProfessionalStatus
{
    UnderNegotiation,
    Accepted,
    Rejected,
    InProgress,
    Completed
}
