# Life Played — Core Domain Model

**Status:** Authoritative domain foundation  
**Depends on:** `docs/MASTER_PRODUCT_BIBLE.md`, `docs/V1_SCOPE.md`  
**Purpose:** Define the core entities, ownership boundaries, relationships, lifecycle concepts, and client/server authority required before schemas or production code are created.

---

# 1. Domain design principles

1. **Real-life planning, game progression, story, commerce, and rewards are separate domains.**
2. **The client may originate user intent; the server owns valuable authoritative outcomes.**
3. **Content definitions are separate from player state.**
4. **Financial/reward history uses append-only events/ledgers rather than mutable totals alone.**
5. **Offline mutations are idempotent and carry globally unique event IDs.**
6. **Provider-specific data does not leak into core product entities.**
7. **Sensitive evidence is minimized and separated from general player/profile data.**
8. **No monolithic `user_data` blob is the source of truth.**
9. **Derived counters may be cached, but must be reconstructable from authoritative records where financially/game-economically important.**
10. **Identity Worlds share account-level real-life history but own separate story/world state.**

---

# 2. High-level bounded contexts

Life Played is divided into these conceptual domains:

- Identity & Accounts
- Life OS
- Progression & Economy
- World & Character
- Story & Content
- Chronicle
- AI Planning
- Commerce Intelligence
- Real-World Evidence
- Rewards & Financial State
- Entitlements
- Sync & Device Integrity
- Notifications / Live Ops

These domains may initially live in one backend deployment, but their boundaries must remain explicit.

---

# 3. Identity & Accounts

## 3.1 Account

Represents one Life Played user identity.

Core responsibilities:

- Authentication identity mapping
- Account creation state
- Account lifecycle
- Account-level settings
- Cross-device ownership

Authoritative owner: **server**

Important fields conceptually:

- `account_id`
- account status
- creation timestamp
- locale
- time zone
- onboarding state
- deletion state

The Account is not the game character.

---

## 3.2 Profile

Player-facing identity.

Conceptually includes:

- Display name
- Avatar/profile presentation
- Preference summary
- Accessibility preferences
- Default Identity World
- Notification preferences

Authoritative owner: **server canonical, client-editable**

Sensitive commerce/location information does not belong directly in Profile.

---

## 3.3 DeviceRegistration

Represents an installed client associated with an Account.

Purpose:

- Push token association
- Device/session risk
- Sync cursor state
- Integrity/attestation relationships
- Device revocation

Authoritative owner: **server**

A physical device may be reinstalled; device identity must not be used as the sole permanent player identity.

---

## 3.4 AuthSession

Represents authenticated access.

Purpose:

- Session revocation
- Token rotation/reference
- Device/account association

Actual auth implementation is deferred to architecture/provider selection.

---

## 3.5 ConsentRecord

Records an explicit permission/consent decision where product-level evidence is required.

Examples:

- Commerce personalization
- Location-dependent features
- Optional analytics categories
- Marketing communications

Important concept:

Consent history must be distinguishable from current permission state.

---

# 4. Life OS domain

The Life OS represents what the player is trying to accomplish in real life.

---

## 4.1 Action

Atomic real-world work unit.

Examples:

- Call dentist
- Buy detergent
- Write unit tests
- Walk for 20 minutes

Conceptual fields:

- `action_id`
- `account_id`
- title
- notes
- status
- due time/date
- estimated duration
- completion time
- source
- parent context
- archived timestamp
- local creation/update version

Lifecycle:

`DRAFT → ACTIVE → COMPLETED`

Alternative states:

- PAUSED
- ARCHIVED
- ABANDONED

Completed Actions may be reopened under rules defined later.

Client may create/edit Actions offline.

Canonical authoritative owner: **server after sync**

---

## 4.2 Quest

A meaningful multi-step objective containing Actions and/or subordinate Quest structure where allowed.

Examples:

- Prepare for vacation
- Deep-clean apartment
- Finish onboarding feature

