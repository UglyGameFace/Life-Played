# Life Played — Backend Architecture

**Status:** Authoritative backend foundation  
**Depends on:** `docs/MASTER_PRODUCT_BIBLE.md`, `docs/V1_SCOPE.md`, `docs/CORE_DOMAIN_MODEL.md`, `docs/GAME_ECONOMY.md`, `docs/STORY_CONTENT_ARCHITECTURE.md`, `docs/UNITY_MOBILE_ARCHITECTURE.md`

---

# 1. Backend goal

Life Played must be inexpensive and simple enough to launch, while preserving a clean path to large-scale operation without rewriting the product around a new vendor.

The backend is responsible for:

- Authentication/session integration
- Authoritative progression
- Idempotent sync
- Story/world state
- Companion/inventory ownership
- Entitlements
- AI orchestration
- Deal Scout / commerce orchestration
- Reward/evidence decisions
- Content/config delivery
- Notifications
- Analytics events
- Live Ops
- Audit/reconciliation
- Fraud/risk controls

The client may provide intent and local/offline state. Valuable outcomes remain server-authoritative.

---

# 2. Technology direction

The preferred implementation direction is:

- **ASP.NET Core / modern .NET** for API and workers
- **PostgreSQL** as the primary relational database
- **Object storage + CDN** for large/static content
- **Containerized stateless services**
- **Managed infrastructure** initially
- **Provider-neutral interfaces** around auth, AI, commerce, location, messaging, storage, and payments

Why .NET is favored:

- Strong C# alignment with Unity
- Shared contracts and pure domain primitives where safe
- Mature async/networking ecosystem
- High throughput
- Good container support
- Strong testing/type safety
- Reduces unnecessary language/context switching

Final framework/runtime versions are selected at implementation time from current supported/LTS releases.

---

# 3. Shared-code rule

The client and server may share:

- DTO/contracts
- IDs/value-object formats
- deterministic preview helpers
- validation primitives
- content schemas

The client must **not** receive secrets, fraud rules, authoritative reward controls, merchant credentials, settlement logic, or any server-only enforcement logic.

Shared code never makes the client authoritative.

---

# 4. Initial deployment shape

Start with a small number of deployable units:

```
Internet
  ↓
API / BFF
  ├── PostgreSQL
  ├── Object Storage/CDN
  ├── AI Provider Adapter
  ├── Affiliate/Commerce Adapters
  └── Push Provider Adapter

Worker
  ├── Outbox processing
  ├── notifications
  ├── reconciliation
  ├── scheduled jobs
  ├── AI async work
  └── analytics/export jobs
```

Do **not** begin with dozens of microservices.

Logical boundaries exist in code first.

Split deployables only when scale, isolation, compliance, ownership, or reliability provides a concrete reason.

---

# 5. Modular monolith first

V1 backend should be a **modular monolith** with strict module boundaries.

Recommended modules:

- Identity
- LifeOS
- Progression
- World
- Story
- Chronicle
- Companions
- Commerce
- Rewards
- Entitlements
- Content
- Sync
- Notifications
- AI Planning
- Risk/Integrity

Benefits:

- Cheap to run
- Easier transactions
- Easier debugging
- Less operational complexity
- Clear future extraction boundaries

The architecture must avoid cross-module table manipulation outside defined application/domain interfaces.

---

# 6. API style

Primary mobile API:

- HTTPS
- JSON or another stable mobile-friendly contract
- versioned route/contracts where compatibility requires
- request IDs
- idempotency support
- pagination
- consistent error envelope

REST is the default direction for V1.

GraphQL is not required to prove the product and should not be introduced without a concrete need.

---

# 7. Mobile BFF boundary

The public mobile API behaves as a Backend-for-Frontend boundary.

The Unity client should not directly call:

- PostgreSQL
- Supabase table APIs
- Redis
- affiliate providers
- AI providers
- card-linked providers
- location vendors
- object-storage admin APIs

The BFF protects vendor portability and server authority.

---

# 8. PostgreSQL strategy

PostgreSQL is the primary source of truth for structured application state.

Use relational tables for:

- accounts
- Life OS
- progression
- world/story state
- inventory/companions
- Chronicle
- commerce normalization
- reward/evidence state
- entitlements
- sync metadata

Use JSON/JSONB only for:

- bounded provider metadata
- versioned configuration fragments
- event details where schema flexibility is justified

Do not store the entire player as a giant JSON document.

---

# 9. Schema ownership

Each logical module owns its tables.

Examples:

- `life_os.*`
- `progression.*`
- `world.*`
- `story.*`
- `commerce.*`
- `rewards.*`

Physical schema naming can be finalized later.

Migrations must be version-controlled.

No production schema edits performed manually without migration history.

---

# 10. IDs

