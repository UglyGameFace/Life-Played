# ACTIVE TASK — Life Played

## Active task / desired outcome

**Task:** Milestone 2 — Offline Life OS + sync contract.

**Outcome:** Implement the first usable headless Life OS workflows and synchronization contract without Unity: offline-capable Action/Quest/Campaign/Habit/Focus/Rest mutations, durable idempotent sync behavior, canonical reward confirmation, concurrency/conflict rules, and real PostgreSQL persistence.

This is the **only active implementation task** until its Definition of Done is satisfied.

---

## Status

**State:** ACTIVE — Milestone 2 / implementation + exact-head validation

**Unity cloud usage:** 0 minutes required by this milestone.

---

## Previous completed task

### Milestone 1 — Headless core skeleton — COMPLETE

Milestone 1 established the production-code foundation without using Unity cloud-build minutes.

Implemented and merged:

- .NET 10 LTS solution using modern `.slnx`.
- Domain, Contracts, Application, Infrastructure, and API projects.
- Strict warnings-as-errors/analyzer configuration.
- UUIDv7-based opaque EntityId and MutationId primitives.
- EntityVersion concurrency primitive.
- Minimal Account, Action, Quest, Campaign, and CampaignPhase domain records.
- Explicit Action/Quest/Campaign/CampaignPhase state transitions.
- Eight V1 Life Skill identifiers.
- Versioned `EconomyRulesV1`.
- Deterministic `RewardEvaluation`.
- ProgressionEvent and RewardGrant primitives.
- ClientMutation offline/idempotency primitive.
- Initial persistence abstraction.
- PostgreSQL migration runner and first core schema migration.
- Versioned content-release contract and JSON schema.
- Domain and integration test projects.
- Real PostgreSQL 17 Testcontainers validation.
- Headless GitHub Actions CI.
- Current Node 24-based GitHub Actions majors.

Milestone 1 merge history:

- PR #1 → `5c09a5cf3ea57f4af6f11ef7024d8bff72214994`
- PR #2 → `19d89af7e011e63e05b102cd0391b4314c403723`
- PR #3 → `a43c6a431bb4560152ef8b2b7ff04dda4d4504c2`

Final validation on merged `main`:

- Headless CI run #13: success
- Release build: success
- Build warnings: 0
- Build errors: 0
- Tests: **46 passed, 0 failed, 0 skipped**
- PostgreSQL migration applied twice successfully without duplicate migration records.
- Domain dependency boundary regression-tested against Unity, ASP.NET, EF Core, and Npgsql.
- Application dependency boundary regression-tested against Unity, ASP.NET, EF Core, and Npgsql.
- Content-release schema validated as version 1.
- No Unity cloud minutes used.

Completion audit also corrected earlier missing scope items rather than hiding them.

---

## Milestone 2 scope

Implement only the Offline Life OS + sync contract described in `docs/IMPLEMENTATION_PLAN.md`.

### Required Life OS behavior

- Action create/edit/complete
- Quest create/edit
- Campaign create/edit
- Campaign phase structure
- HabitDefinition basic model
- HabitOccurrence basic model
- FocusSession
- RestPeriod

### Required persistence behavior

- PostgreSQL repositories for Milestone 2 entities
- optimistic concurrency using EntityVersion
- durable persistence of client mutations
- durable authoritative reward/progression records needed by Action completion
- migrations for new Milestone 2 tables/columns/indexes

### Required sync behavior

- client mutation batch contract
- stable mutation IDs
- idempotent mutation processing
- per-mutation canonical result
- sync cursor/incremental change concept
- safe retry after network failure
- partial batch failure behavior
- stale-base-version conflict behavior
- no duplicate reward on retry/replay

### Required completion flow

For ordinary Action completion:

1. validate account/action ownership and current state
2. check mutation/idempotency
3. validate state transition
4. evaluate versioned reward
5. persist progression/reward state atomically
6. update Action canonical state/version
7. persist processed mutation/result
8. return canonical result
9. repeating the same mutation returns the original outcome without duplicating reward

