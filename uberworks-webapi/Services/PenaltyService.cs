// =====================================================================================
// FILE SUMMARY
// What it does: Implements IPenaltyService.cs. CreateAsync validates the target user exists,
//               then — since TBL_PENALTIES.CL_END_DATE is NOT NULL — fills in a far-future
//               sentinel (PermanentEndDateSentinel) when Type is Permanent instead of leaving
//               EndDate null, and requires the caller to supply a real EndDate when Type is
//               Temporary. MapToResponseAsync resolves the username via IUserRepository
//               (N+1 lookups — this app's traffic is nowhere near the scale where that would
//               matter, same reasoning as ServiceService.cs) and computes IsActive so the
//               WebApp doesn't have to duplicate that logic.
// Entities connected: Penalty.cs, User.cs
// Tables related: TBL_PENALTIES
// =====================================================================================
using uberworks_webapi.Common.Enums;
using uberworks_webapi.Common.Exceptions;
using uberworks_webapi.Models.DTOs.Requests;
using uberworks_webapi.Models.DTOs.Responses;
using uberworks_webapi.Models.Entities;
using uberworks_webapi.Repositories.Interfaces;
using uberworks_webapi.Services.Interfaces;

namespace uberworks_webapi.Services;

public class PenaltyService : IPenaltyService
{
    // TBL_PENALTIES.CL_END_DATE is NOT NULL, so a Permanent penalty (which has no real end
    // date) gets this sentinel instead — far enough in the future to never be reached, well
    // within SQL Server's DATETIME range (up to year 9999).
    private static readonly DateTime PermanentEndDateSentinel = new(9999, 12, 31);

    private readonly IPenaltyRepository _penaltyRepository;
    private readonly IUserRepository _userRepository;

    public PenaltyService(IPenaltyRepository penaltyRepository, IUserRepository userRepository)
    {
        _penaltyRepository = penaltyRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<PenaltyResponse>> GetForUserAsync(int userId)
    {
        var penalties = await _penaltyRepository.GetByUserIdAsync(userId);
        var user = await _userRepository.GetByIdAsync(userId);

        return penalties.Select(p => MapToResponse(p, user?.Username ?? string.Empty)).ToList();
    }

    public async Task<IReadOnlyList<PenaltyResponse>> GetAllAsync()
    {
        var penalties = await _penaltyRepository.GetAllAsync();
        var result = new List<PenaltyResponse>(penalties.Count);

        foreach (var penalty in penalties)
        {
            var user = await _userRepository.GetByIdAsync(penalty.UserId);
            result.Add(MapToResponse(penalty, user?.Username ?? string.Empty));
        }

        return result;
    }

    public async Task<PenaltyResponse> CreateAsync(CreatePenaltyRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException($"User with id {request.UserId} was not found.");

        if (request.Type == PenaltyType.Temporary && request.EndDate is null)
        {
            throw new ConflictException("EndDate is required when Type is Temporary.");
        }

        var penalty = new Penalty
        {
            UserId = request.UserId,
            Type = request.Type,
            Reason = request.Reason,
            StartDate = DateTime.UtcNow,
            EndDate = request.Type == PenaltyType.Permanent ? PermanentEndDateSentinel : request.EndDate!.Value
        };

        await _penaltyRepository.AddAsync(penalty);

        return MapToResponse(penalty, user.Username);
    }

    private static PenaltyResponse MapToResponse(Penalty penalty, string username) => new()
    {
        Id = penalty.Id,
        UserId = penalty.UserId,
        Username = username,
        Type = penalty.Type,
        Reason = penalty.Reason,
        StartDate = penalty.StartDate,
        EndDate = penalty.EndDate,
        IsActive = penalty.Type == PenaltyType.Permanent || penalty.EndDate > DateTime.UtcNow
    };
}