Conceptual fields:

- `quest_id`
- account
- title
- status
- due date
- difficulty metadata
- narrative framing reference
- parent Campaign if any

A Quest completion may produce meaningful progression.

Authoritative owner: **server canonical, user intent originates client**

---

## 4.3 Campaign

Large, phase-based real-life goal.

Examples:

- Launch an app
- Move house
- Train for an event

Conceptual fields:

- `campaign_id`
- account
- title
- description
- status
- start/due date
- phase structure
- world/story presentation reference
- completion timestamp

Lifecycle:

`DRAFT → ACTIVE → PAUSED → COMPLETED`

Alternative terminal state:

- ABANDONED
- ARCHIVED

A completed Campaign may generate:

- ChronicleEntry
- Achievement
- WorldProgress event
- Story trigger
- Companion/bond event
- RewardGrant

---

## 4.4 CampaignPhase

Ordered stage inside a Campaign.

Examples:

- Planning
- Prototype
- Build
- Test
- Release

Contains Quests and/or Actions through explicit relationships.

---

## 4.5 WorkRelation

Explicit relationship between Actions, Quests, Campaigns, or phases.

Supports:

- Contains
- Depends on
- Blocks
- Related to

Avoid hidden dependency rules encoded only in UI.

---

## 4.6 HabitDefinition

Defines a repeatable real-life behavior.

Concepts:

- schedule/cadence
- active period
- completion rules
- reminder configuration
- category/skill mapping hints

A HabitDefinition is not itself a completion.

---

## 4.7 HabitOccurrence

Represents one due occurrence of a habit.

Purpose:

- Completion history
- Missed/skipped/rest distinctions
- Momentum calculation
- Offline-safe completion identity

This prevents one mutable Habit row from trying to represent years of history.

---

## 4.8 RecurrenceRule

Reusable recurrence definition.

Must support at least V1 schedules without embedding platform-specific calendar logic into the Action model.

---

## 4.9 FocusSession

Represents a timed focus effort.

Concepts:

- start
- planned duration
- actual duration
- completion/interruption state
- optional Action/Quest/Campaign association

Authoritative reward qualification is calculated server-side.

---

## 4.10 RestPeriod

Explicit player-declared rest/recovery period.

Purpose:

- Momentum rules
- Reminder suppression
- Recurrence interpretation
- Non-punitive planning

Rest is a positive domain concept, not inferred from inactivity.

---

# 5. Progression & Economy domain

---

## 5.1 AccountProgression

Account-wide progression summary.

Contains derived/cached state such as:

- account level
- total authoritative XP

Underlying XP-changing events must be traceable to RewardLedger/ProgressionEvent history.

Authoritative owner: **server**

---

## 5.2 LifeSkillDefinition

Content/config definition for a Life Skill.

Examples are provisional:

- Focus
- Knowledge
- Fitness
- Creativity
- Craft
- Finance
- Social
- Home

This definition is versioned content, not per-player state.

---

## 5.3 LifeSkillProgress

Per-account state for one skill.

Concepts:

- skill ID
- XP
- level
- last progression timestamp

Authoritative owner: **server**

---

## 5.4 MomentumState

Current derived consistency state.

Contains:

- current value/tier
- calculation version
- last calculated timestamp

Momentum history must be supported by underlying events/occurrences so rule changes can be audited/migrated.

---

## 5.5 ProgressionEvent

Immutable record of a progression-affecting event.

Examples:

- Action completed
- Campaign completed
- Focus session qualified
- Achievement granted

Concepts:

- event ID
- account
- source type/source ID
- rule version
- XP deltas
- skill deltas
- world effects
- timestamp

Server-authoritative.

---

## 5.6 CurrencyDefinition

Defines an earnable/premium/resource currency.

V1 should minimize currency count.

Definition includes:

- economic type
- display behavior
- transferability rules
- whether purchasable
- whether withdrawable