### Required validation

- offline mutation simulation
- duplicate retry
- mutation replay
- stale version conflict
- partial batch failure
- two-device edit conflict scenario
- app-restart/durable queue simulation
- real PostgreSQL integration tests
- reward/idempotency invariants
- exact-head Release build/tests
- final diff/secret/generated-junk review

---

## Out of scope

Do not begin during Milestone 2:

- Unity project
- Android/iOS build
- character/companion/world rendering
- Chronicle UI
- AI Quest Master
- Deal Scout
- affiliate providers
- location
- cashback
- production auth provider
- production hosting deployment
- Wild Renewal content engine
- Gridfall

These belong to later milestones.

---

## Architecture constraints

- Domain remains provider/framework independent.
- Application remains free of Unity, ASP.NET, EF Core, and Npgsql.
- Infrastructure owns PostgreSQL implementation.
- API owns transport concerns only.
- Reward logic remains deterministic/versioned.
- Valuable state remains server-authoritative.
- Offline client intent uses globally unique mutation IDs.
- Replays must not create duplicate rewards.
- Conflicts must be explicit, not silently overwritten where unsafe.

---

## Investigation / execution path

Execution path traced and implementation underway:

1. Existing domain/state/migration/API paths inspected; no competing sync implementation existed.
2. The superseded generic IEntityRepository abstraction was removed after confirming it had no callers.
3. Sync batch and mutation result contracts were added.
4. Application-owned mutation behavior now covers Action, Quest, Campaign/Phase, Habit, HabitOccurrence, FocusSession, and RestPeriod flows.
5. PostgreSQL Infrastructure now owns transactional persistence, mutation claims/results, optimistic updates, progression ledgers, and change cursors.
6. API transport exposes POST /sync when a database connection is configured and returns 503 otherwise.
7. Real PostgreSQL integration coverage now exercises replay, restart, stale two-device conflict, partial batch success, exactly-once reward, and all Milestone 2 entity persistence.
8. Exact-head CI and cleanup remain pending.

---

## Definition of Done

Milestone 2 is complete only when:

- [ ] Action create/edit/complete works through Application + persistence.
- [ ] Quest create/edit works through Application + persistence.
- [ ] Campaign + CampaignPhase create/edit works.
- [ ] HabitDefinition and HabitOccurrence basic models/persistence exist.
- [ ] FocusSession and RestPeriod basic models/persistence exist.
- [ ] Sync batch contract exists.
- [ ] Client mutations are durably idempotent.
- [ ] Action completion produces exactly one authoritative reward/progression result.
- [ ] Duplicate retry returns prior canonical outcome.
- [ ] Stale version conflicts are explicit.
- [ ] Partial batch failure is tested.
- [ ] Two-device conflict scenario is tested.
- [ ] Restart/durable replay scenario is tested.
- [ ] Relevant PostgreSQL migrations are idempotent and tested.
- [ ] Release build passes with 0 errors.
- [ ] Relevant automated tests pass on exact head.
- [ ] No Unity dependency is introduced.
- [ ] No production secret or generated junk is committed.
- [ ] Final diff is focused on Milestone 2.
- [ ] Remaining limitations/blockers are documented.

---

## Blockers / risks

- Production auth/account-provider selection is intentionally deferred; Milestone 2 tests use internal Account IDs.
- Production hosting/database-provider selection is intentionally deferred.
- Exact Unity LTS version remains deferred until the Unity milestone.
- Life Played remains a working product name pending formal trademark/domain/store clearance.

---

## Backlog

Immediate next milestone after successful completion:

**Milestone 3 — Content engine + Wild Renewal content skeleton.**

---

## Next step

Inspect the merged Milestone 1 execution path and create the dedicated Milestone 2 branch before implementing any new behavior.
