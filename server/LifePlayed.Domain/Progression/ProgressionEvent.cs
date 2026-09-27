using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.Progression;

public sealed record ProgressionEvent(
    EntityId EventId,
    EntityId AccountId,
    string SourceType,
    EntityId SourceId,
    string RuleVersion,
    int AccountXpDelta,
    IReadOnlyDictionary<LifeSkill, int> SkillXpDeltas,
    DateTimeOffset OccurredAt);
