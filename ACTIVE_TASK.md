# ACTIVE TASK — Life Played

## Active task / desired outcome

**Task:** Establish the production foundation for Life Played before implementation begins.

**Outcome:** Produce an authoritative, implementation-ready product foundation that defines what Life Played is, what ships first, how the systems interact, and which technical/privacy/commerce constraints the eventual Unity client and backend must satisfy.

This is the **only active implementation/design task** for this repository until its Definition of Done is satisfied.

---

## Project identity

**Product:** Life Played  
**Repository:** `UglyGameFace/Life-Played`  
**Primary platforms:** Google Play / Android and Apple App Store / iOS  
**Current client direction:** Unity, because Life Played is intended to contain genuine real-time 3D worlds, characters, companions, animation, lighting, weather, VFX, and cinematic progression moments.

**Core product principle:**

> Your life builds the world.

Life Played is not a task manager with decorative RPG mechanics. Real-world goals, habits, projects, focus sessions, errands, achievements, shopping needs, visits, purchases, and milestones can drive persistent game progression.

---

## Status

**State:** ACTIVE — product foundation / Step 11: implementation plan & final consistency review

**Code implementation:** Not started intentionally.

**Why:** The product has grown beyond a simple gamified task app. Core gameplay, world identity, long-running stories, location/reward validation, commerce intelligence, privacy, offline behavior, and server-authoritative progression all affect the architecture. These must be reconciled before scaffolding production code.

---

## Current findings / requirements understood

### 1. Core gameplay loop

The intended loop is:

`Real Life → Goals & Actions → Quest Engine → Progression → Character & World Growth → New Stories & Adventures`

Real-world actions must produce meaningful game consequences rather than only increasing a generic XP counter.

### 2. Persistent 3D RPG

Life Played is intended to support:

- A genuine stylized 3D persistent world
- 3D player characters
- 3D companions/pets with animation and visual effects
- Upgradeable buildings and world regions
- NPCs, factions, bosses, exploration, collectibles, equipment, cosmetics, and landmarks
- Dynamic lighting, day/night, weather, ambient animation, and premium VFX for important moments
- Scalable visual quality for lower-end through high-end mobile devices

Default visual philosophy:

**Cozy while living. Cinematic while achieving.**

### 3. Personalized identity worlds

The player is not forced into one aesthetic.

Initial identity directions include:

- Cozy / Nature
- Fantasy / Adventure
- Coder / Hacker / Tech
- Science Fiction
- Dark / Arcane
- Modern / Minimal

Each identity must eventually have its **own complete story, world, characters, locations, factions, companions, terminology, visual language, and narrative**, rather than being a shallow reskin.

The systems underneath may be shared.

### 4. Story architecture

Stories may end; the world must not.

Long-term content structure is intended to include:

- Main sagas
- Regional campaigns
- Faction stories
- Companion stories
- Player-generated personal campaigns
- Seasonal stories
- Archived past seasonal narratives
- Live world events
- New regions/worlds
- Legacy/prestige progression without deleting prior accomplishments

A player finishing one authored story must still have meaningful progression and future content.

### 5. Real-life productivity system

The utility side must stand on its own and include a serious planning/productivity foundation, potentially including:

- Tasks
- Habits
- Projects
- Recurring actions
- Campaigns
- Deadlines
- Subtasks/dependencies
- Focus sessions
- Next Best Action
- Calendar integration
- Notifications
- Recovery/rescheduling
- Momentum instead of destructive streak punishment
- Planned rest days

### 6. AI Quest Master

AI may help:

- Break goals into actionable quests
- Suggest scheduling/restructuring
- Adapt to how the player actually works
- Create personalized story presentation
- Summarize campaigns/progress

AI must **not** directly control XP, cash, inventory, reward value, purchase qualification, or other authoritative economy decisions.

### 7. Progression

Current intended progression layers include:

- Account level
- Life Skills
- Momentum
- Persistent world development
- Buildings/regions
- Character equipment/cosmetics
- Companions and bonding/evolution
- Achievements
- Chronicle entries
- Faction reputation
- Campaign completion
- Legacy progression

Large purchases must never simply translate into proportionally massive XP advantages.

### 8. Chronicle / permanent memory

