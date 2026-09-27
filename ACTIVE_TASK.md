# ACTIVE TASK — Life Played

## Active task / desired outcome

**Task:** Milestone 4 — Unity client foundation.

**Outcome:** Establish the first real Unity mobile client foundation for Life Played using the current Unity 6.3 LTS line, while preserving the headless architecture and protecting free-tier Build Automation quota. The milestone must produce a structurally correct Unity project, URP/mobile configuration, assembly boundaries, bootstrap/app shell, content/sync integration seams, graphics-quality framework, and the groundwork for the first impactful Android playable build.

This is the **only active implementation task** until its Definition of Done is satisfied.

---

## Status

**State:** ACTIVE — Milestone 4 / runtime foundation built, Unity import/build proof pending

**Unity cloud usage so far:** **0 minutes**

No cloud build should be triggered merely to prove that files exist. The first Unity Build Automation use must satisfy the repository's impactful-build policy.

---

## Previous completed task

### Milestone 3 — Content engine + Wild Renewal content skeleton — COMPLETE

Merged PR:

- PR #5 → `741190d812ca09b926d2d82a11c0c2416409851c`

Implemented:

- headless `LifePlayed.Content` project
- versioned content-release definitions
- chapters as first-class release data
- SHA-256 content-manifest verification
- safe release-directory path handling
- deterministic content validation reason codes
- duplicate ID validation
- cross-reference validation
- localization-key validation
- chapter prerequisite cycle detection
- chapter entry-to-completion path validation
- V1 feature-boundary validation
- last-known-good content activation
- content-definition JSON schema
- Wild Renewal Saga I structural release
- all 8 authoritative chapters:
  1. The Empty Hearth
  2. Footprints in the Moss
  3. The Workshop Wakes
  4. Voices Return
  5. The Faded Grove
  6. The Long Rain
  7. Roots Remember
  8. The Quiet Bloom
- 6 core Wild Renewal NPC roles
- 4 Wild Renewal companions, including starter and guardian tiers
- 8 world-structure transformation targets
- 8 Chronicle triggers
- versioned reward references
- all 10 Campaign archetype framings

Validation evidence:

- Exact PR head `0ae4aef8fcf44cdd6b13f6abbf2f8d2ce8041fd2`: Headless CI run #21 succeeded.
- Release build: success
- Tests: **57 passed, 0 failed, 0 skipped**
- valid Wild Renewal release loaded from disk and passed SHA-256 manifest verification
- all eight chapters validated end to end
- broken duplicate-ID fixture rejected
- broken missing-reference fixture rejected
- prerequisite-cycle fixture rejected
- dead-end/no-completion-path fixture rejected
- post-V1 feature dependency rejected from V1 content
- missing localization key rejected
- corrupt content after a known-good activation rejected while the prior release remained active
- generated-junk scan: clean
- secret-pattern scan: clean
- post-merge main Headless CI run #22 succeeded with **57/57** tests
- Unity cloud minutes used: **0**

---

## Current Unity/toolchain decision

Current official Unity information checked on 2026-09-27:

- **Unity 6.3 LTS** remains the current LTS line and is supported through December 2027.
- Current Unity 6.3 LTS patch selected for this milestone: **6000.3.25f1**.
- URP is a core package tied to Unity 6000.3; Unity documentation maps Unity 6000.3 to **URP 17.3**.
- Input System released for Unity 6000.3: **1.20.0**.
- Addressables released for Unity 6000.3: **2.10.3**.
- Addressables for Android released for Unity 6000.3: **1.1.0**.

Package pins must remain compatible with the selected Editor patch. Do not upgrade casually during the milestone.

---

## Milestone 4 scope

### Required Unity project foundation

- Unity project under `client/LifePlayed.Unity/`
- Unity Editor pinned to `6000.3.25f1`
- URP mobile rendering foundation
- portrait-first mobile configuration
- Input System
- Addressables foundation
- Android/iOS platform configuration boundaries
- bootstrap scene
- app-shell/navigation scene/layer
- Wild Renewal world-hub scene skeleton
- character/companion presentation seams
- graphics quality framework
- safe-area/touch-first UI foundation
- local persistence integration seam
- API/sync integration seam
- content-release integration seam
- feature-flag/config seam
- assembly-definition dependency boundaries