No real-money-equivalent value may be implied accidentally.

---

## 5.7 WalletBalance

Cached balance for a currency.

Must be backed by ledger events for economically important currencies.

---

## 5.8 CurrencyLedgerEntry

Append-only authoritative mutation:

- grant
- spend
- reversal
- correction

Never mutate financially/game-economically meaningful balance without traceability.

---

## 5.9 RewardDefinition

Defines a deterministic reward package or reward rule output shape.

May contain:

- XP
- skill XP
- currency
- items
- cosmetics
- world progression
- companion bond
- Chronicle trigger

Content/config, not a granted reward.

---

## 5.10 RewardGrant

One concrete authoritative reward issued to a player.

Concepts:

- unique grant ID
- source
- rule version
- contents
- state
- idempotency key
- grant/reversal timestamps

Server-authoritative.

---

# 6. Character, Inventory & World domain

---

## 6.1 PlayerCharacter

The player's avatar state.

Concepts:

- character ID
- account
- appearance configuration
- equipped cosmetic/loadout references
- animation/personality selection where supported

Character progression must not duplicate AccountProgression.

---

## 6.2 CosmeticDefinition

Versioned content describing a cosmetic.

Examples:

- outfit
- hair
- accessory
- weapon skin
- companion accessory
- world decoration

---

## 6.3 OwnedCosmetic

Account ownership of a cosmetic.

Authoritative owner: **server**

Acquisition source should be traceable.

---

## 6.4 Loadout

Player-selected equipped presentation.

May be scoped to:

- character
- companion
- Identity World

---

## 6.5 ItemDefinition

Game item/resource definition.

Do not use items for every possible concept.

Items are reserved for things that genuinely behave like inventory.

---

## 6.6 InventoryEntry

Player-owned quantity/state for an item.

Authoritative owner: **server**

For unique items, use unique ownership records rather than quantity semantics where appropriate.

---

## 6.7 IdentityWorldDefinition

Versioned content definition for a world such as Wild Renewal or Gridfall.

Contains references to:

- story content
- world presentation
- building definitions
- companion families
- terminology
- default visual systems

No player-specific state belongs here.

---

## 6.8 PlayerWorldState

Per-account state for one Identity World.

Concepts:

- world ID
- unlock state
- saga/story progress reference
- world level/tier if used
- last active timestamp
- hub state

Server canonical.

---

## 6.9 WorldStructureDefinition

Defines an upgradeable building/area/structure.

Examples:

- sanctuary
- workshop
- archive
- Dev Lab
- command node

---

## 6.10 PlayerWorldStructure

Per-player state for one structure.

Concepts:

- tier/stage
- cosmetic variant
- upgrade progress
- unlock state

World changes caused by real-life achievements should be traceable to progression/story events.

---

## 6.11 WorldLandmark

Persistent player-specific monument/memory object.

May originate from:

- Campaign completion
- major Achievement
- Saga completion
- special Companion event

Links back to Chronicle where appropriate.

---

# 7. Companion domain

---

## 7.1 CompanionDefinition

Versioned content definition.

Contains:

- species/type
- identity-world affinity
- rarity/presentation tier
- model/animation references
- VFX profile
- bond configuration
- evolution possibilities

---

## 7.2 PlayerCompanion

Owned individual companion instance.

Concepts:

- companion instance ID
- account
- definition ID
- acquisition event
- bond state
- chosen name if allowed
- cosmetic state
- evolution state

Server-authoritative ownership.

---

## 7.3 CompanionBondEvent

Immutable bond-affecting event.

Examples:

- Campaign milestone
- Focus milestone
- story event
- interaction

Supports auditability and future rebalance.

---

## 7.4 CompanionEvolution

Represents selected/earned evolution state when applicable.

A companion's evolution should not be represented only by overwriting its original definition.

---

# 8. Story & Content domain

Content definitions and player story state are deliberately separated.

---

## 8.1 ContentRelease

Versioned content bundle.

