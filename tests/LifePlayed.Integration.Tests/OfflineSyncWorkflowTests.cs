using System.Text.Json;
using LifePlayed.Application.Sync;
using LifePlayed.Contracts.LifeOS;
using LifePlayed.Contracts.Sync;
using LifePlayed.Infrastructure.Persistence;
using Npgsql;
using Testcontainers.PostgreSql;

namespace LifePlayed.Integration.Tests;

public sealed class OfflineSyncWorkflowTests
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task OfflineLifeOsSyncIsDurableConflictAwareAndIdempotent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var now = new DateTimeOffset(2026, 9, 27, 15, 0, 0, TimeSpan.Zero);

        await using var postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
        await postgres.StartAsync(cancellationToken);

        var connectionString = postgres.GetConnectionString();
        var migrator = new PostgreSqlMigrator(connectionString);
        await migrator.ApplyAsync(cancellationToken);

        var accountId = Guid.CreateVersion7();
        var deviceA = Guid.CreateVersion7();
        var deviceB = Guid.CreateVersion7();
        await SeedAccountAsync(connectionString, accountId, now, cancellationToken);

        var persistence = new PostgreSqlLifeOsPersistence(connectionString);
        var service = new SyncService(
            persistence,
            persistence,
            new FixedTimeProvider(now));

        long cursor = 0;
        var actionId = Guid.CreateVersion7();

        var createAction = Mutation(
            SyncMutationTypes.ActionCreate,
            actionId,
            0,
            new ActionCreatePayload(
                "Write offline sync tests",
                60,
                now.AddDays(1),
                4));

        var createActionResponse = await service.SyncAsync(
            Batch(accountId, deviceA, cursor, createAction),
            cancellationToken);

        Assert.Equal(SyncMutationStatuses.Applied, createActionResponse.Results[0].Status);
        Assert.Equal(1, createActionResponse.Results[0].CanonicalVersion);
        Assert.Single(createActionResponse.Changes);
        cursor = createActionResponse.NextCursor;

        var editAction = Mutation(
            SyncMutationTypes.ActionEdit,
            actionId,
            1,
            new ActionEditPayload(
                "Write durable offline sync tests",
                60,
                now.AddDays(2),
                4));

        var editActionResponse = await service.SyncAsync(
            Batch(accountId, deviceA, cursor, editAction),
            cancellationToken);

        Assert.Equal(SyncMutationStatuses.Applied, editActionResponse.Results[0].Status);
        Assert.Equal(2, editActionResponse.Results[0].CanonicalVersion);
        cursor = editActionResponse.NextCursor;

        var questId = Guid.CreateVersion7();
        var staleDeviceEdit = Mutation(
            SyncMutationTypes.ActionEdit,
            actionId,
            1,
            new ActionEditPayload(
                "Stale device edit",
                15,
                null,
                null));

        var createQuest = Mutation(
            SyncMutationTypes.QuestCreate,
            questId,
            0,
            new QuestCreatePayload(
                "Ship sync foundation",
                now.AddDays(7),
                null));

        var partialBatch = await service.SyncAsync(
            new SyncBatchRequest(
                accountId,
                deviceB,
                cursor,
                new[] { staleDeviceEdit, createQuest }),
            cancellationToken);

        Assert.Equal(SyncMutationStatuses.Conflict, partialBatch.Results[0].Status);
        Assert.Equal(2, partialBatch.Results[0].CanonicalVersion);
        Assert.Equal(SyncMutationStatuses.Applied, partialBatch.Results[1].Status);
        Assert.Equal(1, partialBatch.Results[1].CanonicalVersion);
        Assert.Single(partialBatch.Changes);
        Assert.Equal(questId, partialBatch.Changes[0].EntityId);
        cursor = partialBatch.NextCursor;

        var completeMutationId = Guid.CreateVersion7();
        var completeAction = Mutation(
            SyncMutationTypes.ActionComplete,
            actionId,
            2,
            new { },
            completeMutationId);

        var completion = await service.SyncAsync(
            Batch(accountId, deviceA, cursor, completeAction),
            cancellationToken);

        var completionResult = completion.Results[0];
        Assert.Equal(SyncMutationStatuses.Applied, completionResult.Status);
        Assert.False(completionResult.Replayed);
        Assert.Equal(3, completionResult.CanonicalVersion);
        Assert.NotNull(completionResult.Reward);
        Assert.Equal(39, completionResult.Reward.AccountXp);
        Assert.Equal(29, completionResult.Reward.SkillXp["Craft"]);
        cursor = completion.NextCursor;

        var immediateReplay = await service.SyncAsync(
            Batch(accountId, deviceA, cursor, completeAction),
            cancellationToken);

        Assert.True(immediateReplay.Results[0].Replayed);
        Assert.NotNull(immediateReplay.Results[0].Reward);
        Assert.Equal(
            completionResult.Reward.AccountXp,
            immediateReplay.Results[0].Reward.AccountXp);
        Assert.Equal(
            completionResult.Reward.SkillXp["Craft"],
            immediateReplay.Results[0].Reward.SkillXp["Craft"]);
        Assert.Empty(immediateReplay.Changes);

        var restartedPersistence = new PostgreSqlLifeOsPersistence(connectionString);
        var restartedService = new SyncService(
            restartedPersistence,
            restartedPersistence,
            new FixedTimeProvider(now.AddMinutes(5)));

        var replayAfterRestart = await restartedService.SyncAsync(
            Batch(accountId, deviceA, cursor, completeAction),
            cancellationToken);

        Assert.True(replayAfterRestart.Results[0].Replayed);
        Assert.Equal(completionResult.CanonicalVersion, replayAfterRestart.Results[0].CanonicalVersion);
        Assert.Empty(replayAfterRestart.Changes);

        var campaignId = Guid.CreateVersion7();
        var phaseId = Guid.CreateVersion7();

        var campaignBatch = await restartedService.SyncAsync(
            new SyncBatchRequest(
                accountId,
                deviceA,
                cursor,
                new[]
                {
                    Mutation(
                        SyncMutationTypes.CampaignCreate,
                        campaignId,
                        0,
                        new CampaignCreatePayload(
                            "Launch Life Played",
                            "Build the first public release",
                            now,
                            now.AddMonths(3))),
                    Mutation(
                        SyncMutationTypes.CampaignPhaseCreate,
                        phaseId,
                        0,
                        new CampaignPhaseCreatePayload(
                            campaignId,
                            "Offline foundation",
                            0)),
                    Mutation(
                        SyncMutationTypes.CampaignEdit,
                        campaignId,
                        1,
                        new CampaignEditPayload(
                            "Launch Life Played",
                            "Build and validate the first public release",
                            now,
                            now.AddMonths(4))),
                    Mutation(
                        SyncMutationTypes.CampaignPhaseEdit,
                        phaseId,
                        1,
                        new CampaignPhaseEditPayload(
                            "Offline Life OS",
                            1)),
                }),
            cancellationToken);

        Assert.All(
            campaignBatch.Results,
            static result => Assert.Equal(SyncMutationStatuses.Applied, result.Status));
        cursor = campaignBatch.NextCursor;

        var habitId = Guid.CreateVersion7();
        var occurrenceId = Guid.CreateVersion7();
        var focusId = Guid.CreateVersion7();
        var restId = Guid.CreateVersion7();

        var lifeOsBatch = await restartedService.SyncAsync(
            new SyncBatchRequest(
                accountId,
                deviceA,
                cursor,
                new[]
                {
                    Mutation(
                        SyncMutationTypes.QuestEdit,
                        questId,
                        1,
                        new QuestEditPayload(
                            "Ship the complete sync contract",
                            now.AddDays(8),
                            campaignId)),
                    Mutation(
                        SyncMutationTypes.HabitCreate,
                        habitId,
                        0,
                        new HabitCreatePayload(
                            "Daily architecture pass",
                            "FREQ=DAILY",
                            20,
                            0)),
                    Mutation(
                        SyncMutationTypes.HabitEdit,
                        habitId,
                        1,
                        new HabitEditPayload(
                            "Daily focused architecture pass",
                            "FREQ=DAILY",
                            25,
                            0)),
                    Mutation(
                        SyncMutationTypes.HabitOccurrenceComplete,
                        occurrenceId,
                        0,
                        new HabitOccurrenceCompletePayload(
                            habitId,
                            now)),
                    Mutation(
                        SyncMutationTypes.FocusSessionRecord,
                        focusId,
                        0,
                        new FocusSessionRecordPayload(
                            actionId,
                            30,
                            25,
                            "completed",
                            now.AddMinutes(-25),
                            now)),
                    Mutation(
                        SyncMutationTypes.RestPeriodCreate,
                        restId,
                        0,
                        new RestPeriodCreatePayload(
                            now.AddDays(1),
                            now.AddDays(2),
                            "Planned recovery")),
                    Mutation(
                        SyncMutationTypes.RestPeriodEdit,
                        restId,
                        1,
                        new RestPeriodEditPayload(
                            now.AddDays(1),
                            now.AddDays(2),
                            "Planned recovery day",
                            "scheduled")),
                }),
            cancellationToken);

        Assert.All(
            lifeOsBatch.Results,
            static result => Assert.Equal(SyncMutationStatuses.Applied, result.Status));
        cursor = lifeOsBatch.NextCursor;

        var caughtUp = await restartedService.SyncAsync(
            new SyncBatchRequest(
                accountId,
                deviceA,
                cursor,
                Array.Empty<SyncMutationRequest>()),
            cancellationToken);

        Assert.Empty(caughtUp.Changes);
        Assert.Equal(cursor, caughtUp.NextCursor);

        await AssertDatabaseStateAsync(
            connectionString,
            accountId,
            cancellationToken);
    }

    private static SyncBatchRequest Batch(
        Guid accountId,
        Guid deviceId,
        long cursor,
        SyncMutationRequest mutation) =>
        new(accountId, deviceId, cursor, new[] { mutation });

    private static SyncMutationRequest Mutation<TPayload>(
        string type,
        Guid entityId,
        long baseVersion,
        TPayload payload,
        Guid? mutationId = null) =>
        new(
            mutationId ?? Guid.CreateVersion7(),
            entityId,
            type,
            baseVersion,
            new DateTimeOffset(2026, 9, 27, 15, 0, 0, TimeSpan.Zero),
            JsonSerializer.SerializeToElement(payload, JsonOptions));

    private static async Task SeedAccountAsync(
        string connectionString,
        Guid accountId,
        DateTimeOffset createdAt,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            INSERT INTO identity.accounts (
                account_id, status, locale, time_zone, created_at, version)
            VALUES (
                @account_id, 0, 'en-US', 'America/New_York', @created_at, 0);
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("account_id", accountId);
        command.Parameters.AddWithValue("created_at", createdAt.UtcDateTime);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task AssertDatabaseStateAsync(
        string connectionString,
        Guid accountId,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        Assert.Equal(1, await ScalarIntAsync(
            connection,
            "SELECT count(*)::integer FROM progression.reward_grants WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(1, await ScalarIntAsync(
            connection,
            "SELECT count(*)::integer FROM progression.progression_events WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(39, await ScalarIntAsync(
            connection,
            "SELECT total_xp::integer FROM progression.account_progression WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(29, await ScalarIntAsync(
            connection,
            "SELECT xp::integer FROM progression.life_skill_progress WHERE account_id = @account_id AND skill = 4",
            accountId,
            cancellationToken));

        Assert.Equal(16, await ScalarIntAsync(
            connection,
            "SELECT count(*)::integer FROM sync.client_mutations WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(15, await ScalarIntAsync(
            connection,
            "SELECT count(*)::integer FROM sync.change_log WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(3, await ScalarIntAsync(
            connection,
            "SELECT version::integer FROM life_os.actions WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(2, await ScalarIntAsync(
            connection,
            "SELECT version::integer FROM life_os.quests WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(2, await ScalarIntAsync(
            connection,
            "SELECT version::integer FROM life_os.campaigns WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(2, await ScalarIntAsync(
            connection,
            "SELECT version::integer FROM life_os.habit_definitions WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(1, await ScalarIntAsync(
            connection,
            "SELECT count(*)::integer FROM life_os.habit_occurrences WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(1, await ScalarIntAsync(
            connection,
            "SELECT count(*)::integer FROM life_os.focus_sessions WHERE account_id = @account_id",
            accountId,
            cancellationToken));

        Assert.Equal(2, await ScalarIntAsync(
            connection,
            "SELECT version::integer FROM life_os.rest_periods WHERE account_id = @account_id",
            accountId,
            cancellationToken));
    }

    private static async Task<int> ScalarIntAsync(
        NpgsqlConnection connection,
        string sql,
        Guid accountId,
        CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("account_id", accountId);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Assert.IsType<int>(result);
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public FixedTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow.ToUniversalTime();
        }

        public override DateTimeOffset GetUtcNow() => _utcNow;
    }
}
