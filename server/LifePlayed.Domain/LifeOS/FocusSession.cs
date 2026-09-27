using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.LifeOS;

public enum FocusSessionStatus
{
    Completed = 0,
    Interrupted = 1,
}

public sealed record FocusSession(
    EntityId FocusSessionId,
    EntityId AccountId,
    EntityId? ActionId,
    TimeSpan PlannedDuration,
    TimeSpan? ActualDuration,
    FocusSessionStatus Status,
    DateTimeOffset? StartedAt,
    DateTimeOffset? EndedAt,
    EntityVersion Version);