### Required architectural separation

At minimum:

- `LifePlayed.Client.DomainBridge`
- `LifePlayed.Client.Application`
- `LifePlayed.Client.Infrastructure`
- `LifePlayed.Client.Presentation.UI`
- `LifePlayed.Client.Presentation.World`
- `LifePlayed.Client.Platform`

The exact names may be refined after inspecting Unity package/assembly constraints, but Domain/business authority must not migrate into MonoBehaviours.

### Required first presentation proof

The foundation must be capable of hosting:

- Wild Renewal hub
- player-character presenter
- one companion presenter
- touch camera/navigation
- Home / Quests / World / Hero / More shell
- Reduced / Standard / High quality selection
- app suspend/resume-safe bootstrap state

Placeholder/prototype assets are allowed during this foundation milestone. The first cloud playable build must still be visually meaningful enough to justify quota.

---

## Build Automation policy for this milestone

Do **not** build on every commit.

Before the first cloud build:

1. finish project/package/config scaffold
2. validate project-file consistency statically where possible
3. add EditMode/PlayMode test assembly structure
4. batch bootstrap/navigation/world skeleton changes
5. inspect diff for generated Unity junk
6. only then trigger one purposeful Android validation build

The intended first cloud build is **Playable Build #1 — First World**, not “Unity opened successfully.”

---

## Out of scope

Do not begin during Milestone 4:

- full Action → reward → world-transformation vertical slice
- final Wild Renewal environment art
- final character creator
- final companion models
- full Saga gameplay
- Gridfall
- AI Quest Master
- Deal Scout
- location/cashback
- store monetization

Those belong to later milestones.

---

## Investigation / execution path

1. verify selected Unity LTS/editor/package compatibility
2. inspect repository ignore rules and existing client paths
3. inspect current headless contracts that the Unity client must consume
4. choose the safest shared-contract strategy without copying server authority into Unity
5. scaffold the Unity project and package manifest
6. establish assembly definitions and bootstrap boundaries
7. create app shell and scene skeletons
8. establish quality/input/platform/config seams
9. add Unity-specific tests where possible
10. use Unity Build Automation only once the milestone has an impactful Android validation target
11. inspect final diff and exact build/test evidence before completion

---

## Definition of Done

Milestone 4 is complete only when:

- [ ] Unity project exists at the approved repository path.
- [ ] Editor version is pinned to 6000.3.25f1.
- [ ] URP/mobile rendering foundation exists.
- [ ] Input System is configured.
- [ ] Addressables foundation exists.
- [ ] Bootstrap/app-shell/world scene structure exists.
- [ ] Assembly boundaries prevent presentation code from owning authoritative game rules.
- [ ] portrait/safe-area/touch-first UI foundation exists.
- [ ] Reduced / Standard / High graphics quality framework exists.
- [ ] content/sync/local-storage integration seams exist.
- [ ] Android and iOS platform boundaries exist.
- [ ] EditMode/PlayMode test structure exists.
- [ ] generated Unity folders/artifacts are ignored.
- [ ] relevant static/headless validation remains green.
- [ ] one purposeful Android validation build is produced when it is worth the quota.
- [ ] Android build/runtime evidence is recorded.
- [ ] no unrelated post-Milestone-4 feature work is mixed in.
- [ ] final diff is reviewed.
- [ ] remaining limitations/blockers are documented.

---

## Blockers / risks

- The environment available through repository tools does not itself provide an interactive Unity Editor; repository scaffolding can be prepared directly, while true Editor/import/runtime proof must come from Unity-capable CI/Build Automation.
- The first cloud build must be deliberately batched to protect the free tier.
- Final mobile bundle IDs remain deferred while the Life Played name is still pending formal trademark/domain/store clearance.

