// =====================================================================================
// FILE SUMMARY
// What it does: Converts ServiceProfessionalStatus to/from the text value stored in
//               TBL_SERVICE_PROFESSIONALS.CL_STATUS. Special case (the diagram's values use
//               spaces — "UNDER NEGOTIATION", "IN PROGRESS" — which a plain
//               ToUpperInvariant() can't produce on its own). Called explicitly by
//               ServiceProfessionalRepository.cs — see UserRoleMapper.cs for why this is a
//               plain static method call instead of a registered Dapper TypeHandler.
// Entities connected: ServiceProfessional.cs
// Tables related: TBL_SERVICE_PROFESSIONALS.CL_STATUS
// =====================================================================================
using uberworks_webapi.Common.Enums;

namespace uberworks_webapi.Common.Persistence;

public static class ServiceProfessionalStatusMapper
{
    public static string ToDb(ServiceProfessionalStatus value) => value switch
    {
        ServiceProfessionalStatus.UnderNegotiation => "UNDER NEGOTIATION",
        ServiceProfessionalStatus.InProgress => "IN PROGRESS",
        _ => value.ToString().ToUpperInvariant()
    };

    public static ServiceProfessionalStatus FromDb(string value) => value switch
    {
        "UNDER NEGOTIATION" => ServiceProfessionalStatus.UnderNegotiation,
        "IN PROGRESS" => ServiceProfessionalStatus.InProgress,
        _ => Enum.Parse<ServiceProfessionalStatus>(value, ignoreCase: true)
    };
}