Major real-world accomplishments should create persistent memories such as:

- Chronicle entries
- Trophies
- World landmarks
- Building changes
- Decorations
- Companion events/evolutions
- Campaign records

The world should become a visual history of the player's real progress.

### 9. Companions

Companions are intended to be real 3D entities, not static inventory icons.

Depending on player preference they may include:

- Animals
- Fantasy creatures
- Dragons
- Spirits
- Drones
- AI constructs
- Mechanical companions
- Cyber creatures
- Minimal/non-creature constructs

Users who dislike fuzzy/cute characters must be able to choose completely different companion and world styles, including no companion if desired.

### 10. Real-world Adventure / Nearby mode

Life Played may contain an optional location-aware real-world adventure layer inspired by the broad category of location-based games, while using only original Life Played branding, characters, worlds, systems, and content.

Potential experiences include:

- Nearby real-world quests
- Merchant quests
- Landmarks
- Discoveries
- Limited-time events
- Sponsored locations
- Visit-based rewards
- Purchase-based rewards

### 11. Commerce Intelligence

Commerce must serve an existing or strongly indicated player need rather than manufacture unnecessary wants.

Signals may include, when appropriate and consented:

- Explicit shopping lists
- Active quests/campaigns
- Saved items
- Purchase history
- Preferred merchants/brands
- User-entered budgets
- Offer interactions
- Commercial locations visited
- Recurring purchase patterns
- Explicit user feedback

Sensitive inferences from locations or purchase activity must be avoided.

### 12. Deal Scout

The intended commerce layer can search or aggregate:

- Sales
- Coupons
- Promo codes
- Cashback
- Affiliate offers
- Price drops
- Merchant-funded rewards
- Sponsored Quests

The engine should compare **effective player cost**, not merely headline sale price.

It may sometimes recommend waiting rather than purchasing when a deal is poor.

Player value/relevance must come before commission size when offers materially differ.

### 13. Affiliate / merchant strategy

Potential future integrations discussed include provider classes such as:

- Affiliate networks
- Offer/coupon feeds
- Cashback networks
- Card-linked transaction providers
- Item-level purchase verification providers
- Location/place providers
- Direct merchants

Providers must sit behind replaceable adapters.

Long-term strategy may progress from affiliate publisher → high-intent commerce channel → direct merchant Sponsored Quest platform.

### 14. Location and reward reliability

Location must be treated as evidence, not absolute proof.

The design must account for:

- Android geofence delay
- iOS background limitations
- Approximate location
- Poor indoor GPS
- Doze / battery saver
- App termination
- Offline visits
- Device reboot/reinstall
- Dense malls/shared buildings
- Drive-by geofence triggers
- Curbside pickup
- Drive-through
- Delivery
- Short legitimate visits
- Late/backfilled provider events

Missing one location event must not automatically deny a legitimate player.

### 15. Fraud / anti-exploitation

Higher-value rewards require stronger evidence.

Potential evidence includes:

- App/device integrity
- Play Integrity
- Apple App Attest / DeviceCheck
- Mock/simulated location signals
- Location accuracy/freshness
- Plausible motion
- Dwell/venue match
- Purchase authorization
- Purchase settlement
- Loyalty data
- Receipt validation
- Merchant QR/NFC/BLE proof
- Server-issued challenges/nonces
- Duplicate detection
- Account/device risk history

The client must never be authoritative for valuable rewards.

### 16. Reward lifecycle

Financial and merchant-funded rewards require proper state instead of a single paid boolean.

Expected conceptual lifecycle:

`AVAILABLE → ACTIVATED → VISIT_CANDIDATE → VERIFIED/PENDING → PURCHASE_VERIFIED → REWARD_PENDING → REWARD_FINAL → REVERSED if required`

Game rewards may be granted provisionally sooner when confidence is sufficient, while real-money cashback may remain pending until provider settlement/return conditions are satisfied.

### 17. Offline-first behavior

Core app/game functionality should remain useful without a reliable network connection.

Offline actions require:

- Client-generated event IDs
- Local durable queueing
- Later synchronization
- Server-side idempotency
- Conflict/reconciliation rules

No duplicate rewards from retries or repeated taps.

### 18. Server-authoritative valuable state

The server must ultimately own/validate:

- XP
- Skill progression
- Valuable currency
- Inventory grants
- Premium entitlements
- Merchant reward status
- Cashback state
- Competitive/shared progression where applicable
- Purchase qualification
- Fraud decisions

### 19. Privacy / trust

Core gameplay must not require continuous location access.

Location and commerce permissions should be feature-specific and understandable.

The product should avoid turning sensitive location/purchase information into invasive profiling.

Players should be able to understand why offers are shown and control recommendation categories/preferences.

### 20. Monetization direction

Potential revenue includes:

- Affiliate commissions
- Merchant-funded cashback margins
- Sponsored Quests
- Sponsored real-world adventures
- Direct merchant campaigns
- Premium subscription/advanced planning features
- Cosmetics
- Companion/world customization
- Premium story/expansion content where appropriate

No pay-to-win core progression.

---

## Execution path for this active task

The product foundation must be completed in this order before production implementation begins:

1. **Master Product Bible**
   - consolidate all agreed product systems and non-negotiable principles
   - define terminology and authoritative concepts

2. **V1 scope boundary**
   - define what is required for the first public Play Store/App Store release
   - explicitly defer systems that are not needed for V1

3. **Core domain model**
   - users/accounts
   - profiles
   - quests/actions/habits/campaigns
   - progression
   - characters/companions/worlds
   - Chronicle
   - offers/needs/deals
   - evidence/rewards/ledger
   - entitlements

4. **Game economy & progression specification**
   - reward calculations
   - anti-farming/diminishing returns
   - Life Skills
   - Momentum
   - currencies/resources
   - item/companion/world progression boundaries

5. **Story/content architecture**
   - identity-world structure
   - saga/chapter/mission model
   - faction/NPC/companion story structure
   - seasonal/archive/live-ops model

6. **Mobile architecture**
   - Unity project structure
   - mobile UI/game-scene boundaries
   - local persistence
   - sync
   - graphics quality tiers
   - platform integrations

7. **Backend architecture**
   - API boundaries
   - authoritative game services
   - queues/workers
   - relational database model
   - event/ledger model
   - provider-adapter boundaries
   - observability/backups

8. **Commerce/rewards architecture**
   - Need Graph
   - Deal Scout
   - offer ranking
   - affiliate attribution
   - location evidence
   - transaction/receipt verification
   - reward lifecycle
   - reconciliation/refunds

9. **Privacy/security/fraud model**
   - consent boundaries
   - sensitive-data exclusions
   - integrity/attestation
   - abuse controls
   - account/device risk
   - retention/minimization rules

10. **Validation plan**
    - unit/integration tests
    - offline/sync failure testing
    - duplicate/retry tests
    - reward/fraud edge cases
    - Android/iOS location failure paths
    - provider outage/reconciliation
    - store-build/CI requirements

11. **Implementation plan**
    - milestone sequence
    - repository structure
    - CI/CD
    - first vertical slice
    - Definition of Done for each milestone

---

## Definition of Done

This active task is complete only when:

- [x] The Master Product Bible exists in-repo and reflects the agreed product.
- [x] V1 scope and explicit post-V1 backlog are documented.
- [x] Core gameplay loop and progression rules are defined well enough to implement without inventing behavior ad hoc.
- [x] Identity-world/story architecture is documented.
- [x] Commerce, Deal Scout, location/reward, and fraud boundaries are documented.
- [x] Privacy/consent principles are documented.
- [ ] Offline/sync and server-authority rules are documented.
- [x] Unity/mobile architecture is documented.
- [x] Backend/service/domain architecture is documented.
- [x] Initial database/domain model is documented.
- [x] Validation/test strategy is documented.
- [ ] Initial implementation milestones and first vertical slice are defined.
- [ ] The final foundation docs are checked for contradictions, duplicate/conflicting systems, stale assumptions, or accidental scope gaps.
- [ ] No temporary design notes remain as competing sources of truth.

Only after this Definition of Done is satisfied should production implementation become the next active task.

---

## Changes made for this task

