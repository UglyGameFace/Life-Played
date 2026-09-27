# Life Played — Implementation Plan

**Status:** Authoritative implementation sequence  
**Depends on:** All foundation docs  
**Goal:** Turn the approved foundation into a production app while minimizing rework, cloud-build waste, and premature complexity.

---

# 1. Delivery principle

Life Played will be built in vertical slices that each produce an observable product capability.

Rules:

- One active implementation task at a time.
- Headless/business logic before Unity-specific presentation when practical.
- No Unity cloud build for doc-only or backend-only work.
- Batch visual/runtime changes into meaningful mobile builds.
- Android is the rapid playable validation platform.
- iOS is validated at defined parity checkpoints.
- Every milestone has its own Definition of Done.
- No milestone is “complete” without evidence.

---

# 2. Branch / PR discipline

Recommended workflow:

- `main` remains releasable/stable.
- Each milestone or focused feature uses a dedicated branch.
- PR scope follows the single-active-task rule.
- Exact-head CI must pass before merge.
- No unrelated cleanup in feature PRs unless required for correctness.
- Final diff reviewed for generated junk, secrets, debug code, accidental asset changes, and conflicts.

---

# 3. Repository structure target

Conceptual target:

```
Life-Played/
├── README.md
├── ACTIVE_TASK.md
├── docs/
├── client/
│   └── LifePlayed.Unity/
├── server/
│   ├── LifePlayed.Api/
│   ├── LifePlayed.Worker/
│   ├── LifePlayed.Domain/
│   ├── LifePlayed.Application/
│   ├── LifePlayed.Infrastructure/
│   └── LifePlayed.Contracts/
├── tests/
│   ├── Domain/
│   ├── Application/
│   ├── Integration/
│   └── Content/
├── content/
│   ├── worlds/
│   ├── story/
│   ├── balance/
│   └── schemas/
├── tools/
└── .github/
```

Final exact structure may change during scaffolding if tooling imposes a better convention.

---

# 4. Milestone 0 — Foundation closeout

Purpose:

- finalize the documents already created
- resolve contradictions
- lock first implementation task

No production code.

Definition of Done:

- all foundation docs readable
- cross-document terminology consistent
- V1/post-V1 boundaries consistent
- active task updated
- first implementation milestone explicitly selected

This milestone is the current task.

---

# 5. Milestone 1 — Headless core skeleton

No Unity build required.

Create:

- solution/repository structure
- Domain project
- Application project
- Contracts project
- initial test projects
- backend API skeleton
- PostgreSQL test infrastructure
- versioned config/content schema skeleton

Implement minimum entities:

- Account
- Action
- Quest
- Campaign
- CampaignPhase
- LifeSkill
- ProgressionEvent
- RewardGrant
- ClientMutation

Implement:

- IDs
- state transitions
- deterministic reward preview/evaluation
- idempotency primitives
- initial persistence abstractions

Validation:

- unit tests
- integration-test DB
- migrations
- static/build checks

Unity minutes used: **0**

---

# 6. Milestone 2 — Offline Life OS + sync contract

Still no required Unity playable build.

Implement:

- Action create/edit/complete
- Quest create/edit
- Campaign + phase structure
- Habit basic model
- FocusSession
- RestPeriod
- local/offline mutation contract
- server sync endpoint
- idempotent completion handling
- canonical reward response
- conflict/version rules

Use a minimal test harness or non-visual client simulation to prove round-trip behavior.

Validation:

- offline mutation tests
- duplicate retry
- app-restart simulation
- two-device conflict scenarios
- reward invariants

Unity minutes used: **0**

---

# 7. Milestone 3 — Content engine + Wild Renewal content skeleton

No mobile build until content validates.

Implement:

- ContentRelease schema
- Saga/Chapter/StoryNode schema
- NPC definitions
- Chronicle trigger schema
- WorldStructure definitions
- Companion definitions
- content validator
- Wild Renewal Saga I data skeleton
- Campaign archetype framing

Validation:

- every Wild Renewal Chapter path validates
- IDs resolve
- no impossible prerequisites
- reward references valid
- no post-V1 dependency

Unity minutes used: **0**

---

# 8. Milestone 4 — Unity client foundation

This is the first real Unity project scaffold.

Create:

- approved LTS Unity project
- URP
- assembly definitions
- Bootstrap
- App shell
- local persistence adapter
- API/sync client
- feature flag/content loader
- basic navigation
- graphics quality framework
- Android project config
- iOS project config

Use temporary/approved prototype assets only where final assets are not ready.

Do not spend cloud build quota until local/CI validations indicate a coherent runtime candidate exists.

---

# 9. Playable Build #1 — First World

First cloud-build checkpoint.

Must visibly include:

- Life Played app launch
- Wild Renewal hub
- stylized player character
- one real 3D companion
- touch navigation
- basic Home / Quests / World / Hero shell
- persistent local state
- Reduced / Standard graphics behavior

This build exists to prove:

- Unity/mobile pipeline
- rendering direction
- character scale
- companion scale
- world scale
- Android install/update path

It should not be a gray-box-only artifact unless a blocker makes that unavoidable.

---

# 10. Milestone 5 — Core Life Played loop

Implement the entire signature loop:

1. Create real Action
2. Persist offline
3. Complete Action
4. Sync mutation
5. Server calculates canonical reward
6. Account XP/Life Skill update
7. Wild Renewal world visibly changes
8. Companion reacts
9. Chronicle entry created
10. state survives restart

This is the first **true vertical slice**.

---

# 11. Playable Build #2 — Core Loop

Must demonstrate on Android:

- create Action
- complete it
- reward presentation
- visible hub transformation
- companion reaction
- Chronicle record
- offline/reconnect
- no duplicate reward from repeat tap/retry

This is the most important early product proof.

If this loop is not satisfying, do not bury the problem under more features.

---

# 12. Milestone 6 — Wild Renewal Saga I production pass

Implement:

- all V1 Saga I chapters
- 4–7 core NPCs
- 3 early companions
- one aspirational companion
- world transformations
- onboarding
- Campaign archetypes
- Momentum/rest/recovery
- first achievements
- first building progression

Art can progress in parallel only when it does not create a separate active implementation task.

The active engineering task remains singular.

---

# 13. Playable Build #3 — Wild Renewal Saga slice

Must show:

- polished onboarding
- early Saga progression
- multiple world transformations
- companion Bond milestone
- Momentum/rest behavior
- meaningful Campaign milestone
- performance tiering

Android primary.

First iOS parity checkpoint occurs once this milestone is stable.

---

# 14. Milestone 7 — Gridfall Founding World

Reuse engine, not content.

Implement:

- Gridfall hub
- tech-specific art/UI treatment
- Saga I content
- 4–7 core NPCs
- drone/AI companion family
- tech Campaign framing
- world transformations
- Gridfall-specific story choices

Validation requirement:

The shared engine must not force Wild Renewal terminology/content into Gridfall.

---

# 15. Playable Build #4 — Identity proof

Android build must demonstrate:

- switch/unlock Founding Worlds
- account progression shared correctly
- story/world state separate
- Wild Renewal feels warm/restorative
- Gridfall feels technical/cyber
- companion types differ
- same real-life Action system powers both

Second iOS parity checkpoint after Android validation.

---

# 16. Milestone 8 — AI Quest Master V1

Implement backend AI orchestration:

- goal breakdown
- Campaign suggestion
- oversized-task breakdown
- recovery/reschedule suggestion
- identity-specific framing
- progress summary

Add:

- cost quotas
- timeout/fallback
- structured output validation
- prompt/template versioning
- user approval/edit step

Core task creation remains functional with AI disabled.

No Unity cloud build unless UI/runtime integration warrants it.

---

# 17. Milestone 9 — Deal Scout V1 beta

Implement:

- Need
- NeedEvidence
- explicit shopping-list linkage
- offer provider adapter
- normalized Offer
- OfferMatch
- ranking
- effective-price display
- save/dismiss/already bought
- affiliate deep link
- disclosure
- conversion reconciliation where provider supports

No cashback wallet.

No background location.

No card linking.

Validation:

- commerce can be disabled by feature flag
- game remains fully usable

---

# 18. Playable Build #5 — Utility + business-model proof

Android build demonstrates:

- stable Life OS
- both Founding Worlds
- AI Quest Master
- Deal Scout beta
- Chronicle
- companions
- progression
- clean navigation
- production-like UX

This is the first build resembling the full V1 shape.

iOS parity checkpoint follows.

---

# 19. Milestone 10 — Production hardening

Focus exclusively on:

