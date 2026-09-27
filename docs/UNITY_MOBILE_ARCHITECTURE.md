# Life Played — Unity & Mobile Architecture

**Status:** Authoritative client architecture foundation  
**Depends on:** `docs/MASTER_PRODUCT_BIBLE.md`, `docs/V1_SCOPE.md`, `docs/CORE_DOMAIN_MODEL.md`, `docs/GAME_ECONOMY.md`, `docs/STORY_CONTENT_ARCHITECTURE.md`

---

# 1. Client architecture goal

Life Played must deliver premium stylized 3D on Android and iOS without making the entire product dependent on expensive or slow Unity build cycles.

The guiding rule is:

> **Headless first. Unity last.**

Core game/business rules should be expressible in plain testable C#/.NET-style domain code where practical.

Unity owns:

- Rendering
- Scenes
- Character presentation
- Companion presentation
- Animation
- VFX
- Audio
- Touch input
- 3D world interaction
- Platform presentation
- Mobile UI integration

Unity should not own every business rule merely because the app is rendered in Unity.

---

# 2. Unity version policy

The exact Unity version is **not yet locked**.

Implementation must choose:

- A current supported **LTS** release
- URP support appropriate for mobile
- Stable Android/iOS toolchain compatibility
- Compatible Addressables/content pipeline
- Compatible testing/build tooling

Version upgrades are explicit tasks with validation.

Do not chase every new Unity release mid-development.

---

# 3. Render pipeline direction

**URP is the default technical direction** for Life Played.

Why:

- Mobile-friendly
- Supports stylized lighting
- Supports post-processing
- Supports scalable shader quality
- Fits the required 3D/VFX direction
- Cross-platform Android/iOS support

HDRP is not appropriate for the V1 mobile target.

Built-in pipeline should not be chosen merely for familiarity if it undermines the long-term visual target.

---

# 4. Client layering

Recommended logical layers:

```
LifePlayed.Client
├── Bootstrap
├── Presentation
│   ├── UI
│   ├── World
│   ├── Character
│   ├── Companion
│   ├── VFX
│   └── Audio
├── Application
│   ├── UseCases
│   ├── Navigation
│   ├── Sync
│   └── FeatureState
├── Domain
│   ├── LifeOS
│   ├── Progression
│   ├── World
│   ├── Story
│   └── Commerce
├── Infrastructure
│   ├── API
│   ├── LocalStore
│   ├── Content
│   ├── Auth
│   ├── Notifications
│   └── Platform
└── Tests
```

Dependency direction:

`Presentation → Application → Domain`

Infrastructure implements interfaces consumed by Application/Domain.

Domain code should not reference Unity scene objects, GameObjects, MonoBehaviours, or platform SDKs.

---

# 5. Assembly boundaries

Unity Assembly Definition files should enforce architectural boundaries.

Suggested assemblies:

- `LifePlayed.Domain`
- `LifePlayed.Application`
- `LifePlayed.Infrastructure`
- `LifePlayed.Presentation.UI`
- `LifePlayed.Presentation.World`
- `LifePlayed.Platform.Android`
- `LifePlayed.Platform.iOS`
- corresponding test assemblies

Purpose:

- Faster compilation
- Clear dependencies
- Easier headless testing
- Reduced accidental coupling
- Platform isolation

---

# 6. Scene strategy

Avoid one gigantic permanent scene.

Recommended scene model:

## Bootstrap Scene

Always lightweight.

Responsibilities:

- Dependency composition
- Global services
- Auth/session bootstrap
- Local database initialization
- Remote config/content bootstrap
- Initial sync decision
- Navigation entry

## App Shell Scene / UI Layer

Contains persistent navigation and 2D application UI.

## World Hub Scene

Loads the active Identity World hub.

Examples:

- Wild Renewal hub
- Gridfall hub

## Cinematic/Encounter overlays

Prefer additive loading or isolated lightweight scenes for:

- Major reward sequences
- Companion evolution
- Saga climax
- Special interaction

## Character Creator

Can be isolated to reduce persistent memory cost.

Do not keep every world, character creator, and cinematic loaded simultaneously.

---

# 7. UI strategy

The productivity side must behave like a serious mobile application.

Required UI characteristics:

- Portrait-first
- One-handed friendly
- Touch-safe
- Responsive across phone aspect ratios
- Safe-area aware
- Accessible
- Fast transitions
- Minimal 3D dependency for ordinary task management

Primary navigation remains:

- Home
- Quests
- World
- Hero
- More

3D rendering should not be required just to edit a task.

The client may suspend/reduce world rendering while deep in utility UI to save battery.

