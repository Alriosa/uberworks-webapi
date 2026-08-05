// =====================================================================================
// FILE SUMMARY
// What it does: Stores a user's reward points balance (1 row per user, updated over time —
//               not an event history). Doesn't have a Repository/Service/Controller built yet.
// Entities connected: User.cs (1:1)
// Tables related: TBL_REWARDS (mapping in Data/Configurations/RewardConfiguration.cs)
// =====================================================================================
namespace uberworks_webapi.Models.Entities;

/// <summary>
/// Maps to TBL_REWARDS.
/// </summary>
public class Reward
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public int Points { get; set; }
    public DateTime LastUpdateDate { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