- crashes
- sync failures
- performance
- thermal
- battery
- accessibility
- auth
- account deletion
- backup/recovery
- observability
- content rollback
- entitlement/store flows
- security
- privacy declarations

No major new feature enters here without explicit scope change.

---

# 20. Milestone 11 — Store monetization

Implement only approved V1 monetization:

- subscription if retained
- cosmetic purchases
- AI allowance if retained
- expansion entitlement framework if needed

Server-side verification required.

No pay-to-win.

---

# 21. Milestone 12 — Release candidate

Required:

- Android AAB
- iOS archive/build
- signed production config
- production backend
- production content release
- final privacy/store metadata
- complete Wild Renewal Saga I
- complete Gridfall Saga I
- release notes
- support/recovery procedures

Run full `docs/VALIDATION_STRATEGY.md` release checklist.

---

# 22. Cloud build budget policy

Unity cloud builds are not tied to commits.

Primary planned major playable builds:

1. First World
2. Core Loop
3. Wild Renewal Saga slice
4. Identity proof / Gridfall
5. Utility + business proof
6. Release candidates / hotfixes as necessary

Additional builds are allowed for blockers/native integrations, but each should have a clear validation purpose.

Android receives more iteration builds.

iOS receives defined parity builds.

Always re-check the current free-tier quota before enabling automation.

Keep contingency quota available.

---

# 23. Non-Unity CI

Use cheaper validation for:

- .NET compile
- domain tests
- backend tests
- content validation
- schema/migration tests
- secret scan
- formatting/static analysis

Do not trigger Unity because a markdown or API file changed.

---

# 24. Unity CI/build triggers

Potential triggers:

- explicit manual dispatch
- milestone tag
- PR label
- path-based Unity-impact detection

Avoid unconditional push-to-main mobile builds.

---

# 25. Asset production timing

Art production should track proven mechanics.

Priority order:

1. visual style lock
2. hero/companion scale test
3. Wild Renewal hub kit
4. progression transformation assets
5. Wild Renewal NPC/companion production
6. Gridfall kit
7. cinematic/VFX polish
8. cosmetic breadth

Do not manufacture hundreds of cosmetics before players care about the world.

---

# 26. First vertical slice exact definition

The first vertical slice is **Wild Renewal: First Action → First Change**.

User experience:

1. Open Life Played.
2. Enter Wild Renewal.
3. See character + starter companion + partially dormant Hearthwild.
4. Create one Action.
5. Complete it.
6. Completion persists locally immediately.
7. Backend confirms reward.
8. Account XP and one/two Life Skills increase.
9. Hearthwild central structure changes visibly.
10. Companion performs a reaction.
11. Chronicle records the moment.
12. Kill/reopen app.
13. All state remains.
14. Repeat network submission does not duplicate reward.

This slice is the earliest point where Life Played's core thesis is genuinely testable.

---

# 27. First vertical slice non-goals

Do not include yet:

- full Saga
- AI
- affiliate offers
- cashback
- location
- store billing
- multiple Worlds
- deep character creator
- large companion roster
- social/guilds
- mounts
- full live ops

Those are distractions until the core loop feels good.

---

# 28. First vertical slice Definition of Done

## Domain/backend

- Action create/complete works
- reward deterministic/versioned
- idempotency works
- progression persists
- Chronicle event generated
- world progression event generated
- tests pass

## Client

- Android app installs
- Action UI usable
- Wild Renewal hub renders
- character renders
- companion renders/animates
- world transformation runs
- Chronicle visible
- restart preserves state
- graphics Reduced/Standard available

## Validation

- duplicate completion does not duplicate reward
- offline completion queues
- reconnect reconciles
- no crash on suspend/resume
- representative Android device runtime checked
- build artifact SHA/build ID recorded
- known limitations documented

---

# 29. What happens after vertical slice

Do not automatically proceed to more features.

First review:

- Was completing the Action satisfying?
- Did the world change feel meaningful?
- Did companion reaction add attachment?
- Was task creation fast enough?
- Was the reward understandable?
- Did 3D interfere with utility?
- Did performance/battery remain acceptable?

Fix the core loop before expanding.

---

# 30. Implementation completion rule

No milestone completion claim based only on:

- code written
- branch pushed
- one happy-path test
- build succeeded

Completion follows the active-task Definition of Done plus milestone-specific evidence.