---

# 8. World/UI coexistence

Life Played is one product, but not every screen needs a live 3D scene.

Modes:

## Utility-heavy screen

Examples:

- Edit Action
- Campaign planner
- Deal Scout
- Settings

Behavior:

- 3D world may pause, downshift, or be unloaded
- UI remains responsive
- battery/GPU use reduced

## World-heavy screen

Examples:

- Hub
- companion interaction
- reward transformation
- Saga scene

Behavior:

- full appropriate render tier
- cinematic VFX where justified

This separation protects mobile battery and thermal performance.

---

# 9. Portrait orientation

V1 is **portrait-first**.

Reasons:

- Frequent task interactions
- One-handed use
- Native mobile feel
- Easier integration of planning and RPG layers

Landscape is not required for V1.

Future specialized scenes may support landscape only if a clear game need emerges.

---

# 10. Local persistence

V1 requires structured local persistence.

Do not use PlayerPrefs as the application database.

Local persistence must support:

- Actions
- Quests
- Campaigns
- Habit occurrences
- Focus sessions
- cached progression
- cached world/story state
- cached content definitions
- pending ClientMutations
- sync cursors
- settings

Exact library/SQLite implementation is deferred to package selection.

Client storage is a cache + offline work store, not the final authority for valuable state.

---

# 11. Local repository interfaces

Domain/Application code should consume abstractions such as:

- `IActionRepository`
- `IQuestRepository`
- `ICampaignRepository`
- `ISyncQueue`
- `IContentRepository`
- `IProgressionCache`

This allows changing SQLite/local implementation without rewriting gameplay.

---

# 12. Sync architecture

The client maintains a durable mutation queue.

Flow:

```
User action
  ↓
local transaction
  ↓
ClientMutation created
  ↓
UI updates optimistically where safe
  ↓
sync worker
  ↓
server idempotent mutation
  ↓
canonical response
  ↓
local reconciliation
```

Rules:

- Every mutation has globally unique ID.
- Network retry does not create duplicate rewards.
- Client preserves unsynced work across app restart.
- Sync status is observable.
- Conflicts use entity-specific policy.

---

# 13. Offline behavior

Offline-safe V1 operations include:

- Create/edit ordinary Action
- Complete ordinary Action locally
- View cached Quests/Campaigns
- Record HabitOccurrence
- Record FocusSession
- View cached Chronicle/world state
- Character cosmetic selection where locally owned data is known

Connection-required examples:

- AI Quest Master
- New entitlement purchase
- Deal Scout refresh
- affiliate activation
- canonical reward finalization
- account recovery
- remote content updates

The UI should distinguish:

- completed locally
- syncing
- confirmed

without making normal offline use feel broken.

---

# 14. Server authority UX

When a safe local action occurs, Life Played may show immediate provisional feedback.

Example:

`Action completed → animation + pending progression preview`

Once authoritative response arrives:

`confirmed reward → canonical values`

For ordinary trusted Life OS actions, server response should normally match the deterministic client preview.

The client cannot authoritatively create:

- premium entitlement
- cash reward
- inventory ownership
- authoritative XP
- currency balance

---

# 15. Content delivery

Use a versioned remote content layer for data-driven content.

Examples:

- Story graph
- Dialogue
- Reward configs
- Balance configs
- Feature flags
- Offer configuration
- Campaign archetype text

Client keeps a last-known-good cached content release.

If new content fails validation/download:

- keep prior release
- do not brick the app

---

# 16. Addressables direction

Unity Addressables are the default asset-delivery direction for content that benefits from:

- asynchronous loading
- memory control
- optional downloads
- later remote content expansion

V1 should use Addressables intentionally, not convert every tiny asset into a remote dependency.

Use cases:

- Identity World scene assets
- Companion asset groups
- larger cosmetic groups
- future episodic content

Critical bootstrap/UI assets remain local.

---

# 17. Asset ownership

Repository should distinguish:

- source-controlled runtime assets
- generated/imported Unity artifacts
- source art/DCC files
- remote content assets

Do not commit:

- Library
- Temp
- Build outputs
- local IDE caches

Large binary/source-art policy is finalized before large art production begins.

Git LFS may be used when justified, but must not become an uncontrolled storage bill.

---

# 18. Character architecture

V1 player character uses:

- shared base rig strategy
- modular appearance
- modular outfit slots
- world-aware style variants
- shared core animation set

Avoid unique skeletons for every cosmetic.

Character system should support:

- idle
- locomotion
- celebration
- world interaction
- companion interaction
- cinematic reaction

