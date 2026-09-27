using System.Text.Json;
using System.Text.Json.Serialization;
using LifePlayed.Application.Persistence;
using LifePlayed.Contracts.LifeOS;
using LifePlayed.Contracts.Sync;
using LifePlayed.Domain.Common;
using LifePlayed.Domain.LifeOS;
using LifePlayed.Domain.Progression;
using LifePlayed.Domain.Sync;

namespace LifePlayed.Application.Sync;

public sealed class SyncService
{
    private const int MaxBatchSize = 100;
    private const int MaxChangePageSize = 500;

    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    private readonly ILifeOsUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ISyncChangeReader _changeReader;
    private readonly TimeProvider _timeProvider;

    public SyncService(
        ILifeOsUnitOfWorkFactory unitOfWorkFactory,
        ISyncChangeReader changeReader,
        TimeProvider? timeProvider = null)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _changeReader = changeReader;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<SyncBatchResponse> SyncAsync(
        SyncBatchRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateBatch(request);

        var accountId = new EntityId(request.AccountId);
        var deviceId = new EntityId(request.DeviceId);
        var results = new List<SyncMutationResult>(request.Mutations.Count);

        foreach (var mutation in request.Mutations)
        {
            results.Add(await ProcessMutationAsync(accountId, deviceId, mutation, cancellationToken));
        }

        var afterCursor = request.SinceCursor.GetValueOrDefault();
        ArgumentOutOfRangeException.ThrowIfNegative(afterCursor);

        var changes = await _changeReader.ReadChangesAsync(
            accountId,
            afterCursor,
            MaxChangePageSize,
            cancellationToken);

        var responseChanges = changes
            .Select(static change => new SyncChange(
                change.Cursor,
                change.EntityType,
                change.EntityId.Value,
                change.Version.Value,
                change.ChangedAt))
            .ToArray();

        var nextCursor = responseChanges.Length == 0
            ? afterCursor
            : responseChanges[^1].Cursor;

        return new SyncBatchResponse(results, responseChanges, nextCursor);
    }

    private async Task<SyncMutationResult> ProcessMutationAsync(
        EntityId accountId,
        EntityId deviceId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        ValidateMutationEnvelope(request);

        var mutation = new ClientMutation(
            new MutationId(request.MutationId),
            accountId,
            deviceId,
            new EntityId(request.EntityId),
            request.Type,
            new EntityVersion(request.BaseVersion),
            request.ClientTimestamp);

        await using var unitOfWork = await _unitOfWorkFactory.BeginAsync(cancellationToken);

        if (!await unitOfWork.TryStartMutationAsync(mutation, cancellationToken))
        {
            var existingJson = await unitOfWork.GetMutationResultJsonAsync(
                mutation.MutationId,
                accountId,
                cancellationToken);

            if (existingJson is null)
            {
                return Rejected(
                    request,
                    "mutation_id_collision",
                    "The mutation ID already belongs to another request.");
            }

            var existing = JsonSerializer.Deserialize<SyncMutationResult>(existingJson, JsonOptions)
                ?? throw new InvalidOperationException("Stored mutation result could not be deserialized.");

            return existing with { Replayed = true };
        }

        SyncMutationResult result;
        try
        {
            result = await ApplyMutationAsync(unitOfWork, accountId, request, cancellationToken);
        }
        catch (JsonException)
        {
            result = Rejected(request, "invalid_payload", "The mutation payload is invalid.");
        }
        catch (ArgumentException exception)
        {
            result = Rejected(request, "invalid_payload", exception.Message);
        }

        var resultJson = JsonSerializer.Serialize(result, JsonOptions);
        await unitOfWork.CompleteMutationAsync(
            mutation.MutationId,
            accountId,
            resultJson,
            _timeProvider.GetUtcNow(),
            cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);
        return result;
    }

