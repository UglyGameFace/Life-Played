CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS life_os;
CREATE SCHEMA IF NOT EXISTS progression;
CREATE SCHEMA IF NOT EXISTS sync;

CREATE TABLE IF NOT EXISTS identity.accounts (
    account_id uuid PRIMARY KEY,
    status smallint NOT NULL,
    locale text NOT NULL,
    time_zone text NOT NULL,
    created_at timestamptz NOT NULL,
    version bigint NOT NULL CHECK (version >= 0)
);

CREATE TABLE IF NOT EXISTS life_os.actions (
    action_id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    title text NOT NULL CHECK (length(title) > 0),
    status smallint NOT NULL,
    expected_minutes integer NULL CHECK (expected_minutes IS NULL OR expected_minutes >= 0),
    due_at timestamptz NULL,
    completed_at timestamptz NULL,
    version bigint NOT NULL CHECK (version >= 0)
);

CREATE TABLE IF NOT EXISTS life_os.quests (
    quest_id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    title text NOT NULL CHECK (length(title) > 0),
    status smallint NOT NULL,
    due_at timestamptz NULL,
    campaign_id uuid NULL,
    version bigint NOT NULL CHECK (version >= 0)
);

CREATE TABLE IF NOT EXISTS life_os.campaigns (
    campaign_id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    title text NOT NULL CHECK (length(title) > 0),
    description text NULL,
    status smallint NOT NULL,
    start_at timestamptz NULL,
    due_at timestamptz NULL,
    completed_at timestamptz NULL,
    version bigint NOT NULL CHECK (version >= 0)
);

ALTER TABLE life_os.quests
    DROP CONSTRAINT IF EXISTS quests_campaign_id_fkey;

ALTER TABLE life_os.quests
    ADD CONSTRAINT quests_campaign_id_fkey
    FOREIGN KEY (campaign_id) REFERENCES life_os.campaigns(campaign_id);

CREATE TABLE IF NOT EXISTS life_os.campaign_phases (
    campaign_phase_id uuid PRIMARY KEY,
    campaign_id uuid NOT NULL REFERENCES life_os.campaigns(campaign_id),
    title text NOT NULL CHECK (length(title) > 0),
    position integer NOT NULL CHECK (position >= 0),
    status smallint NOT NULL,
    version bigint NOT NULL CHECK (version >= 0),
    UNIQUE (campaign_id, position)
);

CREATE TABLE IF NOT EXISTS progression.progression_events (
    event_id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    source_type text NOT NULL,
    source_id uuid NOT NULL,
    rule_version text NOT NULL,
    account_xp_delta integer NOT NULL,
    skill_xp_deltas jsonb NOT NULL DEFAULT '{}'::jsonb,
    occurred_at timestamptz NOT NULL
);

CREATE TABLE IF NOT EXISTS progression.reward_grants (
    grant_id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    source_type text NOT NULL,
    source_id uuid NOT NULL,
    rule_version text NOT NULL,
    idempotency_key text NOT NULL,
    state smallint NOT NULL,
    created_at timestamptz NOT NULL,
    reversed_at timestamptz NULL,
    UNIQUE (account_id, idempotency_key)
);

CREATE TABLE IF NOT EXISTS sync.client_mutations (
    mutation_id uuid PRIMARY KEY,
    account_id uuid NOT NULL REFERENCES identity.accounts(account_id),
    device_id uuid NOT NULL,
    entity_id uuid NOT NULL,
    mutation_type text NOT NULL,
    base_version bigint NOT NULL CHECK (base_version >= 0),
    client_timestamp timestamptz NOT NULL,
    processed_at timestamptz NULL
);

CREATE INDEX IF NOT EXISTS idx_actions_account_status
    ON life_os.actions (account_id, status);

CREATE INDEX IF NOT EXISTS idx_quests_account_status
    ON life_os.quests (account_id, status);

CREATE INDEX IF NOT EXISTS idx_campaigns_account_status
    ON life_os.campaigns (account_id, status);

CREATE INDEX IF NOT EXISTS idx_progression_events_account_time
    ON progression.progression_events (account_id, occurred_at);