without duplicating animation controllers per outfit.

---

# 19. Companion architecture

Companion systems share:

- common movement contract
- bond reaction events
- follow/idle states
- interaction hooks
- VFX hooks
- cosmetic hooks

Different families may have different locomotion:

- quadruped
- flying
- hovering
- biped/mech
- abstract/orb

Do not force every companion onto one animation topology.

Use shared behavior contracts with family-specific implementations.

---

# 20. Companion runtime budget

Mobile performance target requires explicit companion limits.

V1:

- one primary active companion in normal hub play
- additional ambient creatures/NPCs are world budget items
- high-end VFX only when needed
- off-screen companion logic throttles

A collectible roster does not imply every owned companion exists live in the scene.

---

# 21. NPC architecture

NPC presentation uses:

- pooled/reusable interaction framework
- shared locomotion/interaction systems
- identity-specific models/animation profiles
- data-driven dialogue/state

NPC brain/ambient behavior should not run expensive per-frame logic when far from the player.

---

# 22. VFX architecture

VFX classes:

## Ambient

- fireflies
- drifting leaves
- subtle digital particles
- environmental glow

Low-cost and scalable.

## Gameplay feedback

- task completion
- building progress
- companion reaction

Short-lived.

## Cinematic

- building transformation
- Saga completion
- rare companion evolution
- major Chronicle landmark

Highest visual budget.

Cinematic effects are rare enough to feel special.

---

# 23. Graphics quality tiers

Minimum V1 tiers:

## Reduced

- lower render scale
- minimal shadows
- reduced particles
- simplified weather
- reduced post-processing

## Standard

- intended mainstream presentation

## High

- improved shadows
- richer particles
- improved environment effects

Optional:

## Cinematic

- supported devices only
- premium effects
- higher-quality lighting/post

The exact device classification is empirical and must be tested.

---

# 24. Performance targets

Targets are finalized after first vertical slice profiling, but architecture should aim for:

- stable interactive frame pacing
- memory discipline
- reasonable thermal behavior
- reasonable battery impact

Prefer a stable 30 FPS on weak supported devices over unstable 60 FPS.

High-capability devices may target 60 FPS.

V1 must include an FPS/render-quality control policy.

---

# 25. Battery strategy

Life Played is an app people may open frequently.

Battery protections:

- reduce/pause world rendering behind utility UI
- throttle background animations
- no unnecessary GPS polling
- avoid permanent high-cost post-processing
- pause expensive effects when app unfocused
- use event-driven systems instead of per-frame polling where practical

A productivity app that melts the battery is philosophically impressive but commercially stupid.

---

# 26. Thermal strategy

Validation must include sustained sessions.

Potential adaptive responses:

- lower render scale
- reduce particle counts
- reduce shadow quality
- reduce weather complexity
- cap frame rate

Do not wait for store reviews to discover thermal throttling.

---

# 27. Android platform boundary

Android-specific code belongs behind platform interfaces.

Potential integrations:

- Play Integrity
- Google Play Billing
- notifications
- deep links
- share intents
- future Health Connect
- future location services

Unity domain/application code must not depend directly on Android Java/Kotlin APIs.

---

# 28. iOS platform boundary

iOS-specific code belongs behind platform interfaces.

Potential integrations:

- App Attest / DeviceCheck
- StoreKit
- notifications
- universal links
- share sheets
- future HealthKit
- future Core Location

Unity domain/application code must not depend directly on Objective-C/Swift APIs.

---

# 29. Platform parity rule

A feature is not “cross-platform complete” because it compiles.

It must have:

- equivalent user intent
- equivalent privacy controls
- appropriate platform implementation
- equivalent authoritative backend behavior

Platform UX may differ where Android/iOS conventions differ.

---

# 30. Notification architecture

Push/local notifications are presented through one application-level abstraction.

Sources:

- due Action
- HabitOccurrence
- Campaign reminder
- content/event alert
- reward state

The domain decides notification intent.

Platform adapters perform delivery.

---

# 31. Deep-link architecture

Life Played should support structured deep links for:

- Action/Quest
- Campaign
- World
- Chronicle entry
- Deal
- optional future Sponsored Quest

Links must resolve safely when:

- user is logged out
- content expired
- offer expired
- target world not downloaded

---

# 32. Error handling

User-facing failures use domain-aware states.

Examples:

- offline
- syncing
- authentication expired
- content unavailable
- AI unavailable
- offer expired

Do not surface raw HTTP/SDK exceptions to users.

Central logging/telemetry captures underlying cause.

---

# 33. Free-tier build strategy