Use globally unique opaque IDs suitable for offline-originated entities.

Preferred family:

- UUIDv7 or equivalent sortable globally unique identifiers

Benefits:

- offline creation
- low collision risk
- time locality/index behavior
- no leaking sequential user counts

Externally visible IDs must not expose database sequence assumptions.

---

# 11. Concurrency/versioning

Mutable synchronized entities should carry:

- version number / concurrency token
- updated timestamp
- server canonical version

Update requests include expected base version where conflict matters.

Simple entities may use deterministic last-write policy only when data-loss risk is acceptable.

---

# 12. Idempotency

Every mutation capable of producing progression, money, entitlement, inventory, or external side effect requires an idempotency key.

Examples:

- complete Action
- grant reward
- process affiliate conversion
- process store purchase
- process webhook
- send payout
- issue entitlement

Repeated delivery returns prior outcome rather than repeating the side effect.

---

# 13. Transactional outbox

Use the **transactional outbox pattern** for reliable background side effects.

Example:

```
DB transaction:
  Action completion
  RewardGrant
  ProgressionEvent
  OutboxEvent
COMMIT
        ↓
worker publishes/processes OutboxEvent
        ↓
notification / analytics / async work
```

This prevents:

- progression committed but notification/worker event lost
- external event emitted but DB transaction rolled back

---

# 14. Event design

Not every state change requires full event sourcing.

Use append-only events where history matters:

- ProgressionEvent
- RewardGrant/ledger
- CurrencyLedgerEntry
- Entitlement changes
- EvidenceDecision
- RewardSettlementEvent
- audit/security events

Ordinary editable task text does not need to become a fully event-sourced aggregate.

---

# 15. Queue strategy

V1 can begin with:

- database-backed outbox/worker queue

Move to dedicated queue infrastructure when load/latency requires it.

Possible future queue providers are implementation choices.

Queue workloads:

- push notifications
- AI planning
- affiliate reconciliation
- content publication jobs
- email if added
- analytics export
- receipt processing post-V1
- location/reward reconciliation post-V1

Critical Action completion should not wait synchronously for noncritical workers.

---

# 16. Caching

Do not add Redis merely because scalable diagrams traditionally contain a red box.

Initial cache candidates:

- content manifests
- feature flags
- frequently read static definitions
- offer catalogs
- public aggregate data

In-process cache may be enough initially.

Introduce distributed cache when multi-instance consistency/performance requires it.

Authoritative progression never exists only in cache.

---

# 17. Connection pooling

Database access must use bounded connection pooling.

The backend must survive horizontal API scaling without each instance opening unlimited database connections.

At larger scale:

- pooler/proxy
- read replicas
- query optimization
- partitioning

are introduced based on evidence.

---

# 18. Query discipline

Required from V1:

- explicit indexes for synchronization/query paths
- bounded pagination
- no unbounded list endpoints
- query timeouts
- slow-query monitoring
- explain/analyze before large query changes
- N+1 prevention

A viral launch usually kills databases through boring query mistakes before exotic distributed-systems problems arrive.

---

# 19. Sync API

Sync supports:

- client mutation batch
- idempotent mutation processing
- canonical server results
- incremental changed-record feed
- sync cursor
- tombstones where needed
- partial retry

Conceptual call:

`POST /sync`

Payload:

- device ID
- prior cursor
- mutation batch

Response:

- per-mutation result
- authoritative changes
- next cursor
- content/config hints

Exact contract defined later.

---

# 20. Reward processing transaction

Ordinary authoritative Action completion should conceptually execute:

1. Verify account/entity state.
2. Check idempotency.
3. Validate completion transition.
4. Evaluate versioned reward rule.
5. Insert ProgressionEvent.
6. Insert RewardGrant.
7. Update cached progression totals.
8. Update related Campaign/Quest state if applicable.
9. Insert outbox events.
10. Commit atomically.
11. Return canonical reward.

External notification/analytics waits for workers, not the user's button.

---

# 21. Story/world processing

Story/world progression should be driven by explicit triggers/events.

Examples:

- Campaign phase completed
- Life Skill level reached
- Chapter condition satisfied
- Saga completed

A story evaluator may emit:

- chapter unlock
- world structure update
- Chronicle entry
- companion beat

All economically meaningful results remain idempotent.

---

# 22. Content service

Content is separate from player state.

Content service responsibilities:

- active ContentRelease
- balance configuration
- Saga/Chapter definitions
- dialogue
- feature flags
- campaign archetype framing
- client compatibility constraints

Clients cache a last-known-good release.

The backend rejects incompatible content publication before activation.

---

# 23. Content signing/integrity

Remote content manifests should support integrity validation.

At minimum:

- version
- hash
- compatibility version
- publication timestamp

For high-trust remote bundles/config, use signed manifests where appropriate.