Purpose:

- Safe publishing
- Rollback
- Environment promotion
- Content compatibility

May include story/config but does not necessarily package binary 3D assets.

---

## 8.2 SagaDefinition

Defines a major authored narrative arc.

---

## 8.3 ChapterDefinition

Ordered story chapter.

References conditions, story nodes, world changes, rewards, and dialogue.

---

## 8.4 StoryNodeDefinition

Atomic data-driven narrative node.

Potential node types:

- dialogue
- objective
- choice
- world event
- reward trigger
- cinematic trigger
- unlock

Exact authoring system is defined later.

---

## 8.5 PlayerStoryState

Per-account/per-world progress through authored content.

Must support:

- current Saga
- completed chapters
- choices/flags
- unlocks

Do not mix this into PlayerWorldState as an unstructured blob.

---

## 8.6 NPCDefinition

Authored NPC content.

---

## 8.7 PlayerNPCState

Optional per-player relationship/state.

V1 may use limited fields while preserving room for deeper post-V1 relationships.

---

## 8.8 FactionDefinition

Post-V1-capable authored faction content.

---

## 8.9 FactionReputation

Per-player/per-world faction state.

May remain unused in V1 but belongs in the long-term domain architecture.

---

# 9. Chronicle domain

---

## 9.1 ChronicleEntry

Permanent player-facing memory record.

Concepts:

- entry ID
- account
- timestamp
- category
- source entity
- title/presentation
- Identity World context
- optional landmark/trophy relationship
- privacy/share setting

Chronicle entries should be generated from meaningful authoritative events, not arbitrary client claims.

---

## 9.2 AchievementDefinition

Versioned achievement criteria/presentation.

---

## 9.3 PlayerAchievement

Authoritative achievement grant.

Links to the event that qualified it.

---

# 10. AI Planning domain

AI suggestions must remain distinguishable from player-approved canonical work.

---

## 10.1 PlanningRequest

Represents a player request for AI planning help.

Examples:

- Break down goal
- Reschedule campaign
- Summarize progress

Contains only the minimum context required.

---

## 10.2 PlanningSuggestion

AI-produced proposal.

State:

`PROPOSED → ACCEPTED / EDITED / DISMISSED`

Only accepted/edited output creates canonical Actions/Quests/Campaigns.

---

## 10.3 NarrativeSuggestion

AI-generated identity-specific flavor or summary.

Must not directly mutate authoritative reward state.

---

# 11. Commerce Intelligence domain

---

## 11.1 Need

Represents something the player actually or probably needs.

Concepts:

- need ID
- account
- normalized category/product intent
- source type
- confidence
- budget if explicitly known
- timing if known
- status
- privacy classification

Lifecycle:

`ACTIVE → SATISFIED / PAUSED / DISMISSED / EXPIRED`

Need is not the same as an ad targeting segment.

---

## 11.2 NeedEvidence

Explains why the Need exists.

Examples:

- explicit shopping list
- Campaign requirement
- saved item
- recurring purchase pattern
- weak behavioral inference

Keeping evidence separate makes `Why am I seeing this?` possible.

---

## 11.3 Merchant

Normalized merchant identity independent of affiliate provider.

---

## 11.4 MerchantLocation

Physical merchant venue/location.

Provider IDs belong in adapter mapping tables, not as the merchant's primary identity.

---

## 11.5 Offer

Normalized commercial offer.

Concepts:

- merchant
- product/category applicability
- start/end
- price/promotion
- coupon terms
- geography
- qualification
- source/provider
- commission metadata
- freshness/validation state

---

## 11.6 OfferMatch

Represents a ranked match between a Need and an Offer.

Contains:

- relevance score components
- player-value estimate
- effective-price estimate
- revenue estimate
- ranking reason
- model/rule version

OfferMatch history supports measurement without changing the underlying Need/Offer.

---

## 11.7 OfferInteraction

Player behavior:

