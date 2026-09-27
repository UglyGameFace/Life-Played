using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.Progression;

public enum RewardGrantState
{
    Pending = 0,
    Granted = 1,
    Reversed = 2,
}

public sealed record RewardGrant(
    EntityId GrantId,
    EntityId AccountId,
    string SourceType,
    EntityId SourceId,
    string RuleVersion,
    string IdempotencyKey,
    RewardGrantState State,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ReversedAt = null);