- Repository inspected.
- Existing README reviewed.
- `ACTIVE_TASK.md` established as the active-task source of truth.
- `docs/MASTER_PRODUCT_BIBLE.md` created and verified as the authoritative product definition.
- `docs/V1_SCOPE.md` created and verified as the authoritative V1 boundary and post-V1 backlog.
- `docs/CORE_DOMAIN_MODEL.md` created and verified with entity, relationship, lifecycle, sensitivity, offline, and client/server authority boundaries.
- `docs/GAME_ECONOMY.md` created and verified with versioned XP curves, task valuation, anti-farming, Momentum, currency, world progression, companion Bond, and pay-to-win guardrails.
- `docs/STORY_CONTENT_ARCHITECTURE.md` created and verified with complete V1 Saga spines for Wild Renewal and Gridfall plus reusable NPC, companion, seasonal, Archive, and player-Campaign content rules.
- `docs/UNITY_MOBILE_ARCHITECTURE.md` created and verified with URP/mobile layering, offline local persistence, sync boundaries, Addressables direction, graphics tiers, Android/iOS platform isolation, and free-tier milestone build policy.
- `docs/BACKEND_ARCHITECTURE.md` created and verified with modular-monolith boundaries, PostgreSQL, BFF/API isolation, idempotency/outbox, workers, provider adapters, observability, recovery, and growth scaling.
- `docs/COMMERCE_REWARDS_ARCHITECTURE.md` created and verified with Need Graph, Deal Scout, affiliate attribution, offer freshness, Sponsored Quest, future visit/purchase evidence, cashback settlement/reversal, and reconciliation boundaries.
- `docs/PRIVACY_SECURITY_FRAUD.md` created and verified with consent, data classification/minimization, current location-policy direction, attestation, spoof/replay controls, false-positive protection, restrictions, retention, deletion, and future cash-reward launch gates.
- `docs/VALIDATION_STRATEGY.md` created and verified with domain/API/sync/economy/content/Unity/device/performance/security/privacy/recovery validation and completion evidence requirements.
- No production code has been added yet.

---

## Validation / results

Current repository baseline contains:

- `.gitignore`
- `README.md`
- `ACTIVE_TASK.md` (this file)
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

README aligns with the high-level product direction. Product, V1, domain, economy, story/content, Unity/mobile, backend, commerce/reward, privacy/security/fraud, and validation specifications are verified readable on `main`; validation now requires concrete test/build/runtime evidence rather than edit-only completion claims.

No build/test validation applies yet because the Unity project has not been scaffolded.

---

## Cleanup / conflicts

No duplicate implementation exists yet.

No legacy code exists yet.

No conflicting project files were found.

The README is high-level vision material; future authoritative technical/game specifications should live in dedicated foundation documents and remain consistent with it.

---

## Blockers / risks

- **Product naming/legal clearance:** Life Played is the current working name. Preliminary conflict screening was encouraging, but formal trademark/domain/store clearance has not been completed.
- **Bundle IDs:** Must not be permanently chosen until naming clearance is strong enough.
- **Provider contracts/access:** Foursquare/Upside/Fidel/Cardlytics/Banyan/Rakuten/Awin/Impact/CJ or similar providers are candidates/classes, not committed dependencies.
- **Store policy changes:** Location, tracking, commerce, payments, and rewards rules must be re-verified against current Apple/Google requirements before implementing those surfaces.
- **Scope risk:** The full product is large. V1 must prove the real-life → game-world loop without attempting every live-service/commerce feature at launch.
- **Content cost:** Multiple complete identity worlds require shared content tooling and data-driven narrative systems to avoid maintaining multiple separate games.

---

## Backlog — not active

The following are intentionally backlogged until the product-foundation Definition of Done is satisfied:

- Unity project scaffolding
- Android/iOS bundle identifiers
- Backend provider selection
- Authentication implementation
- Quest UI implementation
- 3D world implementation
- Character creator
- Companion system
- AI Quest Master implementation
- Nearby/location mode
- Deal Scout implementation
- Affiliate integrations
- Cashback integrations
- Merchant portal
- Live Ops tooling
- Social/guild systems
- Store submission assets
- Production deployment

Do not begin these as separate tasks while the product-foundation task remains active.

---

## Next step

Define the **implementation plan** and first vertical slice in-repo, then perform a final cross-document contradiction/duplication/staleness review. Do not scaffold production code until that review is complete and the product-foundation Definition of Done is satisfied.