- viewed
- saved
- dismissed
- already bought
- too expensive
- not interested
- activated/clicked

Used to improve recommendations.

---

## 11.8 AffiliateAttribution

Tracks outbound commerce attribution.

Concepts:

- click/activation ID
- account pseudonymous reference as appropriate
- Offer
- provider
- timestamp
- conversion status

Must not expose unnecessary player profile data to merchants/providers.

---

# 12. Real-World Evidence domain

This domain is intentionally separated from general profile/gameplay data.

---

## 12.1 EvidenceCase

Container for evidence supporting a real-world claim.

Examples:

- store visit
- purchase
- sponsored quest qualification

Concepts:

- case ID
- account
- claim type
- risk tier
- state
- decision
- timestamps

---

## 12.2 LocationEvidence

Represents a minimum necessary location observation/result.

Avoid retaining continuous raw trails unless an approved feature genuinely requires them.

Concepts may include:

- observation time
- accuracy
- venue candidate
- dwell summary
- provider
- mock/simulation indicator
- confidence

---

## 12.3 VisitCandidate

Derived possible merchant/place visit.

State examples:

`CANDIDATE → VERIFIED / NEEDS_VALIDATION / REJECTED`

Location alone may be insufficient for high-value rewards.

---

## 12.4 PurchaseEvidence

Provider-neutral purchase evidence.

Possible source:

- card-linked provider
- loyalty provider
- merchant confirmation
- receipt
- affiliate conversion

Provider payload remains stored/minimized according to adapter/security policy.

---

## 12.5 ReceiptEvidence

Structured receipt verification result.

Raw receipt image retention is governed separately and should be minimized.

---

## 12.6 IntegrityEvidence

Device/app integrity result.

Examples:

- Android integrity
- Apple attestation
- replay challenge result

Do not collapse device trust into a permanent binary `trusted_user` flag.

---

## 12.7 EvidenceDecision

Server-side decision with:

- outcome
- rule/model version
- supporting evidence references
- reason codes
- review state

Possible outcomes:

- ACCEPT
- PENDING
- REQUEST_MORE_EVIDENCE
- REJECT

---

# 13. Reward / cashback domain

Post-V1-capable even when V1 does not yet operate a cash wallet.

---

## 13.1 SponsoredQuestDefinition

Merchant-funded quest configuration.

Provider/merchant content separate from player activation state.

---

## 13.2 SponsoredQuestActivation

Player activation/eligibility instance.

---

## 13.3 RealWorldReward

One merchant-funded/cashback reward obligation.

Conceptual lifecycle:

`PENDING → QUALIFIED → FINAL → REVERSED`

May also have:

- EXPIRED
- REJECTED
- MANUAL_REVIEW

---

## 13.4 RewardSettlementEvent

Append-only provider/payment lifecycle event.

Examples:

- authorization observed
- cleared
- cashback confirmed
- partial refund
- full refund
- reversal

---

## 13.5 CashBalance / Payout

Not required V1.

If introduced later, must be separate from ordinary gameplay currency.

Real money must never masquerade as game gold.

---

# 14. Entitlements domain

---

## 14.1 ProductDefinition

Defines purchasable app-store product:

- subscription
- cosmetic pack
- expansion
- consumable only if ever justified

---

## 14.2 PurchaseRecord

Normalized app-store purchase record.

Contains provider/platform references needed for verification.

---

## 14.3 Entitlement

Server-authoritative access right.

Examples:

- Premium active
- Cosmetic pack owned
- Expansion owned

Entitlements are derived from verified purchases/promotions/admin correction, not local receipt presence alone.

---

# 15. Sync domain

---

## 15.1 ClientMutation

Globally unique offline-capable user mutation.

Examples:

- Action created
- Action completed
- Quest edited

Concepts:

- mutation ID
- account
- device
- entity
- mutation type
- base version
- client timestamp
- received timestamp

Server handles mutations idempotently.

---

## 15.2 SyncCursor

