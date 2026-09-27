using LifePlayed.Domain.Common;
using LifePlayed.Domain.LifeOS;
using LifePlayed.Domain.Progression;
using LifePlayed.Domain.Sync;

namespace LifePlayed.Application.Persistence;

public sealed record SyncChangeRecord(
    long Cursor,
    EntityId AccountId,
    string EntityType,
    EntityId EntityId,
    EntityVersion Version,
    DateTimeOffset ChangedAt);

public interface ILifeOsUnitOfWorkFactory
{
    ValueTask<ILifeOsUnitOfWork> BeginAsync(CancellationToken cancellationToken = default);
}

public interface ISyncChangeReader
{
    ValueTask<IReadOnlyList<SyncChangeRecord>> ReadChangesAsync(
        EntityId accountId,
        long afterCursor,
        int limit,
        CancellationToken cancellationToken = default);
}

public interface ILifeOsUnitOfWork : IAsyncDisposable
{
    ValueTask<LifeAction?> GetActionAsync(EntityId actionId, CancellationToken cancellationToken = default);
    ValueTask InsertActionAsync(LifeAction action, CancellationToken cancellationToken = default);
    ValueTask<bool> UpdateActionAsync(
        LifeAction action,
        EntityVersion expectedVersion,
        CancellationToken cancellationToken = default);

    ValueTask<Quest?> GetQuestAsync(EntityId questId, CancellationToken cancellationToken = default);
    ValueTask InsertQuestAsync(Quest quest, CancellationToken cancellationToken = default);
    ValueTask<bool> UpdateQuestAsync(
        Quest quest,
        EntityVersion expectedVersion,
        CancellationToken cancellationToken = default);

    ValueTask<Campaign?> GetCampaignAsync(EntityId campaignId, CancellationToken cancellationToken = default);
    ValueTask InsertCampaignAsync(Campaign campaign, CancellationToken cancellationToken = default);
    ValueTask<bool> UpdateCampaignAsync(
        Campaign campaign,
        EntityVersion expectedVersion,
        CancellationToken cancellationToken = default);

    ValueTask<CampaignPhase?> GetCampaignPhaseAsync(
        EntityId campaignPhaseId,
        CancellationToken cancellationToken = default);
    ValueTask InsertCampaignPhaseAsync(
        CampaignPhase campaignPhase,
        CancellationToken cancellationToken = default);
    ValueTask<bool> UpdateCampaignPhaseAsync(
        CampaignPhase campaignPhase,
        EntityVersion expectedVersion,
        CancellationToken cancellationToken = default);

    ValueTask<HabitDefinition?> GetHabitAsync(EntityId habitId, CancellationToken cancellationToken = default);
    ValueTask InsertHabitAsync(HabitDefinition habit, CancellationToken cancellationToken = default);
    ValueTask<bool> UpdateHabitAsync(
        HabitDefinition habit,
        EntityVersion expectedVersion,
        CancellationToken cancellationToken = default);

    ValueTask<HabitOccurrence?> GetHabitOccurrenceAsync(
        EntityId occurrenceId,
        CancellationToken cancellationToken = default);
    ValueTask InsertHabitOccurrenceAsync(
        HabitOccurrence occurrence,
        CancellationToken cancellationToken = default);

    ValueTask<FocusSession?> GetFocusSessionAsync(
        EntityId focusSessionId,
        CancellationToken cancellationToken = default);
    ValueTask InsertFocusSessionAsync(
        FocusSession focusSession,
        CancellationToken cancellationToken = default);

    ValueTask<RestPeriod?> GetRestPeriodAsync(
        EntityId restPeriodId,
        CancellationToken cancellationToken = default);
    ValueTask InsertRestPeriodAsync(
        RestPeriod restPeriod,
        CancellationToken cancellationToken = default);
    ValueTask<bool> UpdateRestPeriodAsync(
        RestPeriod restPeriod,
        EntityVersion expectedVersion,
        CancellationToken cancellationToken = default);

    ValueTask<bool> TryStartMutationAsync(
        ClientMutation mutation,
        CancellationToken cancellationToken = default);
    ValueTask<string?> GetMutationResultJsonAsync(
        MutationId mutationId,
        EntityId accountId,
        CancellationToken cancellationToken = default);
    ValueTask CompleteMutationAsync(
        MutationId mutationId,
        EntityId accountId,
        string resultJson,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken = default);

    ValueTask ApplyProgressionAsync(
        ProgressionEvent progressionEvent,
        RewardGrant rewardGrant,
        CancellationToken cancellationToken = default);

    ValueTask<long> AppendChangeAsync(
        EntityId accountId,
        string entityType,
        EntityId entityId,
        EntityVersion version,
        DateTimeOffset changedAt,
        CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);
}
