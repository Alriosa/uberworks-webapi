// =====================================================================================
// FILE SUMMARY
// What it does: Mirrors uberworks-webapi's Models/DTOs/Responses/PenaltyResponse.cs — a
//               sanction/warning applied to a user. Returned by GET /api/penalties/mine.
//               Backs the Professional dashboard's "Advertencias" modal, per explicit
//               request ("también debe generar su modal que abra e indique qué advertencias
//               se le dieron a la persona o si no en ninguna").
// Entities connected: None — this project has no database entities, only API contracts
// Tables related: None
// =====================================================================================
namespace uberworks_webapp.Models.ApiContracts;

public class PenaltyResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public PenaltyType Type { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
