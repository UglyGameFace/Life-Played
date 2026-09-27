# ACTIVE TASK — Life Played

## Active task / desired outcome

**Task:** Milestone 1 — Headless core skeleton.

**Outcome:** Establish the first production code foundation for Life Played without using Unity cloud-build minutes: modern .NET solution structure, domain/application/contracts/API/test projects, core IDs/state primitives, deterministic reward/economy primitives, and automated headless validation.

This is the **only active implementation task** until its Definition of Done is satisfied.

---

## Status

**State:** ACTIVE — Milestone 1 / headless core skeleton

**Unity cloud usage:** 0 minutes required by this milestone.

---

## Previous completed task

### Production foundation — COMPLETE

The pre-implementation product foundation was completed and cross-checked before production code began.

Authoritative documents:

- `docs/FOUNDATION_INDEX.md`
- `docs/MASTER_PRODUCT_BIBLE.md`
- `docs/V1_SCOPE.md`
- `docs/CORE_DOMAIN_MODEL.md`
- `docs/GAME_ECONOMY.md`
- `docs/STORY_CONTENT_ARCHITECTURE.md`
- `docs/UNITY_MOBILE_ARCHITECTURE.md`
- `docs/BACKEND_ARCHITECTURE.md`
- `docs/COMMERCE_REWARDS_ARCHITECTURE.md`
- `docs/PRIVACY_SECURITY_FRAUD.md`
- `docs/VALIDATION_STRATEGY.md`
- `docs/IMPLEMENTATION_PLAN.md`

Foundation validation completed:

- Product Bible exists and reflects the agreed product.
- V1 scope and explicit post-V1 backlog are documented.
- Core gameplay/progression rules are documented.
- Identity-world/story architecture is documented.
- Commerce, Deal Scout, location/reward, and fraud boundaries are documented.
- Privacy/consent principles are documented.
- Offline/sync and server-authority rules are documented.
- Unity/mobile architecture is documented.
- Backend architecture is documented.
- Initial domain model is documented.
- Validation strategy is documented.
- Initial milestones and first vertical slice are documented.
- Cross-document audit found no material V1/post-V1 contradiction.
- No competing temporary design document remains.
- README links to the authoritative foundation index.

No production code was present during foundation closeout.

---

## Scope

Milestone 1 includes only the headless foundation required by `docs/IMPLEMENTATION_PLAN.md`.

### Required project structure

- modern .NET SDK baseline
- Domain project
- Application project
- Contracts project
- API project
- Worker project skeleton if useful to the outbox boundary
- unit/integration test projects
- shared build settings

### Required first domain primitives

- opaque globally unique IDs
- entity version/concurrency primitive
- Action state
- Quest state
- Campaign state
- Campaign phase state
- Life Skill identifiers/definitions
- Progression event
- Reward grant
- Client mutation/idempotency primitive

### Required economy primitives

Implement the current versioned foundation rules needed for tests, including:

- account-level next-XP curve
- Life Skill next-XP curve
- duration score
- effort score inputs/bounds
- ordinary account XP clamp
- skill-XP pool
- duplicate diminishing-return sequence
- focus reward taper boundaries
- Momentum value bounds/primitives

### Required validation

- build/compile
- targeted unit tests
- economy invariant tests
- state-transition tests
- no Unity dependency in Domain/Application
- no secrets/generated junk
- final diff review

---

## Out of scope

Do not begin these during Milestone 1:

- Unity project scaffolding
- 3D assets
- Android/iOS builds
- database provider deployment
- production authentication provider
- AI provider integration
- Deal Scout provider integration
- location permissions
- cashback
- live merchant systems
- Wild Renewal scene
- Gridfall scene

These belong to later milestones.

---

## Architecture decisions for this milestone

- Backend/domain baseline: **.NET 10 LTS**
- Client remains Unity later; this milestone contains no Unity runtime dependency.
- PostgreSQL remains the planned authoritative relational database, but production provider selection/deployment is not part of this milestone.
- Domain code remains free of ASP.NET, EF Core, Unity, provider SDKs, and platform SDKs.
- Authoritative reward/economy rules are versioned and deterministic.
- Client-originated IDs/mutations must be safe for offline creation and idempotent server processing.

---

## Findings / execution path

1. Scaffold the headless project structure.
2. Establish shared compiler/analyzer settings.
3. Implement domain IDs/value objects/state enums.
4. Implement versioned economy calculation primitives from `docs/GAME_ECONOMY.md`.
5. Implement first progression/reward/client-mutation records.
6. Add focused unit tests.
7. Add the cheapest headless CI/build validation.
8. Run exact-head validation.
9. Inspect final diff and remove accidental/generated/conflicting files.

---

## Definition of Done

Milestone 1 is complete only when:

- [ ] The .NET 10 headless project structure exists.
- [ ] Domain has no Unity/ASP.NET/EF/provider dependency.
- [ ] Core IDs and state primitives exist.
- [ ] Versioned economy primitives exist.
- [ ] Economy formula tests cover documented examples and boundaries.
- [ ] Duplicate-diminishing-return tests pass.
- [ ] Focus-taper boundary tests pass.
- [ ] State-transition tests pass.
- [ ] ClientMutation/idempotency primitive exists and is tested.
- [ ] Solution/projects compile on exact head.
- [ ] Relevant automated tests pass on exact head.
- [ ] No production secret or generated junk is committed.
- [ ] Final diff is focused on Milestone 1.
- [ ] Remaining limitations/blockers are documented.

---

## Changes

No Milestone 1 code has been committed yet.

---

## Validation / results

Pending implementation.

---

## Cleanup / conflicts

Foundation documents are authoritative and must not be duplicated as code comments/spec files.

No pre-existing production code exists to preserve.

---

## Blockers / risks

- The Life Played product name remains a working name pending formal trademark/domain/store clearance; do not lock permanent mobile bundle IDs yet.
- Exact Unity LTS version remains intentionally deferred until the Unity milestone.
- Production backend hosting/auth/database provider remains intentionally deferred.

---

## Backlog

Post-Milestone-1 work remains in `docs/IMPLEMENTATION_PLAN.md`.

Immediate next milestone after successful completion is:

**Milestone 2 — Offline Life OS + sync contract.**

---

## Next step

Create a dedicated Milestone 1 branch and scaffold the .NET 10 headless projects and tests.