The client should never execute arbitrary remote code.

---

# 24. Authentication

Use a standards-based identity system/provider.

Backend stores internal `account_id` independent of provider subject IDs.

This allows:

- provider migration
- multiple login methods
- account linking

Server endpoints authorize by internal account identity/claims, never client-supplied account ID alone.

---

# 25. Authorization

Authorization rules include:

- account owns private resource
- moderator/admin/service role where explicitly supported
- merchant portal post-V1 uses separate permissions
- support tools use auditable privileged actions

Do not trust UI visibility as authorization.

---

# 26. Entitlements

App-store receipts/transactions are verified server-side.

Entitlement flow:

```
Store transaction
  ↓
server verification
  ↓
PurchaseRecord
  ↓
Entitlement mutation
  ↓
client sync
```

Store webhooks/server notifications are reconciled with existing state.

The client cache may unlock provisionally only under explicitly safe rules; canonical access comes from Entitlement.

---

# 27. AI orchestration

AI access routes through backend.

Never expose provider API keys in Unity.

Responsibilities:

- model/provider selection
- prompt templates/versioning
- context minimization
- rate limit
- cost quota
- retries/timeouts
- safety controls
- structured output validation
- fallback

AI responses are suggestions, not authoritative rewards.

---

# 28. AI cost control

Track:

- calls/account/day
- tokens/cost
- failure rate
- latency
- feature
- subscription tier where applicable

Controls:

- per-account quota
- global spend guard
- cached/reusable deterministic results where sensible
- cheaper model for low-value classification
- premium allowance for expensive planning

If AI provider fails, core Life OS remains operational.

---

# 29. Commerce adapters

Each external provider implements normalized interfaces.

Examples:

- `IOfferProvider`
- `IAffiliateAttributionProvider`
- `ILocationProvider`
- `IPurchaseEvidenceProvider`
- `ICashbackProvider`

Core Commerce entities do not expose provider-specific schemas.

Provider raw IDs map through adapter tables.

---

# 30. Webhook ingress

External webhooks terminate at dedicated validated endpoints.

Required:

- signature verification
- timestamp/replay validation where provider supports it
- raw event ID deduplication
- provider allowlist/config
- safe parsing
- async processing

Webhook HTTP response should not wait for every downstream action.

---

# 31. Reconciliation

Never assume webhooks are perfect.

Scheduled reconciliation compares:

- expected conversions
- provider transactions
- settlements
- refunds
- entitlements
- reward state

Missing or duplicated events are repaired idempotently.

This is mandatory before real cashback/payout systems ship.

---

# 32. Risk/integrity module

Risk module consumes normalized evidence.

Examples:

- device/app integrity
- event velocity
- location confidence
- purchase verification
- duplicate receipt/transaction
- account/device graph signals

It returns reasoned decisions/flags.

Ordinary gameplay should not synchronously call heavy fraud vendors unless necessary.

---

# 33. Object storage

Use object storage/CDN for:

- content bundles
- remote 3D assets
- images
- downloadable story/media assets
- support uploads where needed

Do not store large binary assets in PostgreSQL rows.

Public assets use CDN/cache headers.

Sensitive evidence uploads use private storage with short-lived access.

---

# 34. Media processing

Post-V1 receipt/media ingestion runs asynchronously.

Flow:

`upload → private object storage → job → normalized evidence → retention cleanup`

The API should not block while performing expensive image processing.

---

# 35. Notifications

Backend generates notification intent.

Worker dispatches through platform/provider adapters.

Store notification records sufficient for:

- dedupe
- delivery attempt
- user preference checks
- campaign tracking where appropriate

Do not spam a player because a retry worker forgot it already sent the message.

---

# 36. Scheduled jobs

Examples:

- recurrence generation
- Momentum recalculation/checkpoints
- reminder scheduling
- stale Need expiration
- content activation
- provider reconciliation
- offer expiration
- cleanup/retention

Jobs require:

- idempotency
- locking/lease
- retry policy
- observability

---

# 37. Time

Store authoritative instants in UTC.

Store user time zone separately.

Planning/recurrence interprets local calendar rules using user's current configured time zone.

Financial/offer deadlines use authoritative server/provider time.

---

# 38. Observability

Required:

- structured logs
- request IDs/correlation IDs
- error tracking
- metrics
- traces where useful
- health endpoints
- queue depth
- DB pool saturation
- slow query metrics
- AI cost/latency
- sync failures
- reward error rate

Logs must avoid secrets and unnecessary sensitive data.

---

# 39. Product metrics

Privacy-conscious product metrics may include:

- onboarding funnel
- first Action completion
- first world transformation
- retention
- Campaign completion
- companion engagement
- sync health
- Deal Scout engagement
- offer click/conversion
- AI usage

