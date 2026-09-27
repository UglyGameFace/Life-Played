using LifePlayed.Domain.Common;
using LifePlayed.Domain.Progression;

namespace LifePlayed.Domain.LifeOS;

public enum HabitStatus
{
    Active = 0,
    Paused = 1,
    Archived = 2,
}

public sealed record HabitDefinition(
    EntityId HabitDefinitionId,
    EntityId AccountId,
    string Title,
    string RecurrenceRule,
    HabitStatus Status,
    TimeSpan? ExpectedDuration,
    EntityVersion Version,
    LifeSkill? PrimarySkill = null);