---

## Backlog

Immediate next milestone after successful completion:

**Milestone 5 — Core Life Played loop**

That milestone proves:

`Create Action → Complete Action → Authoritative Reward → Visible Wild Renewal Change → Companion Reaction → Chronicle → Restart Persistence`

---

## Current implementation status

Implemented on `feat/unity-client-foundation` / PR #6:

### Project/toolchain

- Unity Editor pinned to `6000.3.25f1`
- URP 17.3
- Input System 1.20.0
- Addressables 2.10.3
- Addressables for Android 1.1.0
- Unity Test Framework 1.6.0
- nested Unity-generated folders ignored
- generated scenes, URP assets, and staged StreamingAssets content ignored
- DomainBridge/Application assemblies remain free of UnityEngine references
- Infrastructure / Platform / UI / World / Bootstrap assembly boundaries exist

### App shell / mobile UX

- portrait-first safe-area shell
- real route bodies for Home / Quests / World / Hero / More
- persisted last route
- World route uses a translucent veil so the 3D scene remains visible
- World status card for The Quiet Bloom / Hearthwild
- Reduced / Standard / High quality controls
- persisted graphics tier
- development-only runtime diagnostics showing platform, route, quality tier, FPS, target FPS, world-render state, content release/schema, safe-area size, and pause state

### Wild Renewal presentation

- layered Hearthwild island and soil edge
- moss stepping path
- rebuilt Central Hearth and five-petal emissive Quiet Bloom
- 12 multi-crown grove trees
- six waystones/runes
- dormant workshop frame
- gathering circle
- grove arch
- rain pool
- eight animated Bloom Wisps
- improved Waykeeper silhouette with limbs, mantle, pack, and glow sigil
- authored starter identity **Leafglow Fox**
- animated Leafglow Fox body/head/legs/tail/glow charm
- presentation-only **Call Leafglow** interaction with hop/spin/pulse reaction
- one-finger orbit
- two-finger Enhanced Touch pinch zoom
- bottom UI/control zone excluded from world-camera gestures

### Mobile runtime / resilience

- 3D world suspends outside World
- 3D world also suspends on app pause/focus loss
- low-memory notification downshifts to Reduced quality and unloads unused assets
- Android/iOS runtime platform boundary
- Android edge-to-edge rendering with safe-area UI
- Android optimized frame pacing
- fullscreen start
- `runInBackground=false`

### Persistence / content

- PlayerPrefs restricted to lightweight client settings
- separate JSON-backed offline mutation queue + sync cursor
- offline store recreation/deduplication/acknowledgement EditMode coverage
- authoritative Wild Renewal release remains single-source under `content/releases/wild-renewal-v1`
- pre-export stages the validated release into StreamingAssets
- Android-safe `UnityWebRequest` manifest loader
- runtime diagnostics expose loaded content release/schema and whether it came from the bundled release or last-known-good cache
- staged `content.json` is SHA-256 checked against the authoritative manifest during pre-export
- runtime content is SHA-256 verified again before activation
- verified bundled content seeds a persistent last-known-good cache under `persistentDataPath`
- bundled-content failure can fall back to the verified last-known-good cache
- minimum client version is enforced against the content manifest
- Playable Build #1 client version pinned to `0.1.0`
- Development APK writes a persistent thread-safe Unity runtime log under `lifeplayed/diagnostics/development.log`

### Build Automation / quota protection

- official pre-export hook:
  `LifePlayed.Client.Editor.LifePlayedProjectConfigurator.PreExport`
- hard gate for `ENABLE_INPUT_SYSTEM`
- hard gate for Android target and Unity `6000.3.25f1`
- mobile URP generation and validation
- authored-content staging validation
- Development APK forced on
- profiler auto-connect and deep profiling forced off
- ARM64-only target
- IL2CPP backend
- IL2CPP Debug C++ configuration for faster prototype compilation
- EditMode and PlayMode tests configured for the cloud build
- Addressables production content build intentionally deferred until it has real groups/catalog content

---