Private task text is not required for most analytics.

---

# 40. Audit logging

Security/financial/admin changes require an audit trail.

Examples:

- manual reward adjustment
- entitlement correction
- support account action
- merchant campaign changes post-V1
- fraud restriction

Audit records identify actor, action, target, timestamp, and reason.

---

# 41. Secrets

Secrets live in managed secret/environment systems.

Never commit:

- API keys
- signing keys
- database passwords
- affiliate secrets
- AI credentials
- store credentials

Different environments use different credentials.

---

# 42. Environments

Minimum:

- local/test
- staging
- production

Production data is not casually copied into development.

Content can be staged before activation.

Database migrations are tested before production.

---

# 43. Migrations

Rules:

- backward-compatible where possible
- expand/migrate/contract for risky large changes
- migrations logged/versioned
- rollback/recovery plan for destructive operations
- no app release assumes schema migration completed unless deployment order guarantees it

---

# 44. Backups

Production PostgreSQL requires:

- automated backups
- point-in-time recovery when available/economically feasible
- periodic restore test
- documented recovery procedure

A backup that has never been restored is a comforting theory, not a recovery plan.

---

# 45. Disaster recovery

Document:

- RPO target
- RTO target
- database restore
- object storage recovery/versioning
- secret rotation
- provider outage behavior
- content rollback

Exact V1 targets are set during deployment planning.

---

# 46. Provider outage behavior

Examples:

## AI provider down

- Life OS works
- AI button reports temporary unavailability
- no task data loss

## affiliate provider down

- cached offers may remain with freshness warning
- no false “verified” deal
- core game works

## notification provider down

- in-app state remains correct
- worker retries within bounds

## content CDN down

- last-known-good content remains usable

No third-party outage should take down unrelated core gameplay.

---

# 47. Rate limiting

Apply rate limits to:

- authentication abuse
- AI calls
- expensive search
- offer refresh
- evidence upload
- suspicious mutation volume

Rate limiting should not make ordinary offline mutation replay fail after reconnect.

Sync endpoints need batch-aware limits.

---

# 48. Scaling path

## Stage 1 — Launch

- few stateless API instances
- worker
- managed Postgres
- object storage/CDN
- no dedicated Redis unless justified

## Stage 2 — Growth

- horizontal API scaling
- connection pooler
- dedicated queue if outbox polling becomes limiting
- Redis/cache if justified
- read replicas for heavy reads
- isolated workers

## Stage 3 — Large scale

Potentially:

- extracted high-load modules
- regional edge/CDN
- database partitioning
- sharded/partitioned event history
- dedicated analytics pipeline
- multi-region strategy where business requirements justify it

The application contract should not require this complexity at launch.

---

# 49. Viral-growth protections

Before public launch:

- hard AI spend cap
- request rate limits
- bounded DB pools
- pagination
- queue backpressure
- feature flags
- provider kill switches
- content CDN
- autoscaling where host supports it
- alerting on latency/error/DB saturation

A sudden user spike should degrade optional systems before corrupting authoritative state.

---

# 50. Graceful degradation order

When capacity is constrained, preserve in this order:

1. Authentication/account access
2. Life OS read/write/sync
3. authoritative progression
4. world/story state
5. entitlements
6. content delivery
7. notifications
8. AI
9. Deal Scout refresh
10. nonessential analytics/live extras

Optional sparkle dies before core user data.

---

# 51. Data retention boundaries

Exact retention policy is defined in privacy/security work.

Architecture must support:

- deletion/anonymization
- evidence-specific retention
- financial/legal retention exceptions
- raw provider payload cleanup
- account deletion workflow

No table design should assume every record lives forever.

---

# 52. Support tooling direction

Support/admin tools eventually need:

- account lookup
- sync health
- reward history
- entitlement history
- evidence/reward state
- safe correction actions

Every privileged action is audited.

Direct production database editing is not the normal support workflow.

---

# 53. Backend testing

Required layers:

- domain unit tests
- application/use-case tests
- database integration tests
- migration tests
- API contract tests
- idempotency/retry tests
- worker tests
- provider-adapter contract tests
- webhook verification tests
- reconciliation tests

External providers are mocked/stubbed in CI, with controlled staging integration tests.

---

# 54. Backend architecture acceptance criteria

Before implementation:

- API is the only mobile gateway to authoritative backend state.
- PostgreSQL is the structured source of truth.
- provider-specific schemas are behind adapters.
- valuable mutations are idempotent.
- progression/reward commits are transactional.
- background side effects use outbox/worker.
- AI is isolated and cost-controlled.
- third-party outages degrade gracefully.
- observability/backups/recovery are explicit.
- architecture starts as a modular monolith, not premature microservices.
- the scaling path does not require changing the client contract.

