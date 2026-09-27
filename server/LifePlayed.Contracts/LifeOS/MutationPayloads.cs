namespace LifePlayed.Contracts.LifeOS;

public sealed record ActionCreatePayload(
    string Title,
    int? ExpectedMinutes,
    DateTimeOffset? DueAt,
    int? PrimarySkill);

public sealed record ActionEditPayload(
    string Title,
    int? ExpectedMinutes,
    DateTimeOffset? DueAt,
    int? PrimarySkill);

public sealed record QuestCreatePayload(
    string Title,
    DateTimeOffset? DueAt,
    Guid? CampaignId);

public sealed record QuestEditPayload(
    string Title,
    DateTimeOffset? DueAt,
    Guid? CampaignId);

public sealed record CampaignCreatePayload(
    string Title,
    string? Description,
    DateTimeOffset? StartAt,
    DateTimeOffset? DueAt);

public sealed record CampaignEditPayload(
    string Title,
    string? Description,
    DateTimeOffset? StartAt,
    DateTimeOffset? DueAt);

public sealed record CampaignPhaseCreatePayload(
    Guid CampaignId,
    string Title,
    int Position);

public sealed record CampaignPhaseEditPayload(
    string Title,
    int Position);

public sealed record HabitCreatePayload(
    string Title,
    string RecurrenceRule,
    int? ExpectedMinutes,
    int? PrimarySkill);

public sealed record HabitEditPayload(
    string Title,
    string RecurrenceRule,
    int? ExpectedMinutes,
    int? PrimarySkill);

public sealed record HabitOccurrenceCompletePayload(
    Guid HabitDefinitionId,
    DateTimeOffset ScheduledAt);

public sealed record FocusSessionRecordPayload(
    Guid? ActionId,
    int PlannedMinutes,
    int? ActualMinutes,
    string Status,
    DateTimeOffset? StartedAt,
    DateTimeOffset? EndedAt);

public sealed record RestPeriodCreatePayload(
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    string? Note);

public sealed record RestPeriodEditPayload(
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    string? Note,
    string Status);
