# ACTIVE TASK — Life Played

## Active task / desired outcome

**Task:** Milestone 3 — Content engine + Wild Renewal content skeleton.

**Outcome:** Build the first data-driven content engine and a validated Wild Renewal Saga I skeleton without Unity cloud-build usage: versioned content definitions, Saga/Chapter/StoryNode/NPC/Companion/WorldStructure schemas, deterministic validation, content release loading, and the complete eight-chapter Wild Renewal launch skeleton from the authoritative story architecture.

This is the **only active implementation task** until its Definition of Done is satisfied.

---

## Status

**State:** ACTIVE — Milestone 3 / investigation and content-engine trace

**Unity cloud usage:** 0 minutes required by this milestone.

---

## Previous completed task

### Milestone 2 — Offline Life OS + sync contract — COMPLETE

Merged PR:

- PR #4 → `fd420d68d17385141d29a054f2f7c230cc24eeec`

Implemented:

- offline-capable sync batch contract
- stable mutation IDs
- durable mutation claim/result persistence
- replay-safe canonical results
- optimistic EntityVersion conflicts
- per-mutation transaction isolation
- partial batch success
- sync change cursor/incremental change feed
- Action create/edit/complete
- Quest create/edit
- Campaign create/edit
- CampaignPhase create/edit
- HabitDefinition create/edit
- HabitOccurrence completion
- FocusSession record
- RestPeriod create/edit
- exactly-once Action completion reward
- account XP + Life Skill progression persistence
- PostgreSQL migration `0002_offline_life_os.sql`
- POST `/sync` transport
- superseded generic repository abstraction removed after caller inspection

Validation evidence:

- Exact PR head `00f7c61ca29b7fe4924e7b8048116a980d0bc053`: Headless CI run #17 succeeded.
- Release build: success
- Build warnings: 0
- Build errors: 0
- Tests: **47 passed, 0 failed, 0 skipped**
- Real PostgreSQL 17 Testcontainers coverage passed.
- Migration application remained idempotent.
- Offline replay was tested after constructing a new SyncService/persistence instance.
- Duplicate Action completion returned the original canonical result and produced only one RewardGrant and one ProgressionEvent.
- Stale two-device edit produced an explicit version conflict.
- Partial batch conflict did not block the valid mutation in the same batch.
- Change cursors stopped returning already-consumed changes.
- Final PR diff: 17 scoped files.
- Generated-junk scan: clean.
- Secret-pattern scan: clean.
- Post-merge main Headless CI run #18 succeeded with **47/47** tests.
- Unity cloud minutes used: **0**.

---

## Milestone 3 scope

Implement only the Content Engine + Wild Renewal content skeleton described in `docs/IMPLEMENTATION_PLAN.md` and `docs/STORY_CONTENT_ARCHITECTURE.md`.

### Required content definitions

- ContentRelease
- IdentityWorldDefinition
- SagaDefinition
- ChapterDefinition
- StoryNodeDefinition
- NPCDefinition
- CompanionDefinition
- WorldStructureDefinition
- Chronicle trigger/reference definitions
- Campaign archetype framing definitions

### Required engine behavior

- load a versioned content release from source-controlled data
- validate content before activation/use
- resolve IDs/references deterministically
- detect missing referenced IDs
- detect impossible prerequisite cycles
- require completion paths for canonical chapters
- validate reward/content references
- prevent V1 Wild Renewal content from requiring post-V1 features
- support localization keys rather than hard-coded UI strings in runtime definitions
- preserve a last-known-good content concept in the engine boundary

### Required Wild Renewal content

Implement the full **Saga I: The Quiet Bloom** skeleton from the authoritative architecture:

1. The Empty Hearth
2. Footprints in the Moss
3. The Workshop Wakes
4. Voices Return
5. The Faded Grove
6. The Long Rain
7. Roots Remember
8. The Quiet Bloom

The skeleton must include:

- chapter order/prerequisites
- primary story nodes
- real-life objective hook categories
- core NPC references
- companion references
- world-structure transformation references
- Chronicle trigger references
- completion conditions
- post-Saga state marker

This is structural content, not final prose/localization/art.

### Required validation

- content release parses
- IDs unique
- all references resolve
- cycle detection
- chapter path validation
- V1 feature-boundary validation
- Wild Renewal Saga I validates end to end
- intentionally broken fixtures fail with useful reason codes
- exact-head Release build/tests
- final diff / secrets / generated-junk review

---

## Out of scope

Do not begin during Milestone 3:

- Unity project scaffolding
- 3D Wild Renewal scene
- character/companion models
- VFX
- Android/iOS build
- final narrative prose
- voice acting
- AI Quest Master
- Gridfall content implementation
- Deal Scout
- location/cashback
- live seasonal backend
- remote asset delivery

These belong to later milestones.

---

## Architecture constraints

- Content definitions remain data-driven and source-controlled.
- Content/player state remain separate.
- Content cannot execute arbitrary remote code.
- Canon remains authored/versioned, not AI-generated.
- Story text uses localization keys.
- Reward references point to versioned reward IDs/config, not inline economic values.
- V1 content cannot depend on post-V1 systems.
- Content validation must run without Unity.

---

## Investigation / execution path

1. inspect the existing content-release contract/schema
2. inspect story/domain definitions to avoid duplicate models
3. define the smallest reusable content model
4. implement deterministic content loader + validator
5. create Wild Renewal content data from the authoritative eight-chapter spine
6. add valid and intentionally broken fixtures
7. run headless exact-head validation
8. inspect final diff
9. merge only when the full Milestone 3 Definition of Done is satisfied

---

## Definition of Done

Milestone 3 is complete only when:

- [ ] Versioned content model exists.
- [ ] Content loader works without Unity.
- [ ] Content validator returns deterministic reason codes.
- [ ] Duplicate IDs are rejected.
- [ ] Missing references are rejected.
- [ ] Chapter prerequisite cycles are rejected.
- [ ] Canonical chapters require a completion path.
- [ ] V1 content cannot require post-V1 features.
- [ ] Localization keys are used for player-facing authored text references.
- [ ] Wild Renewal Saga I eight-chapter skeleton exists.
- [ ] Wild Renewal core NPC references exist.
- [ ] Wild Renewal starter/guardian companion references exist.
- [ ] Wild Renewal world-structure transformation references exist.
- [ ] Chronicle triggers are represented.
- [ ] Campaign archetype framing definitions exist.
- [ ] Wild Renewal Saga I validates end to end.
- [ ] Broken content fixtures fail with expected reason codes.
- [ ] Release build passes with 0 errors.
- [ ] Relevant automated tests pass on exact head.
- [ ] No Unity dependency is introduced.
- [ ] No production secret or generated junk is committed.
- [ ] Final diff is focused on Milestone 3.
- [ ] Remaining limitations/blockers are documented.

---

## Blockers / risks

- Final narrative names/prose remain subject to later creative review.
- Final localization language set is deferred; architecture begins with localization keys.
- Final Unity LTS version remains deferred until the Unity milestone.
- Life Played remains a working product name pending formal trademark/domain/store clearance.

---

## Backlog

Immediate next milestone after successful completion:

**Milestone 4 — Unity client foundation.**

That milestone is the first one that can require Unity cloud-build usage.

---

## Next step

Inspect the current content contract/schema and create the dedicated Milestone 3 branch before implementing content definitions.
