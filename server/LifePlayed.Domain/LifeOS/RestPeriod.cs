using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.LifeOS;

public enum RestPeriodStatus
{
    Scheduled = 0,
    Active = 1,
    Completed = 2,
    Cancelled = 3,
}

public sealed record RestPeriod(
    EntityId RestPeriodId,
    EntityId AccountId,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    string? Note,
    RestPeriodStatus Status,
    EntityVersion Version);
