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

- Unity Editor pin `6000.3.25f1`
- URP 17.3 package foundation
- Input System 1.20.0
- Addressables 2.10.3
- Addressables for Android 1.1.0
- nested Unity generated-folder ignore rules
- DomainBridge and Application assemblies with `noEngineReferences=true`
- Infrastructure / Platform / UI / World / Bootstrap assembly boundaries
- EditMode / PlayMode test assembly structure
- client sync/content/local-state/feature-flag seams
- portrait-first safe-area app shell
- Home / Quests / World / Hero / More navigation
- platform pause/focus lifecycle bridge
- Reduced / Standard / High graphics tier controller
- Input System touch/pointer orbit camera
- prototype Wild Renewal hub
- visually meaningful Wild Renewal prototype pass:
  - layered Hearthwild island/soil edge
  - moss stepping path
  - rebuilt Central Hearth + five-petal Quiet Bloom
  - 12 multi-crown grove trees
  - six waystones
  - dormant workshop frame
  - gathering stone circle
  - grove arch
  - rain pool
  - eight animated Bloom Wisps
- improved Waykeeper silhouette with limbs, mantle, pack, and glow sigil
- starter companion now matches authoritative content identity: Leafglow Fox
- animated Leafglow Fox body/head/legs/tail/glow charm
- World route UI veil now exposes the 3D scene instead of obscuring it
- World status card for The Quiet Bloom / Hearthwild
- interactive Reduced / Standard / High graphics controls
- selected graphics tier persists through PlayerPrefs
- explicit Android/iOS runtime platform profile boundary
- orbit camera ignores the bottom UI/control zone
- reproducible Editor scene generator
- reproducible URP asset generator
- Android development-build entry point
- static scaffold validation workflow

Validation so far:

- Unity Scaffold CI run #1: success
- Unity Scaffold CI run #2: success
- Unity Scaffold CI run #3 on exact head `d90a1c65729567c1b56b95f98ec4606a2c6f7ab9`: success
- Unity Scaffold CI run #6 after the meaningful visual pass: success
- Unity Scaffold CI run #7 after tree-hierarchy correction: success
- Unity Scaffold CI run #8 after graphics persistence/platform-boundary pass: success
- Unity Scaffold CI run #10 after documented URP global-settings API hardening: success
- editor/package/asmdef JSON: valid
- nested generated-directory protections: valid
- pure client DomainBridge/Application assembly boundary: statically enforced
- Build Automation dashboard handoff documented in `docs/UNITY_BUILD_AUTOMATION_SETUP.md`
- official Build Automation pre-export hook wired as `LifePlayed.Client.Editor.LifePlayedProjectConfigurator.PreExport`
- final repository-side diff audit after visual hardening: 36 scoped files, no generated Unity folders/build artifacts, no APK/AAB, no secret-pattern findings
- PR #6 exact code head before this task-record update: `50c6325fdd4262e94685c0cdd76d86596899c6d5`
- PR #6 merge state: clean / mergeable, intentionally draft
- PR #6 merge state: clean, intentionally draft
- Unity cloud minutes consumed: 0

Still unvalidated:

- Unity Editor package resolution/import
- C# compilation under Unity
- Editor-generated URP/scenes
- EditMode/PlayMode execution inside Unity
- Android player build/install/runtime
- touch behavior on a real Android device
- graphics-tier runtime behavior on device

## Current blocker

This ChatGPT workspace has no connected Unity Build Automation action/connector and GitHub currently reports no external Unity build check on PR #6. Static repository work is therefore complete enough to attempt the first purposeful Unity build, but Unity Editor/package compilation and Android runtime proof cannot be claimed from the available tools.

## Next step

Use the Build Automation configuration in `docs/UNITY_BUILD_AUTOMATION_SETUP.md` for one manual Android prototype build from PR #6 / `feat/unity-client-foundation`. Capture the build ID/logs/test summary/APK evidence, repair any actual Unity compiler/import/runtime failure under this same Milestone 4 task, and do not merge until that evidence is green.
