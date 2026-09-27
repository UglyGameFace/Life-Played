using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.LifeOS;

public sealed record LifeAction(
    EntityId ActionId,
    EntityId AccountId,
    string Title,
    ActionStatus Status,
    TimeSpan? ExpectedDuration,
    DateTimeOffset? DueAt,
    DateTimeOffset? CompletedAt,
    EntityVersion Version);