Per device/account progress marker for incremental sync.

---

## 15.3 ConflictRecord

Represents an unresolved/reconciled edit conflict when simple last-write rules are unsafe.

Conflict policy varies by entity.

A duplicate completion is not treated as an edit conflict; it is handled by idempotency.

---

# 16. Notification & Live Ops domain

---

## 16.1 NotificationPreference

Per-account configurable delivery choices.

---

## 16.2 ScheduledReminder

Derived reminder schedule for Life OS tasks/habits/campaigns.

---

## 16.3 LiveEventDefinition

Data-driven event definition.

May eventually reference:

- story
- community objective
- merchant campaign
- cosmetic reward
- limited world state

---

## 16.4 FeatureFlag

Server-controlled rollout/kill switch.

Critical for:

- staged releases
- provider outages
- disabling risky commerce/location surfaces
- A/B testing where appropriate

---

# 17. Relationship summary

Conceptually:

`Account`
- has one Profile
- has Devices/Sessions
- owns Actions, Quests, Campaigns, Habits
- has AccountProgression and LifeSkillProgress
- owns PlayerCharacter
- owns PlayerCompanions
- owns one PlayerWorldState per unlocked IdentityWorld
- has PlayerStoryState per world
- receives ChronicleEntries
- has Needs and OfferMatches
- may have EvidenceCases
- receives RewardGrants and Entitlements

`Campaign`
- has ordered CampaignPhases
- contains/relates to Quests and Actions
- may drive Story framing
- may generate Chronicle/World/Companion events

`IdentityWorldDefinition`
- has Sagas/Chapters/NPCs/Structures/Companion definitions
- has separate PlayerWorldState and PlayerStoryState per Account

`Need`
- has NeedEvidence
- may match Offers
- may be satisfied by confirmed PurchaseEvidence

`EvidenceCase`
- aggregates provider-neutral evidence
- creates EvidenceDecision
- may qualify RealWorldReward

---

# 18. Authority matrix

| Entity / decision | Client may originate? | Server authoritative? | Offline-capable? |
|---|---:|---:|---:|
| Action draft/create/edit | Yes | Canonical after sync | Yes |
| Action completion intent | Yes | Yes for rewards | Yes |
| Quest/Campaign edit | Yes | Canonical after sync | Yes |
| Habit completion intent | Yes | Yes for progression | Yes |
| Focus session record | Yes | Yes for reward qualification | Yes |
| XP / Life Skill XP | No | Yes | Display cached |
| Gameplay currency | No | Yes | Display cached |
| Inventory grant | No | Yes | Display cached |
| World upgrade qualification | Intent may originate | Yes | Partially |
| Character appearance | Yes | Canonical after sync | Yes |
| Companion ownership | No | Yes | Display cached |
| Companion name/cosmetic selection | Yes | Canonical after sync | Yes |
| Story completion | Client reports conditions | Yes | Limited |
| Chronicle authoritative event | No/directly | Yes | Display cached |
| AI suggestion | Yes/request | No economic authority | Requires network unless local later |
| Need | Yes / inferred | Canonical server | Some local draft |
| Offer | No | Server/provider | Cached |
| Offer ranking | No | Server/config | Cached |
| Location observation | Device supplies | Server decides meaning | Queueable |
| Purchase qualification | No | Server/provider | No final decision offline |
| Cashback | No | Server/provider | No |
| Entitlement | No | Server/store verification | Cached |
| Feature flag | No | Server | Cached fallback |

---

# 19. Lifecycle invariants

## 19.1 No reward without an idempotent source

Every authoritative reward must reference:

- a stable source
- an idempotency key/event
- a reward/rule version

## 19.2 No destructive history rewrite for financial state

Refunds/reversals create new ledger events.

They do not erase history.

## 19.3 Content and player state remain separate

Updating a Saga definition cannot overwrite PlayerStoryState.

## 19.4 Provider IDs remain adapter data