The project must conserve Unity cloud-build resources.

## Rule

**No Unity cloud build on every commit.**

Cloud builds are reserved for changes that require Unity/device proof.

---

# 34. Build categories

## Category A — No Unity build

Examples:

- Documentation
- Backend/API changes
- schema design
- story content text validation
- pure domain/economy tests
- non-Unity C# logic where separately testable
- configuration linting

These should not consume Unity build quota.

## Category B — Unity validation build

Use when changes affect:

- serialization
- Unity package compatibility
- scene/prefab references
- Addressables
- native plugins
- animation/VFX integration
- mobile UI/runtime behavior

Batch related changes before building.

## Category C — Playable milestone build

Required for:

- first playable
- core loop milestone
- new Identity World milestone
- major rendering/VFX milestone
- release candidate

These are the most important cloud builds.

---

# 35. Android-first iteration policy

Because the primary developer/tester workflow is Android-only:

- Android is the rapid playable validation platform.
- Meaningful changes are batched.
- Android artifacts are produced at milestone checkpoints.
- iOS is not rebuilt after every Android iteration.

This does **not** reduce iOS to a second-class product.

It reduces unnecessary Mac build consumption.

---

# 36. iOS parity checkpoints

iOS cloud builds occur at defined checkpoints such as:

- client foundation
- first complete core loop
- first complete Founding World
- commerce/entitlement integration
- release candidate

Platform-specific code changes may force additional targeted iOS builds.

---

# 37. Build budget guard

Before enabling cloud automation:

- check the current Unity free-plan allowance
- record monthly quotas in repository operations docs
- configure warnings
- reserve contingency capacity

Operational target:

- do not deliberately consume the final 10–20% of monthly free allowance during normal development
- preserve emergency/hotfix capacity

Exact free-plan minute counts must be rechecked when automation is configured because vendor pricing/quotas can change.

---

# 38. Impactful build requirement

A playable cloud build should normally prove at least one meaningful outcome.

Examples:

## Milestone 1 — First World

- character
- companion
- hub
- touch camera/navigation

## Milestone 2 — Real-life loop

- create Action
- complete Action
- authoritative progression
- visible world change
- companion reaction
- Chronicle entry

## Milestone 3 — Second identity

- Gridfall hub
- tech UI language
- drone/AI companion
- same underlying Life OS/progression

## Milestone 4 — Cinematic progression

- building transformation
- VFX
- lighting
- reward sequence

A cloud build whose only visible change is text spacing is usually not worth the quota unless it fixes a blocking issue.

---

# 39. Test architecture

Testing layers:

## Pure domain tests

No Unity runtime required where practical.

Targets:

- reward formulas
- Momentum
- task relationships
- state transitions
- content validation
- sync/idempotency helpers

## Unity EditMode tests

Targets:

- Unity serialization
- ScriptableObject/content adapters
- asset references
- Unity-specific application glue

## Unity PlayMode tests

Targets:

- scene bootstrap
- UI navigation
- world interaction
- character/companion behavior
- Addressables integration

## Device tests

Targets:

- Android/iOS lifecycle
- local persistence
- native plugins
- notifications
- performance
- thermal/battery
- touch/safe areas

---

# 40. CI principle

CI should run the cheapest relevant validation first.

Conceptually:

```
format/static checks
   ↓
pure tests
   ↓
content validation
   ↓
Unity-specific validation only when needed
   ↓
playable build at milestone gate
```

Do not spend cloud minutes to discover a typo that a cheap validation step could have caught.

---

# 41. First vertical slice architecture target

The first actual Unity vertical slice must prove:

1. App bootstrap
2. Local account/profile stub compatible with future auth
3. Create one Action
4. Persist it locally
5. Complete it
6. Generate idempotent mutation
7. Receive/mock authoritative reward through interface
8. Update Account/Life Skill progression
9. Trigger visible 3D hub change
10. Trigger companion reaction
11. Create Chronicle entry
12. Restart app and preserve state

The first slice does not need real affiliate, AI, background location, or live payments.

---

# 42. Mobile architecture acceptance criteria

This architecture is implementation-ready when:

- Domain code can be tested without scene dependencies.
- Unity presentation does not own authoritative economy logic.
- UI can operate without rendering the full 3D world continuously.
- local persistence supports offline Life OS use.
- mutation queue is durable/idempotent.
- Android/iOS SDK integrations are isolated.
- content and assets can be versioned.
- graphics scale down gracefully.
- companion/NPC systems have explicit runtime budgets.
- build policy protects free-tier cloud quota.
- first vertical slice can be built without committing to post-V1 providers.

