using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.LifeOS;

public enum HabitOccurrenceStatus
{
    Pending = 0,
    Completed = 1,
    Skipped = 2,
    Rested = 3,
}

public sealed record HabitOccurrence(
    EntityId HabitOccurrenceId,
    EntityId HabitDefinitionId,
    EntityId AccountId,
    DateTimeOffset ScheduledAt,
    HabitOccurrenceStatus Status,
    DateTimeOffset? CompletedAt,
    EntityVersion Version);