    private Task<SyncMutationResult> ApplyMutationAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken) =>
        request.Type switch
        {
            SyncMutationTypes.ActionCreate => CreateActionAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.ActionEdit => EditActionAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.ActionComplete => CompleteActionAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.QuestCreate => CreateQuestAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.QuestEdit => EditQuestAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.CampaignCreate => CreateCampaignAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.CampaignEdit => EditCampaignAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.CampaignPhaseCreate => CreateCampaignPhaseAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.CampaignPhaseEdit => EditCampaignPhaseAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.HabitCreate => CreateHabitAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.HabitEdit => EditHabitAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.HabitOccurrenceComplete => CompleteHabitOccurrenceAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.FocusSessionRecord => RecordFocusSessionAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.RestPeriodCreate => CreateRestPeriodAsync(unitOfWork, accountId, request, cancellationToken),
            SyncMutationTypes.RestPeriodEdit => EditRestPeriodAsync(unitOfWork, accountId, request, cancellationToken),
            _ => Task.FromResult(Rejected(request, "unsupported_mutation", "The mutation type is not supported.")),
        };

    private async Task<SyncMutationResult> CreateActionAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.BaseVersion != 0)
        {
            return Conflict(request, null, "New actions must start from version 0.");
        }

        var payload = Deserialize<ActionCreatePayload>(request.Payload);
        ValidateTitle(payload.Title);
        ValidateMinutes(payload.ExpectedMinutes);
        var primarySkill = ParseSkill(payload.PrimarySkill);

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetActionAsync(entityId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(request, existing.Version, "The action already exists.", ActionData(existing));
        }

        var action = new LifeAction(
            entityId,
            accountId,
            payload.Title.Trim(),
            ActionStatus.Active,
            ToDuration(payload.ExpectedMinutes),
            payload.DueAt,
            null,
            new EntityVersion(1),
            primarySkill);

        await unitOfWork.InsertActionAsync(action, cancellationToken);
        await unitOfWork.AppendChangeAsync(
            accountId,
            "action",
            action.ActionId,
            action.Version,
            _timeProvider.GetUtcNow(),
            cancellationToken);

        return Applied(request, action.Version, ActionData(action));
    }

    private async Task<SyncMutationResult> EditActionAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        var payload = Deserialize<ActionEditPayload>(request.Payload);
        ValidateTitle(payload.Title);
        ValidateMinutes(payload.ExpectedMinutes);
        var primarySkill = ParseSkill(payload.PrimarySkill);
        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetActionAsync(entityId, cancellationToken);

        var guard = GuardOwnedVersion(request, accountId, existing?.AccountId, existing?.Version, existing is null ? null : ActionData(existing));
        if (guard is not null)
        {
            return guard;
        }

        var updated = existing! with
        {
            Title = payload.Title.Trim(),
            ExpectedDuration = ToDuration(payload.ExpectedMinutes),
            DueAt = payload.DueAt,
            Version = existing!.Version.Next(),
            PrimarySkill = primarySkill,
        };

        if (!await unitOfWork.UpdateActionAsync(updated, existing.Version, cancellationToken))
        {
            var current = await unitOfWork.GetActionAsync(entityId, cancellationToken);
            return Conflict(request, current?.Version, "The action changed before this edit was applied.", current is null ? null : ActionData(current));
        }

        await AppendChangeAsync(unitOfWork, accountId, "action", entityId, updated.Version, cancellationToken);
        return Applied(request, updated.Version, ActionData(updated));
    }

    private async Task<SyncMutationResult> CompleteActionAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetActionAsync(entityId, cancellationToken);

        var guard = GuardOwnedVersion(request, accountId, existing?.AccountId, existing?.Version, existing is null ? null : ActionData(existing));
        if (guard is not null)
        {
            return guard;
        }

        if (!WorkStateTransitions.CanTransition(existing!.Status, ActionStatus.Completed))
        {
            return Rejected(request, "invalid_transition", "The action cannot be completed from its current state.");
        }

        var now = _timeProvider.GetUtcNow();
        var reward = EconomyRulesV1.EvaluateOrdinaryAction(
            existing.ExpectedDuration?.TotalMinutes ?? 0d);

        var skillDeltas = existing.PrimarySkill is { } skill
            ? new Dictionary<LifeSkill, int> { [skill] = reward.TotalSkillXp }
            : new Dictionary<LifeSkill, int>();

        var updated = existing with
        {
            Status = ActionStatus.Completed,
            CompletedAt = now,
            Version = existing.Version.Next(),
        };

        if (!await unitOfWork.UpdateActionAsync(updated, existing.Version, cancellationToken))
        {
            var current = await unitOfWork.GetActionAsync(entityId, cancellationToken);
            return Conflict(request, current?.Version, "The action changed before completion was applied.", current is null ? null : ActionData(current));
        }

        var progressionEvent = new ProgressionEvent(
            EntityId.New(),
            accountId,
            "action",
            entityId,
            reward.RuleVersion,
            reward.AccountXp,
            skillDeltas,
            now);

        var rewardGrant = new RewardGrant(
            EntityId.New(),
            accountId,
            "action",
            entityId,
            reward.RuleVersion,
            request.MutationId.ToString("D"),
            RewardGrantState.Granted,
            now);

        await unitOfWork.ApplyProgressionAsync(progressionEvent, rewardGrant, cancellationToken);
        await AppendChangeAsync(unitOfWork, accountId, "action", entityId, updated.Version, cancellationToken);

        var receipt = new SyncRewardReceipt(
            reward.AccountXp,
            skillDeltas.ToDictionary(
                static pair => pair.Key.ToString(),
                static pair => pair.Value,
                StringComparer.Ordinal));

        return Applied(request, updated.Version, ActionData(updated), receipt);
    }

    private async Task<SyncMutationResult> CreateQuestAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.BaseVersion != 0)
        {
            return Conflict(request, null, "New quests must start from version 0.");
        }

        var payload = Deserialize<QuestCreatePayload>(request.Payload);
        ValidateTitle(payload.Title);
        await ValidateCampaignReferenceAsync(unitOfWork, accountId, payload.CampaignId, cancellationToken);

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetQuestAsync(entityId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(request, existing.Version, "The quest already exists.", QuestData(existing));
        }

        var quest = new Quest(
            entityId,
            accountId,
            payload.Title.Trim(),
            QuestStatus.Active,
            payload.DueAt,
            ToEntityId(payload.CampaignId),
            new EntityVersion(1));

        await unitOfWork.InsertQuestAsync(quest, cancellationToken);
        await AppendChangeAsync(unitOfWork, accountId, "quest", entityId, quest.Version, cancellationToken);
        return Applied(request, quest.Version, QuestData(quest));
    }

    private async Task<SyncMutationResult> EditQuestAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        var payload = Deserialize<QuestEditPayload>(request.Payload);
        ValidateTitle(payload.Title);
        await ValidateCampaignReferenceAsync(unitOfWork, accountId, payload.CampaignId, cancellationToken);

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetQuestAsync(entityId, cancellationToken);
        var guard = GuardOwnedVersion(request, accountId, existing?.AccountId, existing?.Version, existing is null ? null : QuestData(existing));
        if (guard is not null)
        {
            return guard;
        }

        var updated = existing! with
        {
            Title = payload.Title.Trim(),
            DueAt = payload.DueAt,
            CampaignId = ToEntityId(payload.CampaignId),
            Version = existing!.Version.Next(),
        };

        if (!await unitOfWork.UpdateQuestAsync(updated, existing.Version, cancellationToken))
        {
            var current = await unitOfWork.GetQuestAsync(entityId, cancellationToken);
            return Conflict(request, current?.Version, "The quest changed before this edit was applied.", current is null ? null : QuestData(current));
        }

        await AppendChangeAsync(unitOfWork, accountId, "quest", entityId, updated.Version, cancellationToken);
        return Applied(request, updated.Version, QuestData(updated));
    }

    private async Task<SyncMutationResult> CreateCampaignAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.BaseVersion != 0)
        {
            return Conflict(request, null, "New campaigns must start from version 0.");
        }

        var payload = Deserialize<CampaignCreatePayload>(request.Payload);
        ValidateTitle(payload.Title);
        ValidateRange(payload.StartAt, payload.DueAt, "Campaign due time cannot be before its start.");

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetCampaignAsync(entityId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(request, existing.Version, "The campaign already exists.", CampaignData(existing));
        }

        var campaign = new Campaign(
            entityId,
            accountId,
            payload.Title.Trim(),
            payload.Description?.Trim(),
            CampaignStatus.Active,
            payload.StartAt,
            payload.DueAt,
            null,
            new EntityVersion(1));

        await unitOfWork.InsertCampaignAsync(campaign, cancellationToken);
        await AppendChangeAsync(unitOfWork, accountId, "campaign", entityId, campaign.Version, cancellationToken);
        return Applied(request, campaign.Version, CampaignData(campaign));
    }

    private async Task<SyncMutationResult> EditCampaignAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        var payload = Deserialize<CampaignEditPayload>(request.Payload);
        ValidateTitle(payload.Title);
        ValidateRange(payload.StartAt, payload.DueAt, "Campaign due time cannot be before its start.");

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetCampaignAsync(entityId, cancellationToken);
        var guard = GuardOwnedVersion(request, accountId, existing?.AccountId, existing?.Version, existing is null ? null : CampaignData(existing));
        if (guard is not null)
        {
            return guard;
        }

        var updated = existing! with
        {
            Title = payload.Title.Trim(),
            Description = payload.Description?.Trim(),
            StartAt = payload.StartAt,
            DueAt = payload.DueAt,
            Version = existing!.Version.Next(),
        };

        if (!await unitOfWork.UpdateCampaignAsync(updated, existing.Version, cancellationToken))
        {
            var current = await unitOfWork.GetCampaignAsync(entityId, cancellationToken);
            return Conflict(request, current?.Version, "The campaign changed before this edit was applied.", current is null ? null : CampaignData(current));
        }

        await AppendChangeAsync(unitOfWork, accountId, "campaign", entityId, updated.Version, cancellationToken);
        return Applied(request, updated.Version, CampaignData(updated));
    }

    private async Task<SyncMutationResult> CreateCampaignPhaseAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.BaseVersion != 0)
        {
            return Conflict(request, null, "New campaign phases must start from version 0.");
        }

        var payload = Deserialize<CampaignPhaseCreatePayload>(request.Payload);
        ValidateTitle(payload.Title);
        ArgumentOutOfRangeException.ThrowIfNegative(payload.Position);

        var campaign = await unitOfWork.GetCampaignAsync(new EntityId(payload.CampaignId), cancellationToken);
        if (campaign is null || campaign.AccountId != accountId)
        {
            return Rejected(request, "campaign_not_found", "The campaign does not exist for this account.");
        }

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetCampaignPhaseAsync(entityId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(request, existing.Version, "The campaign phase already exists.", CampaignPhaseData(existing));
        }

        var phase = new CampaignPhase(
            entityId,
            campaign.CampaignId,
            payload.Title.Trim(),
            payload.Position,
            CampaignPhaseStatus.Active,
            new EntityVersion(1));

        await unitOfWork.InsertCampaignPhaseAsync(phase, cancellationToken);
        await AppendChangeAsync(unitOfWork, accountId, "campaign_phase", entityId, phase.Version, cancellationToken);
        return Applied(request, phase.Version, CampaignPhaseData(phase));
    }

    private async Task<SyncMutationResult> EditCampaignPhaseAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        var payload = Deserialize<CampaignPhaseEditPayload>(request.Payload);
        ValidateTitle(payload.Title);
        ArgumentOutOfRangeException.ThrowIfNegative(payload.Position);

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetCampaignPhaseAsync(entityId, cancellationToken);
        if (existing is null)
        {
            return Rejected(request, "not_found", "The campaign phase does not exist.");
        }

        var campaign = await unitOfWork.GetCampaignAsync(existing.CampaignId, cancellationToken);
        if (campaign is null || campaign.AccountId != accountId)
        {
            return Rejected(request, "not_found", "The campaign phase does not exist for this account.");
        }

        if (existing.Version.Value != request.BaseVersion)
        {
            return Conflict(request, existing.Version, "The campaign phase has changed.", CampaignPhaseData(existing));
        }

        var updated = existing with
        {
            Title = payload.Title.Trim(),
            Position = payload.Position,
            Version = existing.Version.Next(),
        };

        if (!await unitOfWork.UpdateCampaignPhaseAsync(updated, existing.Version, cancellationToken))
        {
            var current = await unitOfWork.GetCampaignPhaseAsync(entityId, cancellationToken);
            return Conflict(request, current?.Version, "The campaign phase changed before this edit was applied.", current is null ? null : CampaignPhaseData(current));
        }

        await AppendChangeAsync(unitOfWork, accountId, "campaign_phase", entityId, updated.Version, cancellationToken);
        return Applied(request, updated.Version, CampaignPhaseData(updated));
    }

    private async Task<SyncMutationResult> CreateHabitAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.BaseVersion != 0)
        {
            return Conflict(request, null, "New habits must start from version 0.");
        }

        var payload = Deserialize<HabitCreatePayload>(request.Payload);
        ValidateTitle(payload.Title);
        ValidateRequiredText(payload.RecurrenceRule, nameof(payload.RecurrenceRule));
        ValidateMinutes(payload.ExpectedMinutes);
        var primarySkill = ParseSkill(payload.PrimarySkill);

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetHabitAsync(entityId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(request, existing.Version, "The habit already exists.", HabitData(existing));
        }

        var habit = new HabitDefinition(
            entityId,
            accountId,
            payload.Title.Trim(),
            payload.RecurrenceRule.Trim(),
            HabitStatus.Active,
            ToDuration(payload.ExpectedMinutes),
            new EntityVersion(1),
            primarySkill);

        await unitOfWork.InsertHabitAsync(habit, cancellationToken);
        await AppendChangeAsync(unitOfWork, accountId, "habit", entityId, habit.Version, cancellationToken);
        return Applied(request, habit.Version, HabitData(habit));
    }

    private async Task<SyncMutationResult> EditHabitAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        var payload = Deserialize<HabitEditPayload>(request.Payload);
        ValidateTitle(payload.Title);
        ValidateRequiredText(payload.RecurrenceRule, nameof(payload.RecurrenceRule));
        ValidateMinutes(payload.ExpectedMinutes);
        var primarySkill = ParseSkill(payload.PrimarySkill);

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetHabitAsync(entityId, cancellationToken);
        var guard = GuardOwnedVersion(request, accountId, existing?.AccountId, existing?.Version, existing is null ? null : HabitData(existing));
        if (guard is not null)
        {
            return guard;
        }

        var updated = existing! with
        {
            Title = payload.Title.Trim(),
            RecurrenceRule = payload.RecurrenceRule.Trim(),
            ExpectedDuration = ToDuration(payload.ExpectedMinutes),
            PrimarySkill = primarySkill,
            Version = existing!.Version.Next(),
        };

        if (!await unitOfWork.UpdateHabitAsync(updated, existing.Version, cancellationToken))
        {
            var current = await unitOfWork.GetHabitAsync(entityId, cancellationToken);
            return Conflict(request, current?.Version, "The habit changed before this edit was applied.", current is null ? null : HabitData(current));
        }

        await AppendChangeAsync(unitOfWork, accountId, "habit", entityId, updated.Version, cancellationToken);
        return Applied(request, updated.Version, HabitData(updated));
    }

    private async Task<SyncMutationResult> CompleteHabitOccurrenceAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.BaseVersion != 0)
        {
            return Conflict(request, null, "New habit occurrences must start from version 0.");
        }

        var payload = Deserialize<HabitOccurrenceCompletePayload>(request.Payload);
        var habit = await unitOfWork.GetHabitAsync(new EntityId(payload.HabitDefinitionId), cancellationToken);
        if (habit is null || habit.AccountId != accountId)
        {
            return Rejected(request, "habit_not_found", "The habit does not exist for this account.");
        }

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetHabitOccurrenceAsync(entityId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(request, existing.Version, "The habit occurrence already exists.", HabitOccurrenceData(existing));
        }

        var occurrence = new HabitOccurrence(
            entityId,
            habit.HabitDefinitionId,
            accountId,
            payload.ScheduledAt,
            HabitOccurrenceStatus.Completed,
            _timeProvider.GetUtcNow(),
            new EntityVersion(1));

        await unitOfWork.InsertHabitOccurrenceAsync(occurrence, cancellationToken);
        await AppendChangeAsync(unitOfWork, accountId, "habit_occurrence", entityId, occurrence.Version, cancellationToken);
        return Applied(request, occurrence.Version, HabitOccurrenceData(occurrence));
    }

    private async Task<SyncMutationResult> RecordFocusSessionAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.BaseVersion != 0)
        {
            return Conflict(request, null, "New focus sessions must start from version 0.");
        }

        var payload = Deserialize<FocusSessionRecordPayload>(request.Payload);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(payload.PlannedMinutes);
        if (payload.ActualMinutes is { } actualMinutes)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(actualMinutes);
        }

        var status = payload.Status.Trim().ToLowerInvariant() switch
        {
            "completed" => FocusSessionStatus.Completed,
            "interrupted" => FocusSessionStatus.Interrupted,
            _ => throw new ArgumentException("Focus session status must be 'completed' or 'interrupted'."),
        };

        EntityId? actionId = null;
        if (payload.ActionId is { } actionGuid)
        {
            var action = await unitOfWork.GetActionAsync(new EntityId(actionGuid), cancellationToken);
            if (action is null || action.AccountId != accountId)
            {
                return Rejected(request, "action_not_found", "The linked action does not exist for this account.");
            }

            actionId = action.ActionId;
        }

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetFocusSessionAsync(entityId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(request, existing.Version, "The focus session already exists.", FocusSessionData(existing));
        }

        var focus = new FocusSession(
            entityId,
            accountId,
            actionId,
            TimeSpan.FromMinutes(payload.PlannedMinutes),
            ToDuration(payload.ActualMinutes),
            status,
            payload.StartedAt,
            payload.EndedAt,
            new EntityVersion(1));

        await unitOfWork.InsertFocusSessionAsync(focus, cancellationToken);
        await AppendChangeAsync(unitOfWork, accountId, "focus_session", entityId, focus.Version, cancellationToken);
        return Applied(request, focus.Version, FocusSessionData(focus));
    }

    private async Task<SyncMutationResult> CreateRestPeriodAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.BaseVersion != 0)
        {
            return Conflict(request, null, "New rest periods must start from version 0.");
        }

        var payload = Deserialize<RestPeriodCreatePayload>(request.Payload);
        ValidateRange(payload.StartsAt, payload.EndsAt, "Rest period end must be after its start.");

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetRestPeriodAsync(entityId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(request, existing.Version, "The rest period already exists.", RestPeriodData(existing));
        }

        var status = payload.StartsAt > _timeProvider.GetUtcNow()
            ? RestPeriodStatus.Scheduled
            : RestPeriodStatus.Active;

        var rest = new RestPeriod(
            entityId,
            accountId,
            payload.StartsAt,
            payload.EndsAt,
            payload.Note?.Trim(),
            status,
            new EntityVersion(1));

        await unitOfWork.InsertRestPeriodAsync(rest, cancellationToken);
        await AppendChangeAsync(unitOfWork, accountId, "rest_period", entityId, rest.Version, cancellationToken);
        return Applied(request, rest.Version, RestPeriodData(rest));
    }

    private async Task<SyncMutationResult> EditRestPeriodAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        SyncMutationRequest request,
        CancellationToken cancellationToken)
    {
        var payload = Deserialize<RestPeriodEditPayload>(request.Payload);
        ValidateRange(payload.StartsAt, payload.EndsAt, "Rest period end must be after its start.");

        var status = payload.Status.Trim().ToLowerInvariant() switch
        {
            "scheduled" => RestPeriodStatus.Scheduled,
            "active" => RestPeriodStatus.Active,
            "completed" => RestPeriodStatus.Completed,
            "cancelled" => RestPeriodStatus.Cancelled,
            _ => throw new ArgumentException("Rest period status is invalid."),
        };

        var entityId = new EntityId(request.EntityId);
        var existing = await unitOfWork.GetRestPeriodAsync(entityId, cancellationToken);
        var guard = GuardOwnedVersion(request, accountId, existing?.AccountId, existing?.Version, existing is null ? null : RestPeriodData(existing));
        if (guard is not null)
        {
            return guard;
        }

        var updated = existing! with
        {
            StartsAt = payload.StartsAt,
            EndsAt = payload.EndsAt,
            Note = payload.Note?.Trim(),
            Status = status,
            Version = existing!.Version.Next(),
        };

        if (!await unitOfWork.UpdateRestPeriodAsync(updated, existing.Version, cancellationToken))
        {
            var current = await unitOfWork.GetRestPeriodAsync(entityId, cancellationToken);
            return Conflict(request, current?.Version, "The rest period changed before this edit was applied.", current is null ? null : RestPeriodData(current));
        }

        await AppendChangeAsync(unitOfWork, accountId, "rest_period", entityId, updated.Version, cancellationToken);
        return Applied(request, updated.Version, RestPeriodData(updated));
    }

    private async Task ValidateCampaignReferenceAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        Guid? campaignId,
        CancellationToken cancellationToken)
    {
        if (campaignId is null)
        {
            return;
        }

        var campaign = await unitOfWork.GetCampaignAsync(new EntityId(campaignId.Value), cancellationToken);
        if (campaign is null || campaign.AccountId != accountId)
        {
            throw new ArgumentException("The referenced campaign does not exist for this account.");
        }
    }

    private Task<long> AppendChangeAsync(
        ILifeOsUnitOfWork unitOfWork,
        EntityId accountId,
        string entityType,
        EntityId entityId,
        EntityVersion version,
        CancellationToken cancellationToken) =>
        unitOfWork.AppendChangeAsync(
            accountId,
            entityType,
            entityId,
            version,
            _timeProvider.GetUtcNow(),
            cancellationToken).AsTask();

    private static SyncMutationResult? GuardOwnedVersion(
        SyncMutationRequest request,
        EntityId accountId,
        EntityId? ownerAccountId,
        EntityVersion? currentVersion,
        JsonElement? canonicalData)
    {
        if (ownerAccountId is null)
        {
            return Rejected(request, "not_found", "The entity does not exist.");
        }

        if (ownerAccountId.Value != accountId)
        {
            return Rejected(request, "not_found", "The entity does not exist for this account.");
        }

        if (currentVersion!.Value.Value != request.BaseVersion)
        {
            return Conflict(request, currentVersion, "The entity has changed.", canonicalData);
        }

        return null;
    }

    private static SyncMutationResult Applied(
        SyncMutationRequest request,
        EntityVersion version,
        JsonElement canonicalData,
        SyncRewardReceipt? reward = null) =>
        new(
            request.MutationId,
            request.EntityId,
            SyncMutationStatuses.Applied,
            false,
            version.Value,
            null,
            null,
            canonicalData,
            reward);

    private static SyncMutationResult Conflict(
        SyncMutationRequest request,
        EntityVersion? version,
        string message,
        JsonElement? canonicalData = null) =>
        new(
            request.MutationId,
            request.EntityId,
            SyncMutationStatuses.Conflict,
            false,
            version?.Value,
            "version_conflict",
            message,
            canonicalData,
            null);

    private static SyncMutationResult Rejected(
        SyncMutationRequest request,
        string errorCode,
        string message) =>
        new(
            request.MutationId,
            request.EntityId,
            SyncMutationStatuses.Rejected,
            false,
            null,
            errorCode,
            message,
            null,
            null);

    private static T Deserialize<T>(JsonElement payload)
    {
        var value = payload.Deserialize<T>(JsonOptions);
        return value ?? throw new JsonException("Mutation payload is empty.");
    }

    private static void ValidateBatch(SyncBatchRequest request)
    {
        if (request.AccountId == Guid.Empty)
        {
            throw new ArgumentException("AccountId is required.", nameof(request));
        }

        if (request.DeviceId == Guid.Empty)
        {
            throw new ArgumentException("DeviceId is required.", nameof(request));
        }

        if (request.Mutations.Count > MaxBatchSize)
        {
            throw new ArgumentException($"A sync batch may contain at most {MaxBatchSize} mutations.", nameof(request));
        }

        if (request.SinceCursor is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "SinceCursor cannot be negative.");
        }
    }

    private static void ValidateMutationEnvelope(SyncMutationRequest request)
    {
        if (request.MutationId == Guid.Empty)
        {
            throw new ArgumentException("MutationId is required.", nameof(request));
        }

        if (request.EntityId == Guid.Empty)
        {
            throw new ArgumentException("EntityId is required.", nameof(request));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(request.Type);
        ArgumentOutOfRangeException.ThrowIfNegative(request.BaseVersion);
    }

    private static void ValidateTitle(string title) =>
        ValidateRequiredText(title, nameof(title));

    private static void ValidateRequiredText(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{name} is required.", name);
        }
    }

    private static void ValidateMinutes(int? minutes)
    {
        if (minutes is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes cannot be negative.");
        }
    }

    private static void ValidateRange(
        DateTimeOffset? startsAt,
        DateTimeOffset? endsAt,
        string message)
    {
        if (startsAt is { } start && endsAt is { } end && end < start)
        {
            throw new ArgumentException(message);
        }
    }

    private static void ValidateRange(
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        string message)
    {
        if (endsAt <= startsAt)
        {
            throw new ArgumentException(message);
        }
    }

    private static TimeSpan? ToDuration(int? minutes) =>
        minutes is null ? null : TimeSpan.FromMinutes(minutes.Value);

    private static EntityId? ToEntityId(Guid? id) =>
        id is null ? null : new EntityId(id.Value);

    private static LifeSkill? ParseSkill(int? value)
    {
        if (value is null)
        {
            return null;
        }

        if (!Enum.IsDefined(typeof(LifeSkill), value.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "PrimarySkill is invalid.");
        }

        return (LifeSkill)value.Value;
    }

    private static JsonElement ActionData(LifeAction action) =>
        JsonSerializer.SerializeToElement(new
        {
            actionId = action.ActionId.Value,
            title = action.Title,
            status = action.Status.ToString(),
            expectedMinutes = action.ExpectedDuration?.TotalMinutes,
            dueAt = action.DueAt,
            completedAt = action.CompletedAt,
            primarySkill = action.PrimarySkill?.ToString(),
        }, JsonOptions);

    private static JsonElement QuestData(Quest quest) =>
        JsonSerializer.SerializeToElement(new
        {
            questId = quest.QuestId.Value,
            title = quest.Title,
            status = quest.Status.ToString(),
            dueAt = quest.DueAt,
            campaignId = quest.CampaignId?.Value,
        }, JsonOptions);

    private static JsonElement CampaignData(Campaign campaign) =>
        JsonSerializer.SerializeToElement(new
        {
            campaignId = campaign.CampaignId.Value,
            title = campaign.Title,
            description = campaign.Description,
            status = campaign.Status.ToString(),
            startAt = campaign.StartAt,
            dueAt = campaign.DueAt,
            completedAt = campaign.CompletedAt,
        }, JsonOptions);

    private static JsonElement CampaignPhaseData(CampaignPhase phase) =>
        JsonSerializer.SerializeToElement(new
        {
            campaignPhaseId = phase.CampaignPhaseId.Value,
            campaignId = phase.CampaignId.Value,
            title = phase.Title,
            position = phase.Position,
            status = phase.Status.ToString(),
        }, JsonOptions);

    private static JsonElement HabitData(HabitDefinition habit) =>
        JsonSerializer.SerializeToElement(new
        {
            habitDefinitionId = habit.HabitDefinitionId.Value,
            title = habit.Title,
            recurrenceRule = habit.RecurrenceRule,
            status = habit.Status.ToString(),
            expectedMinutes = habit.ExpectedDuration?.TotalMinutes,
            primarySkill = habit.PrimarySkill?.ToString(),
        }, JsonOptions);

    private static JsonElement HabitOccurrenceData(HabitOccurrence occurrence) =>
        JsonSerializer.SerializeToElement(new
        {
            habitOccurrenceId = occurrence.HabitOccurrenceId.Value,
            habitDefinitionId = occurrence.HabitDefinitionId.Value,
            scheduledAt = occurrence.ScheduledAt,
            status = occurrence.Status.ToString(),
            completedAt = occurrence.CompletedAt,
        }, JsonOptions);

    private static JsonElement FocusSessionData(FocusSession session) =>
        JsonSerializer.SerializeToElement(new
        {
            focusSessionId = session.FocusSessionId.Value,
            actionId = session.ActionId?.Value,
            plannedMinutes = session.PlannedDuration.TotalMinutes,
            actualMinutes = session.ActualDuration?.TotalMinutes,
            status = session.Status.ToString(),
            startedAt = session.StartedAt,
            endedAt = session.EndedAt,
        }, JsonOptions);

    private static JsonElement RestPeriodData(RestPeriod restPeriod) =>
        JsonSerializer.SerializeToElement(new
        {
            restPeriodId = restPeriod.RestPeriodId.Value,
            startsAt = restPeriod.StartsAt,
            endsAt = restPeriod.EndsAt,
            note = restPeriod.Note,
            status = restPeriod.Status.ToString(),
        }, JsonOptions);

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }
}