Core Merchant, Offer, PurchaseEvidence, or Venue identity must not depend on one provider's ID namespace.

## 19.5 Account deletion is a domain lifecycle

Deletion must be deliberately propagated according to legal/security/financial retention requirements rather than blindly cascading all rows.

Exact retention rules are defined later.

---

# 20. V1 required entities

The following are required for V1 architecture:

- Account
- Profile
- DeviceRegistration
- ConsentRecord
- Action
- Quest
- Campaign
- CampaignPhase
- WorkRelation
- HabitDefinition
- HabitOccurrence
- RecurrenceRule
- FocusSession
- RestPeriod
- AccountProgression
- LifeSkillDefinition
- LifeSkillProgress
- MomentumState
- ProgressionEvent
- RewardDefinition
- RewardGrant
- minimal CurrencyDefinition / CurrencyLedgerEntry if currency ships
- PlayerCharacter
- CosmeticDefinition / OwnedCosmetic
- ItemDefinition / InventoryEntry where required
- IdentityWorldDefinition
- PlayerWorldState
- WorldStructureDefinition
- PlayerWorldStructure
- CompanionDefinition
- PlayerCompanion
- CompanionBondEvent
- ContentRelease
- SagaDefinition
- ChapterDefinition
- StoryNodeDefinition
- PlayerStoryState
- NPCDefinition
- PlayerNPCState as needed
- ChronicleEntry
- AchievementDefinition
- PlayerAchievement
- PlanningRequest
- PlanningSuggestion
- Need
- NeedEvidence
- Merchant
- Offer
- OfferMatch
- OfferInteraction
- AffiliateAttribution
- ProductDefinition
- PurchaseRecord
- Entitlement
- ClientMutation
- SyncCursor
- NotificationPreference
- ScheduledReminder
- FeatureFlag

---

# 21. Post-V1-capable entities that should not distort V1

Model/architecture should leave clean room for:

- FactionDefinition
- FactionReputation
- MerchantLocation
- EvidenceCase
- LocationEvidence
- VisitCandidate
- PurchaseEvidence
- ReceiptEvidence
- IntegrityEvidence
- EvidenceDecision
- SponsoredQuestDefinition
- SponsoredQuestActivation
- RealWorldReward
- RewardSettlementEvent
- CashBalance
- Payout
- LiveEventDefinition
- deeper social/guild entities

Do not build unused financial/location infrastructure in V1 solely because these entities exist in the long-term domain.

---

# 22. IDs and versioning requirements

Exact technology is deferred, but identifiers must be:

- Globally unique
- Safe for offline creation where client-originated
- Opaque to users
- Stable across migrations

Version-sensitive domains require explicit version references:

- Reward rules
- Progression rules
- Offer ranking rules
- Story/content releases
- AI prompt/template versions where reproducibility matters
- Evidence/fraud decision rules

---

# 23. Time rules

The server stores authoritative instants in a timezone-safe format.

User timezone remains an explicit account/planning concept.

Do not rely on device-local clock for:

- financial qualification
- offer expiration
- authoritative reward ordering
- anti-fraud timing

Client time may be recorded as evidence but server/provider time controls authoritative decisions.

---

# 24. Data minimization boundary

High-sensitivity domains must be separated from ordinary game/profile tables.

Examples:

- precise location evidence
- purchase provider payloads
- receipts
- device integrity evidence

Access should be narrower than access to ordinary gameplay data.

The product should store normalized results instead of raw provider payload forever when the raw data is no longer necessary.

---

# 25. Domain model success criteria

This model is sufficient when future schema/API design can answer:

- What entity owns this state?
- Is it account-wide or world-specific?
- Can the client create/edit it?
- Can it happen offline?
- Is the server authoritative?
- Does it need immutable history?
- Is it content or player state?
- Is it sensitive evidence?
- Is it tied to a provider or normalized internally?
- What event caused a reward/change?

If those answers remain ambiguous, the domain model needs refinement before implementation.