## Repository-side validation evidence

Latest Playable Build #1 candidate:

- PR #6 head: `20ad197db38d2984e20a20b8771eb9bb64526e87`
- Unity Scaffold CI run #29: **success**
- PR state: clean / mergeable / intentionally draft
- changed files: 41 scoped files
- generated-junk scan: clean
- APK/AAB/keystore scan: clean
- secret-pattern scan: clean
- Unity cloud minutes consumed by repository preparation: **0**

Additional candidate hardening:

- pre-export SHA-256 verifies authoritative `content.json`
- generated verification metadata is staged with the build
- runtime SHA-256 verification happens before content activation
- verified bundled content seeds a last-known-good cache
- bundled content failure can fall back to cached verified content
- content minimum-client version is enforced
- diagnostics distinguish `bundle` vs `cache`
- Development APK persists Unity logs from main and worker threads under `lifeplayed/diagnostics/development.log`
- StreamingAssets loading follows Unity's Android-safe `UnityWebRequest` path

The static validator currently protects:

- editor/package pins
- assembly boundaries
- generated-folder ignores
- public URP global-settings APIs
- URP runtime quality controls
- Android safe-area/frame-pacing settings
- settings-vs-offline-database separation
- authoritative content staging
- Enhanced Touch pinch support
- route-body presence
- Input System build preflight
- Android diagnostic-build configuration
- ARM64/IL2CPP/Debug compiler configuration
- mobile lifecycle / low-memory wiring

---

## Playable Build #1 readiness

Repository-side readiness is **GREEN**.

The first Unity build is now expected to prove all remaining Editor/device-only facts in one attempt:

1. package resolution/import
2. C# compilation under Unity 6000.3.25f1
3. Active Input Handling really defines `ENABLE_INPUT_SYSTEM`
4. URP/global-settings/scenes generate correctly
5. authoritative Wild Renewal content stages, SHA-256 verifies, loads, and seeds last-known-good cache
6. EditMode tests pass
7. PlayMode tests pass
8. ARM64 IL2CPP Development APK exports
9. APK installs and launches on Android
10. safe-area UI is correct
11. all five routes respond
12. world sleeps outside World
13. one-finger orbit and two-finger pinch work without fighting UI
14. Call Leafglow triggers the companion reaction
15. Reduced / Standard / High switch correctly
16. selected route and graphics tier survive restart
17. background/resume safely suspends/restores world presentation
18. development log persists enough evidence to diagnose runtime exceptions without another cloud build

Use `docs/PLAYABLE_BUILD_1_VALIDATION.md` as the authoritative one-build test sheet.

---

## Still unvalidated

- Unity Editor package resolution/import
- Unity C# compilation
- actual Active Input Handling serialized state
- Editor-generated URP/scenes
- EditMode/PlayMode execution inside Unity
- Android Gradle/IL2CPP export
- APK installation/runtime
- real-device touch/safe-area/frame pacing
- real-device suspend/resume behavior

---

## Current blocker

This ChatGPT workspace has no connected Unity Build Automation action/connector and GitHub reports no external Unity build check for PR #6. Repository preparation can be completed directly, but true Unity Editor/import/build/device proof must come from the configured Unity Build Automation target.

---

## Repository freeze point

Repository-side Milestone 4 preparation is now frozen at:

`20ad197db38d2984e20a20b8771eb9bb64526e87`

Do not add speculative code-only features before Playable Build #1 unless a newly discovered authoritative-document mismatch materially affects the build. At this point, additional unexecuted Unity code is more likely to increase first-build risk than reduce it.

## Next step

Do **not** merge PR #6 yet.

Run exactly one manual Build Automation attempt using `docs/UNITY_BUILD_AUTOMATION_SETUP.md`, then validate the resulting APK with `docs/PLAYABLE_BUILD_1_VALIDATION.md`. Capture the build ID, exact commit SHA, test summary, first compiler/import/Gradle error if any, and Android runtime observations. Repair only evidence-backed failures under this same Milestone 4 task.
