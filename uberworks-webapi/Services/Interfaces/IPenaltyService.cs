// =====================================================================================
// FILE SUMMARY
// What it does: Contract for the Penalty ("Advertencias"/sanctions) business logic.
//               PenaltiesController.cs depends on this interface. GetForUserAsync backs
//               both a user checking their own record (GET /api/penalties/mine) and
//               Admin/MasterAdmin checking someone else's (GET /api/penalties/user/{userId}).
//               CreateAsync is Admin/MasterAdmin-only — applying a new sanction.
// Entities connected: Penalty.cs
// Tables related: TBL_PENALTIES (indirectly, via PenaltyService.cs)
// =====================================================================================
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;

namespace uberworks_webapi.Services.Interfaces;

public interface IPenaltyService
{
    Task<IReadOnlyList<PenaltyResponse>> GetForUserAsync(int userId);
    Task<IReadOnlyList<PenaltyResponse>> GetAllAsync();
    Task<PenaltyResponse> CreateAsync(CreatePenaltyRequest request);
}
