using System.Text.Json;
using LifePlayed.Application.Persistence;
using LifePlayed.Domain.Common;
using LifePlayed.Domain.LifeOS;
using LifePlayed.Domain.Progression;
using LifePlayed.Domain.Sync;
using Npgsql;
using NpgsqlTypes;

namespace LifePlayed.Infrastructure.Persistence;

public sealed class PostgreSqlLifeOsPersistence :
    ILifeOsUnitOfWorkFactory,
    ISyncChangeReader
{
    private readonly string _connectionString;

    public PostgreSqlLifeOsPersistence(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
    }

    public async ValueTask<ILifeOsUnitOfWork> BeginAsync(
        CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        var transaction = await connection.BeginTransactionAsync(cancellationToken);
        return new UnitOfWork(connection, transaction);
    }

    public async ValueTask<IReadOnlyList<SyncChangeRecord>> ReadChangesAsync(
        EntityId accountId,
        long afterCursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(afterCursor);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
            SELECT cursor, account_id, entity_type, entity_id, version, changed_at
            FROM sync.change_log
            WHERE account_id = @account_id
              AND cursor > @after_cursor
            ORDER BY cursor
            LIMIT @limit;
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("account_id", accountId.Value);
        command.Parameters.AddWithValue("after_cursor", afterCursor);
        command.Parameters.AddWithValue("limit", limit);

        var changes = new List<SyncChangeRecord>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            changes.Add(new SyncChangeRecord(
                reader.GetInt64(0),
                new EntityId(reader.GetGuid(1)),
                reader.GetString(2),
                new EntityId(reader.GetGuid(3)),
                new EntityVersion(reader.GetInt64(4)),
                ReadTimestamp(reader, 5)));
        }

        return changes;
    }

    private sealed class UnitOfWork : ILifeOsUnitOfWork
    {
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction _transaction;
        private bool _committed;

        public UnitOfWork(
            NpgsqlConnection connection,
            NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public async ValueTask<LifeAction?> GetActionAsync(
            EntityId actionId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT action_id, account_id, title, status, expected_minutes,
                       due_at, completed_at, version, primary_skill
                FROM life_os.actions
                WHERE action_id = @action_id;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("action_id", actionId.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new LifeAction(
                new EntityId(reader.GetGuid(0)),
                new EntityId(reader.GetGuid(1)),
                reader.GetString(2),
                (ActionStatus)reader.GetInt16(3),
                ReadDuration(reader, 4),
                ReadNullableTimestamp(reader, 5),
                ReadNullableTimestamp(reader, 6),
                new EntityVersion(reader.GetInt64(7)),
                ReadNullableEnum<LifeSkill>(reader, 8));
        }

        public async ValueTask InsertActionAsync(
            LifeAction action,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO life_os.actions (
                    action_id, account_id, title, status, expected_minutes,
                    due_at, completed_at, version, primary_skill)
                VALUES (
                    @action_id, @account_id, @title, @status, @expected_minutes,
                    @due_at, @completed_at, @version, @primary_skill);
                """;

            await using var command = CreateCommand(sql);
            AddActionParameters(command, action);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async ValueTask<bool> UpdateActionAsync(
            LifeAction action,
            EntityVersion expectedVersion,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE life_os.actions
                SET title = @title,
                    status = @status,
                    expected_minutes = @expected_minutes,
                    due_at = @due_at,
                    completed_at = @completed_at,
                    version = @version,
                    primary_skill = @primary_skill
                WHERE action_id = @action_id
                  AND account_id = @account_id
                  AND version = @expected_version;
                """;

            await using var command = CreateCommand(sql);
            AddActionParameters(command, action);
            command.Parameters.AddWithValue("expected_version", expectedVersion.Value);
            return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
        }

        public async ValueTask<Quest?> GetQuestAsync(
            EntityId questId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT quest_id, account_id, title, status, due_at, campaign_id, version
                FROM life_os.quests
                WHERE quest_id = @quest_id;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("quest_id", questId.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new Quest(
                new EntityId(reader.GetGuid(0)),
                new EntityId(reader.GetGuid(1)),
                reader.GetString(2),
                (QuestStatus)reader.GetInt16(3),
                ReadNullableTimestamp(reader, 4),
                reader.IsDBNull(5) ? null : new EntityId(reader.GetGuid(5)),
                new EntityVersion(reader.GetInt64(6)));
        }

        public async ValueTask InsertQuestAsync(
            Quest quest,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO life_os.quests (
                    quest_id, account_id, title, status, due_at, campaign_id, version)
                VALUES (
                    @quest_id, @account_id, @title, @status, @due_at, @campaign_id, @version);
                """;

            await using var command = CreateCommand(sql);
            AddQuestParameters(command, quest);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async ValueTask<bool> UpdateQuestAsync(
            Quest quest,
            EntityVersion expectedVersion,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE life_os.quests
                SET title = @title,
                    status = @status,
                    due_at = @due_at,
                    campaign_id = @campaign_id,
                    version = @version
                WHERE quest_id = @quest_id
                  AND account_id = @account_id
                  AND version = @expected_version;
                """;

            await using var command = CreateCommand(sql);
            AddQuestParameters(command, quest);
            command.Parameters.AddWithValue("expected_version", expectedVersion.Value);
            return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
        }

        public async ValueTask<Campaign?> GetCampaignAsync(
            EntityId campaignId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT campaign_id, account_id, title, description, status,
                       start_at, due_at, completed_at, version
                FROM life_os.campaigns
                WHERE campaign_id = @campaign_id;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("campaign_id", campaignId.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new Campaign(
                new EntityId(reader.GetGuid(0)),
                new EntityId(reader.GetGuid(1)),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                (CampaignStatus)reader.GetInt16(4),
                ReadNullableTimestamp(reader, 5),
                ReadNullableTimestamp(reader, 6),
                ReadNullableTimestamp(reader, 7),
                new EntityVersion(reader.GetInt64(8)));
        }

        public async ValueTask InsertCampaignAsync(
            Campaign campaign,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO life_os.campaigns (
                    campaign_id, account_id, title, description, status,
                    start_at, due_at, completed_at, version)
                VALUES (
                    @campaign_id, @account_id, @title, @description, @status,
                    @start_at, @due_at, @completed_at, @version);
                """;

            await using var command = CreateCommand(sql);
            AddCampaignParameters(command, campaign);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async ValueTask<bool> UpdateCampaignAsync(
            Campaign campaign,
            EntityVersion expectedVersion,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE life_os.campaigns
                SET title = @title,
                    description = @description,
                    status = @status,
                    start_at = @start_at,
                    due_at = @due_at,
                    completed_at = @completed_at,
                    version = @version
                WHERE campaign_id = @campaign_id
                  AND account_id = @account_id
                  AND version = @expected_version;
                """;

            await using var command = CreateCommand(sql);
            AddCampaignParameters(command, campaign);
            command.Parameters.AddWithValue("expected_version", expectedVersion.Value);
            return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
        }

        public async ValueTask<CampaignPhase?> GetCampaignPhaseAsync(
            EntityId campaignPhaseId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT campaign_phase_id, campaign_id, title, position, status, version
                FROM life_os.campaign_phases
                WHERE campaign_phase_id = @campaign_phase_id;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("campaign_phase_id", campaignPhaseId.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new CampaignPhase(
                new EntityId(reader.GetGuid(0)),
                new EntityId(reader.GetGuid(1)),
                reader.GetString(2),
                reader.GetInt32(3),
                (CampaignPhaseStatus)reader.GetInt16(4),
                new EntityVersion(reader.GetInt64(5)));
        }

        public async ValueTask InsertCampaignPhaseAsync(
            CampaignPhase campaignPhase,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO life_os.campaign_phases (
                    campaign_phase_id, campaign_id, title, position, status, version)
                VALUES (
                    @campaign_phase_id, @campaign_id, @title, @position, @status, @version);
                """;

            await using var command = CreateCommand(sql);
            AddCampaignPhaseParameters(command, campaignPhase);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async ValueTask<bool> UpdateCampaignPhaseAsync(
            CampaignPhase campaignPhase,
            EntityVersion expectedVersion,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE life_os.campaign_phases
                SET title = @title,
                    position = @position,
                    status = @status,
                    version = @version
                WHERE campaign_phase_id = @campaign_phase_id
                  AND version = @expected_version;
                """;

            await using var command = CreateCommand(sql);
            AddCampaignPhaseParameters(command, campaignPhase);
            command.Parameters.AddWithValue("expected_version", expectedVersion.Value);
            return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
        }

        public async ValueTask<HabitDefinition?> GetHabitAsync(
            EntityId habitId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT habit_definition_id, account_id, title, recurrence_rule, status,
                       expected_minutes, version, primary_skill
                FROM life_os.habit_definitions
                WHERE habit_definition_id = @habit_definition_id;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("habit_definition_id", habitId.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new HabitDefinition(
                new EntityId(reader.GetGuid(0)),
                new EntityId(reader.GetGuid(1)),
                reader.GetString(2),
                reader.GetString(3),
                (HabitStatus)reader.GetInt16(4),
                ReadDuration(reader, 5),
                new EntityVersion(reader.GetInt64(6)),
                ReadNullableEnum<LifeSkill>(reader, 7));
        }

        public async ValueTask InsertHabitAsync(
            HabitDefinition habit,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO life_os.habit_definitions (
                    habit_definition_id, account_id, title, recurrence_rule, status,
                    expected_minutes, version, primary_skill)
                VALUES (
                    @habit_definition_id, @account_id, @title, @recurrence_rule, @status,
                    @expected_minutes, @version, @primary_skill);
                """;

            await using var command = CreateCommand(sql);
            AddHabitParameters(command, habit);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async ValueTask<bool> UpdateHabitAsync(
            HabitDefinition habit,
            EntityVersion expectedVersion,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE life_os.habit_definitions
                SET title = @title,
                    recurrence_rule = @recurrence_rule,
                    status = @status,
                    expected_minutes = @expected_minutes,
                    version = @version,
                    primary_skill = @primary_skill
                WHERE habit_definition_id = @habit_definition_id
                  AND account_id = @account_id
                  AND version = @expected_version;
                """;

            await using var command = CreateCommand(sql);
            AddHabitParameters(command, habit);
            command.Parameters.AddWithValue("expected_version", expectedVersion.Value);
            return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
        }

        public async ValueTask<HabitOccurrence?> GetHabitOccurrenceAsync(
            EntityId occurrenceId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT habit_occurrence_id, habit_definition_id, account_id,
                       scheduled_at, status, completed_at, version
                FROM life_os.habit_occurrences
                WHERE habit_occurrence_id = @habit_occurrence_id;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("habit_occurrence_id", occurrenceId.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new HabitOccurrence(
                new EntityId(reader.GetGuid(0)),
                new EntityId(reader.GetGuid(1)),
                new EntityId(reader.GetGuid(2)),
                ReadTimestamp(reader, 3),
                (HabitOccurrenceStatus)reader.GetInt16(4),
                ReadNullableTimestamp(reader, 5),
                new EntityVersion(reader.GetInt64(6)));
        }

        public async ValueTask InsertHabitOccurrenceAsync(
            HabitOccurrence occurrence,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO life_os.habit_occurrences (
                    habit_occurrence_id, habit_definition_id, account_id,
                    scheduled_at, status, completed_at, version)
                VALUES (
                    @habit_occurrence_id, @habit_definition_id, @account_id,
                    @scheduled_at, @status, @completed_at, @version);
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("habit_occurrence_id", occurrence.HabitOccurrenceId.Value);
            command.Parameters.AddWithValue("habit_definition_id", occurrence.HabitDefinitionId.Value);
            command.Parameters.AddWithValue("account_id", occurrence.AccountId.Value);
            AddTimestamp(command, "scheduled_at", occurrence.ScheduledAt);
            command.Parameters.AddWithValue("status", (short)occurrence.Status);
            AddNullableTimestamp(command, "completed_at", occurrence.CompletedAt);
            command.Parameters.AddWithValue("version", occurrence.Version.Value);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async ValueTask<FocusSession?> GetFocusSessionAsync(
            EntityId focusSessionId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT focus_session_id, account_id, action_id, planned_minutes,
                       actual_minutes, status, started_at, ended_at, version
                FROM life_os.focus_sessions
                WHERE focus_session_id = @focus_session_id;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("focus_session_id", focusSessionId.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new FocusSession(
                new EntityId(reader.GetGuid(0)),
                new EntityId(reader.GetGuid(1)),
                reader.IsDBNull(2) ? null : new EntityId(reader.GetGuid(2)),
                TimeSpan.FromMinutes(reader.GetInt32(3)),
                ReadDuration(reader, 4),
                (FocusSessionStatus)reader.GetInt16(5),
                ReadNullableTimestamp(reader, 6),
                ReadNullableTimestamp(reader, 7),
                new EntityVersion(reader.GetInt64(8)));
        }

        public async ValueTask InsertFocusSessionAsync(
            FocusSession focusSession,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO life_os.focus_sessions (
                    focus_session_id, account_id, action_id, planned_minutes,
                    actual_minutes, status, started_at, ended_at, version)
                VALUES (
                    @focus_session_id, @account_id, @action_id, @planned_minutes,
                    @actual_minutes, @status, @started_at, @ended_at, @version);
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("focus_session_id", focusSession.FocusSessionId.Value);
            command.Parameters.AddWithValue("account_id", focusSession.AccountId.Value);
            AddNullableGuid(command, "action_id", focusSession.ActionId?.Value);
            command.Parameters.AddWithValue("planned_minutes", DurationMinutes(focusSession.PlannedDuration));
            AddNullableInt(command, "actual_minutes", NullableDurationMinutes(focusSession.ActualDuration));
            command.Parameters.AddWithValue("status", (short)focusSession.Status);
            AddNullableTimestamp(command, "started_at", focusSession.StartedAt);
            AddNullableTimestamp(command, "ended_at", focusSession.EndedAt);
            command.Parameters.AddWithValue("version", focusSession.Version.Value);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async ValueTask<RestPeriod?> GetRestPeriodAsync(
            EntityId restPeriodId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT rest_period_id, account_id, starts_at, ends_at, note, status, version
                FROM life_os.rest_periods
                WHERE rest_period_id = @rest_period_id;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("rest_period_id", restPeriodId.Value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new RestPeriod(
                new EntityId(reader.GetGuid(0)),
                new EntityId(reader.GetGuid(1)),
                ReadTimestamp(reader, 2),
                ReadTimestamp(reader, 3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                (RestPeriodStatus)reader.GetInt16(5),
                new EntityVersion(reader.GetInt64(6)));
        }

        public async ValueTask InsertRestPeriodAsync(
            RestPeriod restPeriod,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO life_os.rest_periods (
                    rest_period_id, account_id, starts_at, ends_at, note, status, version)
                VALUES (
                    @rest_period_id, @account_id, @starts_at, @ends_at, @note, @status, @version);
                """;

            await using var command = CreateCommand(sql);
            AddRestPeriodParameters(command, restPeriod);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async ValueTask<bool> UpdateRestPeriodAsync(
            RestPeriod restPeriod,
            EntityVersion expectedVersion,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE life_os.rest_periods
                SET starts_at = @starts_at,
                    ends_at = @ends_at,
                    note = @note,
                    status = @status,
                    version = @version
                WHERE rest_period_id = @rest_period_id
                  AND account_id = @account_id
                  AND version = @expected_version;
                """;

            await using var command = CreateCommand(sql);
            AddRestPeriodParameters(command, restPeriod);
            command.Parameters.AddWithValue("expected_version", expectedVersion.Value);
            return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
        }

        public async ValueTask<bool> TryStartMutationAsync(
            ClientMutation mutation,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO sync.client_mutations (
                    mutation_id, account_id, device_id, entity_id, mutation_type,
                    base_version, client_timestamp, processed_at, result_json)
                VALUES (
                    @mutation_id, @account_id, @device_id, @entity_id, @mutation_type,
                    @base_version, @client_timestamp, NULL, NULL)
                ON CONFLICT (mutation_id) DO NOTHING;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("mutation_id", mutation.MutationId.Value);
            command.Parameters.AddWithValue("account_id", mutation.AccountId.Value);
            command.Parameters.AddWithValue("device_id", mutation.DeviceId.Value);
            command.Parameters.AddWithValue("entity_id", mutation.EntityId.Value);
            command.Parameters.AddWithValue("mutation_type", mutation.MutationType);
            command.Parameters.AddWithValue("base_version", mutation.BaseVersion.Value);
            AddTimestamp(command, "client_timestamp", mutation.ClientTimestamp);

            return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
        }

        public async ValueTask<string?> GetMutationResultJsonAsync(
            MutationId mutationId,
            EntityId accountId,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT result_json::text
                FROM sync.client_mutations
                WHERE mutation_id = @mutation_id
                  AND account_id = @account_id;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("mutation_id", mutationId.Value);
            command.Parameters.AddWithValue("account_id", accountId.Value);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is null or DBNull ? null : (string)result;
        }

        public async ValueTask CompleteMutationAsync(
            MutationId mutationId,
            EntityId accountId,
            string resultJson,
            DateTimeOffset processedAt,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                UPDATE sync.client_mutations
                SET result_json = @result_json,
                    processed_at = @processed_at
                WHERE mutation_id = @mutation_id
                  AND account_id = @account_id
                  AND result_json IS NULL;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("mutation_id", mutationId.Value);
            command.Parameters.AddWithValue("account_id", accountId.Value);
            command.Parameters.Add("result_json", NpgsqlDbType.Jsonb).Value = resultJson;
            AddTimestamp(command, "processed_at", processedAt);

            if (await command.ExecuteNonQueryAsync(cancellationToken) != 1)
            {
                throw new InvalidOperationException("Mutation result could not be completed.");
            }
        }

        public async ValueTask ApplyProgressionAsync(
            ProgressionEvent progressionEvent,
            RewardGrant rewardGrant,
            CancellationToken cancellationToken = default)
        {
            const string eventSql = """
                INSERT INTO progression.progression_events (
                    event_id, account_id, source_type, source_id, rule_version,
                    account_xp_delta, skill_xp_deltas, occurred_at)
                VALUES (
                    @event_id, @account_id, @source_type, @source_id, @rule_version,
                    @account_xp_delta, @skill_xp_deltas, @occurred_at);
                """;

            await using (var command = CreateCommand(eventSql))
            {
                command.Parameters.AddWithValue("event_id", progressionEvent.EventId.Value);
                command.Parameters.AddWithValue("account_id", progressionEvent.AccountId.Value);
                command.Parameters.AddWithValue("source_type", progressionEvent.SourceType);
                command.Parameters.AddWithValue("source_id", progressionEvent.SourceId.Value);
                command.Parameters.AddWithValue("rule_version", progressionEvent.RuleVersion);
                command.Parameters.AddWithValue("account_xp_delta", progressionEvent.AccountXpDelta);
                command.Parameters.Add("skill_xp_deltas", NpgsqlDbType.Jsonb).Value =
                    JsonSerializer.Serialize(progressionEvent.SkillXpDeltas);
                AddTimestamp(command, "occurred_at", progressionEvent.OccurredAt);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            const string grantSql = """
                INSERT INTO progression.reward_grants (
                    grant_id, account_id, source_type, source_id, rule_version,
                    idempotency_key, state, created_at, reversed_at)
                VALUES (
                    @grant_id, @account_id, @source_type, @source_id, @rule_version,
                    @idempotency_key, @state, @created_at, @reversed_at);
                """;

            await using (var command = CreateCommand(grantSql))
            {
                command.Parameters.AddWithValue("grant_id", rewardGrant.GrantId.Value);
                command.Parameters.AddWithValue("account_id", rewardGrant.AccountId.Value);
                command.Parameters.AddWithValue("source_type", rewardGrant.SourceType);
                command.Parameters.AddWithValue("source_id", rewardGrant.SourceId.Value);
                command.Parameters.AddWithValue("rule_version", rewardGrant.RuleVersion);
                command.Parameters.AddWithValue("idempotency_key", rewardGrant.IdempotencyKey);
                command.Parameters.AddWithValue("state", (short)rewardGrant.State);
                AddTimestamp(command, "created_at", rewardGrant.CreatedAt);
                AddNullableTimestamp(command, "reversed_at", rewardGrant.ReversedAt);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            const string accountProgressSql = """
                INSERT INTO progression.account_progression (account_id, total_xp, version)
                VALUES (@account_id, @xp, 1)
                ON CONFLICT (account_id)
                DO UPDATE SET
                    total_xp = progression.account_progression.total_xp + EXCLUDED.total_xp,
                    version = progression.account_progression.version + 1;
                """;

            await using (var command = CreateCommand(accountProgressSql))
            {
                command.Parameters.AddWithValue("account_id", progressionEvent.AccountId.Value);
                command.Parameters.AddWithValue("xp", progressionEvent.AccountXpDelta);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            const string skillProgressSql = """
                INSERT INTO progression.life_skill_progress (account_id, skill, xp, version)
                VALUES (@account_id, @skill, @xp, 1)
                ON CONFLICT (account_id, skill)
                DO UPDATE SET
                    xp = progression.life_skill_progress.xp + EXCLUDED.xp,
                    version = progression.life_skill_progress.version + 1;
                """;

            foreach (var pair in progressionEvent.SkillXpDeltas)
            {
                await using var command = CreateCommand(skillProgressSql);
                command.Parameters.AddWithValue("account_id", progressionEvent.AccountId.Value);
                command.Parameters.AddWithValue("skill", (short)pair.Key);
                command.Parameters.AddWithValue("xp", pair.Value);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async ValueTask<long> AppendChangeAsync(
            EntityId accountId,
            string entityType,
            EntityId entityId,
            EntityVersion version,
            DateTimeOffset changedAt,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                INSERT INTO sync.change_log (
                    account_id, entity_type, entity_id, version, changed_at)
                VALUES (
                    @account_id, @entity_type, @entity_id, @version, @changed_at)
                RETURNING cursor;
                """;

            await using var command = CreateCommand(sql);
            command.Parameters.AddWithValue("account_id", accountId.Value);
            command.Parameters.AddWithValue("entity_type", entityType);
            command.Parameters.AddWithValue("entity_id", entityId.Value);
            command.Parameters.AddWithValue("version", version.Value);
            AddTimestamp(command, "changed_at", changedAt);

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt64(result, System.Globalization.CultureInfo.InvariantCulture);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_committed)
            {
                throw new InvalidOperationException("The unit of work was already committed.");
            }

            await _transaction.CommitAsync(cancellationToken);
            _committed = true;
        }

        public async ValueTask DisposeAsync()
        {
            await _transaction.DisposeAsync();
            await _connection.DisposeAsync();
        }

        private NpgsqlCommand CreateCommand(string sql) =>
            new(sql, _connection, _transaction);

        private static void AddActionParameters(
            NpgsqlCommand command,
            LifeAction action)
        {
            command.Parameters.AddWithValue("action_id", action.ActionId.Value);
            command.Parameters.AddWithValue("account_id", action.AccountId.Value);
            command.Parameters.AddWithValue("title", action.Title);
            command.Parameters.AddWithValue("status", (short)action.Status);
            AddNullableInt(command, "expected_minutes", NullableDurationMinutes(action.ExpectedDuration));
            AddNullableTimestamp(command, "due_at", action.DueAt);
            AddNullableTimestamp(command, "completed_at", action.CompletedAt);
            command.Parameters.AddWithValue("version", action.Version.Value);
            AddNullableShort(command, "primary_skill", action.PrimarySkill is null ? null : (short)action.PrimarySkill.Value);
        }

        private static void AddQuestParameters(
            NpgsqlCommand command,
            Quest quest)
        {
            command.Parameters.AddWithValue("quest_id", quest.QuestId.Value);
            command.Parameters.AddWithValue("account_id", quest.AccountId.Value);
            command.Parameters.AddWithValue("title", quest.Title);
            command.Parameters.AddWithValue("status", (short)quest.Status);
            AddNullableTimestamp(command, "due_at", quest.DueAt);
            AddNullableGuid(command, "campaign_id", quest.CampaignId?.Value);
            command.Parameters.AddWithValue("version", quest.Version.Value);
        }

        private static void AddCampaignParameters(
            NpgsqlCommand command,
            Campaign campaign)
        {
            command.Parameters.AddWithValue("campaign_id", campaign.CampaignId.Value);
            command.Parameters.AddWithValue("account_id", campaign.AccountId.Value);
            command.Parameters.AddWithValue("title", campaign.Title);
            AddNullableText(command, "description", campaign.Description);
            command.Parameters.AddWithValue("status", (short)campaign.Status);
            AddNullableTimestamp(command, "start_at", campaign.StartAt);
            AddNullableTimestamp(command, "due_at", campaign.DueAt);
            AddNullableTimestamp(command, "completed_at", campaign.CompletedAt);
            command.Parameters.AddWithValue("version", campaign.Version.Value);
        }

        private static void AddCampaignPhaseParameters(
            NpgsqlCommand command,
            CampaignPhase phase)
        {
            command.Parameters.AddWithValue("campaign_phase_id", phase.CampaignPhaseId.Value);
            command.Parameters.AddWithValue("campaign_id", phase.CampaignId.Value);
            command.Parameters.AddWithValue("title", phase.Title);
            command.Parameters.AddWithValue("position", phase.Position);
            command.Parameters.AddWithValue("status", (short)phase.Status);
            command.Parameters.AddWithValue("version", phase.Version.Value);
        }

        private static void AddHabitParameters(
            NpgsqlCommand command,
            HabitDefinition habit)
        {
            command.Parameters.AddWithValue("habit_definition_id", habit.HabitDefinitionId.Value);
            command.Parameters.AddWithValue("account_id", habit.AccountId.Value);
            command.Parameters.AddWithValue("title", habit.Title);
            command.Parameters.AddWithValue("recurrence_rule", habit.RecurrenceRule);
            command.Parameters.AddWithValue("status", (short)habit.Status);
            AddNullableInt(command, "expected_minutes", NullableDurationMinutes(habit.ExpectedDuration));
            command.Parameters.AddWithValue("version", habit.Version.Value);
            AddNullableShort(command, "primary_skill", habit.PrimarySkill is null ? null : (short)habit.PrimarySkill.Value);
        }

        private static void AddRestPeriodParameters(
            NpgsqlCommand command,
            RestPeriod restPeriod)
        {
            command.Parameters.AddWithValue("rest_period_id", restPeriod.RestPeriodId.Value);
            command.Parameters.AddWithValue("account_id", restPeriod.AccountId.Value);
            AddTimestamp(command, "starts_at", restPeriod.StartsAt);
            AddTimestamp(command, "ends_at", restPeriod.EndsAt);
            AddNullableText(command, "note", restPeriod.Note);
            command.Parameters.AddWithValue("status", (short)restPeriod.Status);
            command.Parameters.AddWithValue("version", restPeriod.Version.Value);
        }

        private static int DurationMinutes(TimeSpan duration) =>
            checked((int)Math.Round(duration.TotalMinutes, MidpointRounding.AwayFromZero));

        private static int? NullableDurationMinutes(TimeSpan? duration) =>
            duration is null ? null : DurationMinutes(duration.Value);

        private static TimeSpan? ReadDuration(NpgsqlDataReader reader, int ordinal) =>
            reader.IsDBNull(ordinal)
                ? null
                : TimeSpan.FromMinutes(reader.GetInt32(ordinal));

        private static TEnum? ReadNullableEnum<TEnum>(
            NpgsqlDataReader reader,
            int ordinal)
            where TEnum : struct, Enum =>
            reader.IsDBNull(ordinal)
                ? null
                : (TEnum)Enum.ToObject(typeof(TEnum), reader.GetInt16(ordinal));

        private static DateTimeOffset ReadTimestamp(
            NpgsqlDataReader reader,
            int ordinal) =>
            new(reader.GetDateTime(ordinal));

        private static DateTimeOffset? ReadNullableTimestamp(
            NpgsqlDataReader reader,
            int ordinal) =>
            reader.IsDBNull(ordinal)
                ? null
                : ReadTimestamp(reader, ordinal);

        private static void AddTimestamp(
            NpgsqlCommand command,
            string name,
            DateTimeOffset value)
        {
            var parameter = command.Parameters.Add(name, NpgsqlDbType.TimestampTz);
            parameter.Value = value.UtcDateTime;
        }

        private static void AddNullableTimestamp(
            NpgsqlCommand command,
            string name,
            DateTimeOffset? value)
        {
            var parameter = command.Parameters.Add(name, NpgsqlDbType.TimestampTz);
            parameter.Value = value is null ? DBNull.Value : value.Value.UtcDateTime;
        }

        private static void AddNullableGuid(
            NpgsqlCommand command,
            string name,
            Guid? value)
        {
            var parameter = command.Parameters.Add(name, NpgsqlDbType.Uuid);
            parameter.Value = value is null ? DBNull.Value : value.Value;
        }

        private static void AddNullableInt(
            NpgsqlCommand command,
            string name,
            int? value)
        {
            var parameter = command.Parameters.Add(name, NpgsqlDbType.Integer);
            parameter.Value = value is null ? DBNull.Value : value.Value;
        }

        private static void AddNullableShort(
            NpgsqlCommand command,
            string name,
            short? value)
        {
            var parameter = command.Parameters.Add(name, NpgsqlDbType.Smallint);
            parameter.Value = value is null ? DBNull.Value : value.Value;
        }

        private static void AddNullableText(
            NpgsqlCommand command,
            string name,
            string? value)
        {
            var parameter = command.Parameters.Add(name, NpgsqlDbType.Text);
            parameter.Value = value is null ? DBNull.Value : value;
        }
    }

    private static DateTimeOffset ReadTimestamp(
        NpgsqlDataReader reader,
        int ordinal) =>
        new(reader.GetDateTime(ordinal));
}
