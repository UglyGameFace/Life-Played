ALTER TABLE life_os.actions
    ADD COLUMN IF NOT EXISTS primary_skill smallint NULL;

ALTER TABLE sync.client_mutations
    ADD COLUMN IF NOT EXISTS result_json jsonb NULL;

CREATE TABLE IF NOT EXISTS life_os.habit_definitions (
    habit_definition_id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    title text NOT NULL CHECK (length(title) > 0),
    recurrence_rule text NOT NULL CHECK (length(recurrence_rule) > 0),
    status smallint NOT NULL,
    expected_minutes integer NULL CHECK (expected_minutes IS NULL OR expected_minutes >= 0),
    primary_skill smallint NULL,
    version bigint NOT NULL CHECK (version >= 0)
);

CREATE TABLE IF NOT EXISTS life_os.habit_occurrences (
    habit_occurrence_id uuid PRIMARY KEY,
    habit_definition_id uuid NOT NULL REFERENCES life_os.habit_definitions(habit_definition_id),
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    scheduled_at timestamptz NOT NULL,
    status smallint NOT NULL,
    completed_at timestamptz NULL,
    version bigint NOT NULL CHECK (version >= 0),
    UNIQUE (habit_definition_id, scheduled_at)
);

CREATE TABLE IF NOT EXISTS life_os.focus_sessions (
    focus_session_id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    action_id uuid NULL REFERENCES life_os.actions(action_id),
    planned_minutes integer NOT NULL CHECK (planned_minutes > 0),
    actual_minutes integer NULL CHECK (actual_minutes IS NULL OR actual_minutes >= 0),
    status smallint NOT NULL,
    started_at timestamptz NULL,
    ended_at timestamptz NULL,
    version bigint NOT NULL CHECK (version >= 0)
);

CREATE TABLE IF NOT EXISTS life_os.rest_periods (
    rest_period_id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    starts_at timestamptz NOT NULL,
    ends_at timestamptz NOT NULL,
    note text NULL,
    status smallint NOT NULL,
    version bigint NOT NULL CHECK (version >= 0),
    CHECK (ends_at > starts_at)
);

CREATE TABLE IF NOT EXISTS progression.account_progression (
    account_id uuid PRIMARY KEY REFERENCES identity.accounts(account_id),
    total_xp bigint NOT NULL DEFAULT 0 CHECK (total_xp >= 0),
    version bigint NOT NULL DEFAULT 0 CHECK (version >= 0)
);

CREATE TABLE IF NOT EXISTS progression.life_skill_progress (
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    skill smallint NOT NULL,
    xp bigint NOT NULL DEFAULT 0 CHECK (xp >= 0),
    version bigint NOT NULL DEFAULT 0 CHECK (version >= 0),
    PRIMARY KEY (account_id, skill)
);

CREATE TABLE IF NOT EXISTS sync.change_log (
    cursor bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    entity_type text NOT NULL,
    entity_id uuid NOT NULL,
    version bigint NOT NULL CHECK (version >= 0),
    changed_at timestamptz NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_change_log_account_cursor
    ON sync.change_log (account_id, cursor);

CREATE INDEX IF NOT EXISTS idx_habits_account_status
    ON life_os.habit_definitions (account_id, status);

CREATE INDEX IF NOT EXISTS idx_habit_occurrences_account_scheduled
    ON life_os.habit_occurrences (account_id, scheduled_at);

CREATE INDEX IF NOT EXISTS idx_focus_sessions_account_started
    ON life_os.focus_sessions (account_id, started_at);

CREATE INDEX IF NOT EXISTS idx_rest_periods_account_start
    ON life_os.rest_periods (account_id, starts_at);
