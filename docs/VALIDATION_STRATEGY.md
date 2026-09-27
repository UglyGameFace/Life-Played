# Life Played — Validation & Test Strategy

**Status:** Authoritative validation foundation  
**Depends on:** All prior foundation docs

---

# 1. Validation principle

Life Played is not complete because code compiles, one test passes, or one device looks good.

Completion requires evidence appropriate to the change.

Validation must cover:

- domain correctness
- economy correctness
- persistence/sync
- Unity runtime
- Android
- iOS
- backend/API
- content
- commerce
- privacy/security
- accessibility
- performance/thermal
- release integrity

---

# 2. Test pyramid

## Layer 1 — Pure domain/unit tests

Fastest, cheapest, highest frequency.

Targets:

- XP curves
- effort scoring
- duplicate diminishing returns
- Momentum
- Campaign/Quest state transitions
- recurrence
- reward invariants
- Need confidence/ranking primitives
- lifecycle state machines
- content rule validation
- idempotency helpers

These tests should not require Unity.

---

# 3. Application/use-case tests

Targets:

- create/edit/complete Action
- Campaign transitions
- reward orchestration
- Chronicle generation
- world-update triggers
- sync mutation creation
- AI suggestion approval flow
- Need creation/satisfaction
- entitlement application

Use fakes/stubs for external services.

---

# 4. Database integration tests

Targets:

- migrations
- constraints
- transaction behavior
- outbox
- idempotent RewardGrant
- concurrency/version checks
- ledger consistency
- deletion workflow
- indexes/query behavior

Run against real PostgreSQL-compatible test database.

---

# 5. API contract tests

Validate:

- auth requirements
- request/response schema
- validation
- pagination
- idempotency
- error envelope
- sync cursor behavior
- compatibility

Contracts should be machine-verifiable.

---

# 6. Sync tests

Must cover:

- offline create
- offline edit
- offline completion
- app restart before sync
- duplicate retry
- mutation out-of-order
- two-device conflict
- deleted/archived entity sync
- stale cursor
- server rejection
- partial batch failure
- reconnect after long offline period

No duplicate authoritative reward.

---

# 7. Economy invariant tests

Required invariants:

- duplicate spam yields diminishing returns
- scheduled Habit occurrences remain valid
- task decomposition does not linearly multiply milestone reward
- efficiency improvement is not punished solely by lower actual duration
- Momentum missed day does not reset progress
- RestPeriod is neutral where intended
- focus reward tapers after healthy window
- Sponsored Quest cannot exceed comparable progression cap
- purchase price does not scale account XP linearly
- Companion Bond cannot be purchased
- world upgrade cannot be bought with currency alone where real progress gate is required

---

# 8. Property/fuzz tests

Use property-based/fuzz testing where valuable.

Candidates:

- reward formulas across extreme durations
- recurrence generation
- idempotency keys
- state machine transitions
- pagination cursors
- offer ranking inputs
- malformed provider payloads

Goals:

- no negative balances
- no NaN/overflow
- no impossible lifecycle transition
- no duplicate reward

---

# 9. Content validation tests

Before content publication:

- all referenced IDs exist
- no impossible prerequisite cycles
- every required branch can complete
- reward references valid
- V1 content does not require post-V1 feature
- localization keys present
- client compatibility valid
- story completion path exists
- Sponsored content not required for canon
- Archive references valid

Content validation failure blocks publication.

---

# 10. Story acceptance testing

For each Founding World:

- start-to-finish Saga playable
- Chapter unlock sequence correct
- choices persist
- Chronicle entries correct
- world transformations persist
- returning player recap coherent
- companion story does not block main Saga
- Saga ending feels complete
- post-Saga state remains playable

Narrative QA includes human review, not only schema tests.

---

# 11. Unity EditMode tests

Targets:

- serialization
- content adapters
- ScriptableObject/config mapping
- Addressable labels/references
- pure Unity glue
- editor validation

Avoid testing core economy exclusively through Unity.

---

# 12. Unity PlayMode tests

Targets:

- Bootstrap
- scene load/unload
- navigation
- character spawning
- companion follow/reaction
- world upgrade presentation
- Chronicle UI
- offline state restore
- content release loading
- graphics tier switch

Keep automated PlayMode suite targeted to avoid unnecessary cloud time.

---

# 13. Android device validation

Representative matrix should include:

- current flagship
- midrange
- older supported device class
- varied aspect ratios
- Android versions across support floor/current

Test:

- install/update
- cold/warm launch
- suspend/resume
- process kill/restart
- local storage
- notifications
- deep links
- network loss/reconnect
- Play Billing when added
- Play Integrity when added
- graphics tiers
- thermal/battery
- safe areas/input

---

# 14. iOS device validation

Representative matrix:

- current iPhone generation
- older supported iPhone
- varied screen sizes
- current/support-floor iOS

Test:

- install/update
- cold/warm launch
- background/foreground
- process termination
- local persistence
- notifications
- deep/universal links
- StoreKit
- App Attest when added
- graphics tiers
- thermal/battery
- safe areas

Simulator is not sufficient for final native validation.

---

# 15. Lifecycle testing

Critical mobile cases:

- app backgrounded during unsynced mutation
- app killed during sync
- OS kills process
- device reboot
- time zone change
- daylight-saving transition
- no network
- captive portal
- slow network
- API timeout
- expired auth session
- content update mid-session

---

# 16. Performance testing

Measure:

- frame rate/frame time
- memory
- scene load time
- startup time
- GC spikes
- draw calls
- texture memory
- particle load
- battery drain
- thermal throttling

Performance budgets are established from first vertical slice and enforced thereafter.

---

# 17. Graphics-tier validation

Every major world/VFX feature is checked in:

- Reduced
- Standard
- High
- Cinematic if supported

Acceptance:

- feature remains understandable
- no required gameplay cue disappears
- Reduced mode still looks intentional
- High/Cinematic does not destabilize device

---

# 18. Long-session thermal tests

Run sustained:

- 15 min
- 30 min
- 60 min

Measure:

- frame stability
- thermal throttling
- battery rate
- memory growth
- crash/leak

A beautiful first five minutes followed by thermal collapse is a failure.

---

# 19. Accessibility validation

Required:

- text scaling
- reduced motion
- VFX reduction
- color-independent states
- captions where audio carries meaning
- touch target size
- screen-reader labels for utility UI where platform/tooling supports
- non-combat route
- companion-free/minimal presentation

Manual accessibility review supplements automated checks.

---

# 20. Backend load testing

Before public launch:

- auth/session path
- sync path
- Action completion/reward transaction
- content fetch
- AI request controls
- Deal Scout offer search

Measure:

- latency
- DB pool
- CPU/memory
- queue backlog
- error rate

Load tests must not hit real affiliate/AI providers without controlled limits.

---

# 21. Viral-spike test

Simulate burst load sufficient to prove:

- API scales/hits limits safely
- DB connection pool remains bounded
- queues absorb optional work
- AI spend guard engages
- optional features degrade
- core Life OS remains available

Exact target based on launch plan.

---

# 22. Provider adapter tests

For each provider:

- valid response
- malformed response
- timeout
- 429
- 5xx
- auth failure
- duplicate webhook
- out-of-order webhook
- provider outage
- expired offer
- refund/reversal where applicable

Provider failure must not corrupt normalized core state.

---

# 23. Affiliate attribution tests

Cover:

- outbound click ID uniqueness
- deep-link generation
- conversion match
- duplicate conversion
- reversed commission
- no-match conversion
- user Already Bought
- expired Need/Offer

Developer revenue state remains separate from game reward.

---

# 24. Future location validation matrix

Before location rewards launch:

- precise allowed
- approximate only
- denied
- disabled system location
- stale point
- weak accuracy
- mall/shared building
- drive-by
- curbside
- gas station
- short visit
- offline visit/backfill
- mock/simulated signal
- impossible travel
- app killed/backgrounded

Honest fallback path is tested.

---

# 25. Future purchase/reward matrix

Before cashback:

- authorization only
- settlement
- delayed settlement
- full refund
- partial refund
- chargeback
- duplicate transaction
- shared/reused receipt
- webhook missed then reconciliation
- webhook duplicated
- provider outage
- manual adjustment

Ledger must reconcile exactly.

---

# 26. Security tests

Required:

- authorization ownership checks
- IDOR attempts
- token expiration/revocation
- rate limits
- replay/idempotency
- malformed input
- webhook signature
- secret scanning
- dependency vulnerabilities
- privilege escalation
- admin audit

High-risk real-money features justify stronger independent security review.

---

# 27. Privacy validation

Before release/update:

- data inventory reviewed
- store privacy declarations match code
- permissions match actual feature
- no unnecessary SDK collection
- deletion flow tested
- export path tested where required
- logs sampled for sensitive leakage
- analytics payloads sampled
- AI payload minimization checked

---

# 28. Backup/recovery validation

Required:

- backup succeeds
- restore into isolated environment succeeds
- schema compatible
- object storage recovery path known
- content rollback works
- secret rotation procedure tested/documented

Recovery claims require restore evidence.

---

# 29. Migration validation

Every migration:

- applies cleanly from supported previous state
- preserves data
- has rollback/forward-fix plan
- handles concurrent app version where necessary
- tested against representative data volume

---

# 30. Update compatibility

Mobile app versions may coexist.

Backend/content must:

- reject incompatible requests clearly
- maintain contract compatibility for supported versions
- support forced upgrade only when necessary

Do not break older client blindly after app-store rollout starts.

---

# 31. Release candidate checklist

Before V1 release candidate:

- all required tests pass
- Android build passes
- iOS build passes
- first Saga both Worlds complete
- offline/sync validated
- economy invariants pass
- crash/error reporting active
- privacy/store metadata reviewed
- accessibility pass
- performance/thermal pass
- content release validated
- secrets scan clean
- final diff/repo clean
- known limitations documented

---

# 32. Unity cloud-build quota rule

Validation uses cloud builds deliberately.

Do not run full Unity mobile builds for:

- doc-only changes
- backend-only changes
- pure domain tests

Use cloud Unity/device builds for:

- meaningful runtime integration
- milestone proof
- native/platform changes
- release candidates

Current free-plan quotas are checked before CI automation is enabled.

---

# 33. Validation evidence

Completion report should record:

- commit/SHA
- test suite names
- pass/fail counts
- build IDs
- device/platform versions
- performance results
- known unvalidated areas
- blockers

“No errors seen” is not adequate evidence.

---

# 34. Definition of Done rule

A task may be called complete only when:

- intended behavior implemented
- targeted regression tests pass
- relevant broader tests pass
- build/static/config checks pass
- runtime/edge paths checked
- compatibility reviewed
- temporary/debug code removed
- affected-area duplicate/conflicting logic resolved
- final diff inspected
- limitations/blockers stated

This applies to all future implementation milestones.

