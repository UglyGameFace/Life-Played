using System.Text.Json;

namespace LifePlayed.Contracts.Sync;

public static class SyncMutationTypes
{
    public const string ActionCreate = "action.create";
    public const string ActionEdit = "action.edit";
    public const string ActionComplete = "action.complete";
    public const string QuestCreate = "quest.create";
    public const string QuestEdit = "quest.edit";
    public const string CampaignCreate = "campaign.create";
    public const string CampaignEdit = "campaign.edit";
    public const string CampaignPhaseCreate = "campaign_phase.create";
    public const string CampaignPhaseEdit = "campaign_phase.edit";
    public const string HabitCreate = "habit.create";
    public const string HabitEdit = "habit.edit";
    public const string HabitOccurrenceComplete = "habit_occurrence.complete";
    public const string FocusSessionRecord = "focus_session.record";
    public const string RestPeriodCreate = "rest_period.create";
    public const string RestPeriodEdit = "rest_period.edit";
}

public static class SyncMutationStatuses
{
    public const string Applied = "applied";
    public const string Conflict = "conflict";
    public const string Rejected = "rejected";
}

public sealed record SyncBatchRequest(
    Guid AccountId,
    Guid DeviceId,
    long? SinceCursor,
    IReadOnlyList<SyncMutationRequest> Mutations);

public sealed record SyncMutationRequest(
    Guid MutationId,
    Guid EntityId,
    string Type,
    long BaseVersion,
    DateTimeOffset ClientTimestamp,
    JsonElement Payload);

public sealed record SyncRewardReceipt(
    int AccountXp,
    IReadOnlyDictionary<string, int> SkillXp);

public sealed record SyncMutationResult(
    Guid MutationId,
    Guid EntityId,
    string Status,
    bool Replayed,
    long? CanonicalVersion,
    string? ErrorCode,
    string? ErrorMessage,
    JsonElement? CanonicalData,
    SyncRewardReceipt? Reward);

public sealed record SyncChange(
    long Cursor,
    string EntityType,
    Guid EntityId,
    long Version,
    DateTimeOffset ChangedAt);

public sealed record SyncBatchResponse(
    IReadOnlyList<SyncMutationResult> Results,
    IReadOnlyList<SyncChange> Changes,
    long NextCursor);
