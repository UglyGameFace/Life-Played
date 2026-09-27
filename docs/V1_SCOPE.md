# Life Played — V1 Scope Lock

**Status:** Authoritative V1 boundary  
**Depends on:** `docs/MASTER_PRODUCT_BIBLE.md`  
**Goal:** Define the smallest complete public release that proves Life Played's core thesis on Android and iOS without building the entire long-term platform at once.

---

# 1. V1 launch thesis

V1 must prove one thing exceptionally well:

> **A real-life action can meaningfully change a persistent 3D game world.**

If V1 proves that loop, Life Played has a foundation worth expanding.

If V1 tries to ship every long-term idea simultaneously, it will dilute the loop, multiply failure modes, and delay learning.

V1 is therefore a **complete product**, not a feature demo, but it is intentionally not the entire Life Played universe.

---

# 2. V1 audience

The first public release should work for people who want:

- A genuinely useful task/project/habit system
- A game-like reason to complete real-life goals
- A persistent visual world that reflects progress
- A customizable character and real 3D companion
- A story that reacts to real-world progress
- Optional AI help planning goals
- Useful deal discovery tied to things they already need

V1 must not require the player to like fantasy, fuzzy animals, combat, location tracking, or shopping.

---

# 3. Launch platforms

Required:

- Android / Google Play
- iPhone / Apple App Store

Both platforms are first-class.

V1 may not ship publicly on one platform while treating the other as a vague future promise unless an external store/platform blocker makes simultaneous release impossible.

---

# 4. Launch client

**Unity** is the intended V1 client.

V1 must demonstrate:

- Real-time stylized 3D
- Mobile-scalable rendering
- Animated player character
- Animated 3D companion
- Persistent 3D home/world
- Short cinematic reward moments
- Touch-first app UI integrated with the game

V1 does not require a large seamless open world.

---

# 5. Launch Identity Worlds

V1 ships with **two complete Founding Worlds**:

## 5.1 Wild Renewal — Cozy / Nature

Purpose:

- Prove the warm, restorative, non-combat-friendly side of Life Played.
- Serve players who want companionship, nature, building, exploration, and calm progression.

V1 requirements:

- Complete first Saga with a real ending
- One persistent 3D home hub
- Distinct NPC cast
- Distinct world terminology
- Distinct companion family
- Distinct environmental art/VFX
- Identity-specific campaign framing
- Restoration/exploration emphasis

## 5.2 Gridfall — Coder / Hacker / Tech

Purpose:

- Prove that Life Played can appeal to users who do not want cozy fantasy presentation.
- Demonstrate a sharper technical identity with systems, command centers, drones, digital districts, and cyber-style progression.

V1 requirements:

- Complete first Saga with a real ending
- One persistent 3D command hub
- Distinct NPC cast
- Distinct world terminology
- Distinct companion family
- Distinct environmental art/VFX
- Identity-specific campaign framing
- Systems/building/operations emphasis

## 5.3 Why only two at launch

Each Identity World must be a genuine experience, not a shallow palette swap.

Shipping six complete worlds in V1 would multiply:

- Story production
- Character production
- Environment production
- Companions
- Animation/VFX
- QA
- Localization
- Live-content complexity

The architecture must support more worlds, but V1 proves the model with two highly differentiated complete Founding Worlds.

---

# 6. Post-V1 Identity Worlds

Explicitly deferred until after the launch foundation is stable:

- Fantasy / Adventure
- Science Fiction
- Dark / Arcane

**Modern / Minimal** is treated differently:

V1 must include a **Minimal Presentation preference** that reduces story/game intensity and companion prominence for players who want the Life OS first.

It is not yet a separate authored Identity World in V1.

A full Modern / Minimal world remains post-V1.

---

# 7. Identity choice and switching

V1 must not trap the player permanently.

Required:

- Player chooses a Founding World during onboarding.
- Account-wide real-life history and universal progression are preserved.
- A player may begin the second Founding World later without deleting the first.
- Story/world-specific progress remains separate per Identity World.
- The UI clearly distinguishes account progression from world progression.

This establishes the multi-world architecture immediately without requiring more than two complete worlds at launch.

---

# 8. V1 onboarding

The first session should demonstrate the product promise rather than explain it through slides.

Target experience:

1. Create/sign in to account.
2. Choose Wild Renewal or Gridfall.
3. Create/customize a player character.
4. Choose a starter companion or no-companion option where appropriate.
5. Enter one meaningful real-life goal.
6. AI Quest Master may suggest a breakdown.
7. Player chooses one small starter Action.
8. Player completes or starts that Action.
9. The game world visibly reacts.
10. Player receives progression.
11. Chronicle records the first meaningful event.
12. The first story chapter opens.

The player should understand the Life Played loop through play.

---

# 9. V1 Life OS

Required V1 real-life utility:

## 9.1 Actions

- Create
- Edit
- Complete
- Reopen when appropriate
- Due date/time
- Optional estimate
- Optional category/skill hints
- Optional notes

## 9.2 Quests

- Multi-step objective
- Progress tracking
- Actions/subtasks
- Due date
- Status
- World/story presentation

## 9.3 Campaigns

- Multi-phase long-term goals
- Chapters/milestones
- Campaign progress
- Meaningful completion event
- Chronicle entry
- Permanent/visible world consequence where appropriate

## 9.4 Habits

- Repeat schedules
- Completion history
- Momentum contribution
- No destructive streak reset

## 9.5 Recurring actions

- Daily/weekly/custom cadence
- Safe recurrence generation
- Completion history

## 9.6 Focus sessions

- Preset durations
- Custom duration
- Pause/interrupt handling
- Completion record
- Identity-specific visual framing

## 9.7 Recovery

Required:

- Reschedule
- Pause
- Archive
- Abandon without punishment
- Planned Rest Day

## 9.8 Next Best Action

V1 should provide a simple recommendation based on:

- Due date
- Dependencies
- Estimated effort
- Campaign importance
- Available time where known

AI may enrich this recommendation, but deterministic fallback must exist.

---

# 10. V1 AI Quest Master

Required:

- Goal → suggested Actions
- Goal → suggested Quest/Campaign structure
- Suggest breaking down oversized tasks
- Suggest rescheduling/recovery
- Produce identity-specific narrative framing
- Summarize campaign progress

Required guardrails:

- Player approves generated task structure before it becomes authoritative.
- AI does not assign authoritative XP/currency.
- AI unavailability does not block task creation or completion.
- AI output is editable.
- AI quotas/cost controls exist from launch.

Not required V1:

- Open-ended autonomous life coach
- Always-on conversation memory controlling the game
- AI-created economy items
- AI fraud decisions

---

# 11. V1 progression

V1 includes:

- Account level
- Life Skills
- Momentum
- World progression
- Building progression
- Companion bond progression
- Achievements
- Chronicle
- Story progression

The exact Life Skill taxonomy and numeric curves are defined in the economy specification.

V1 launches with **one primary earnable gameplay currency/resource family**, plus world-specific materials only where needed.

Do not launch with a pile of currencies merely because mobile games have collectively lost restraint.

---

# 12. V1 world scope

Each Founding World includes:

- One polished persistent hub
- Multiple visually upgradeable hub structures/areas
- A surrounding explorable presentation space
- Story-specific interaction points
- NPC presence
- Companion presence
- Day/night presentation
- Weather/ambient variants where performance permits
- Visible progression transformations

V1 does **not** require:

- Massive seamless terrain
- Player-controlled vehicles
- MMO-scale shared world
- Dozens of regions
- Real-time multiplayer traversal

---

# 13. V1 story scope

Each Founding World ships with:

- One complete first Saga
- Clear opening
- Character introductions
- Escalation
- Major conflict/problem
- Final chapter
- Meaningful ending
- Permanent world transformation
- Post-Saga state that supports personal Campaigns and later content

Target content shape is approximately **6–10 substantive chapters per Founding World**, subject to production testing.

The first Saga ending must feel complete.

The game must not fake infinity by refusing to resolve anything.

---

# 14. V1 NPC scope

Each Founding World requires a small memorable core cast rather than dozens of disposable NPCs.

Target:

- 4–7 significant recurring NPCs per Founding World
- Clear functional/story roles
- Relationship state where useful
- Identity-specific dialogue and presentation

Deep branching relationship systems can expand post-V1.

---

# 15. V1 companion scope

Each Founding World launches with a small high-quality companion roster.

Target:

- 3 starter/early companions per Founding World
- 1 aspirational rare/late-Saga companion per Founding World
- Optional no-companion presentation
- Real 3D models
- Idle/movement/reaction animation
- Basic bond progression
- At least one visible bond/evolution milestone
- Theme-appropriate VFX for higher-tier companions

Example direction only, not final names:

Wild Renewal:
- Fox-like companion
- Owl/bird companion
- Spirit/nature construct
- Rare guardian creature

Gridfall:
- Recon drone
- AI orb
- Small mech/cyber companion
- Rare advanced construct

Deep breeding, trading, large encyclopedic collection systems are post-V1.

---

# 16. V1 player character

Required:

- Body/presentation choices appropriate to production scope
- Skin tone
- Hair
- Face options
- Basic outfit customization
- Identity-appropriate starter outfits
- Equipment/cosmetic slots sufficient to demonstrate long-term personalization
- Animated movement/idle/reward reactions

V1 does not need hundreds of sliders.

The character creator must be polished, inclusive, and readable on mobile.

---

# 17. V1 combat

Combat is **not required as a universal core mechanic**.

Gridfall may use lightweight encounters or tactical presentation where it strengthens the story.

Wild Renewal may resolve equivalent progression through restoration, exploration, or world interactions.

If combat is included, it must:

- Be simple on mobile
- Not gate Life OS functionality
- Not require twitch reflexes
- Not create pay-to-win pressure

A deep combat meta is post-V1 unless later foundation work proves it essential.

---

# 18. V1 Chronicle

Required:

- Major Campaign completions
- Story milestones
- Important achievements
- Companion milestones
- Major world upgrades
- First/important real-life accomplishments

Each entry includes enough context to be meaningful later.

V1 does not need AI-generated illustrated memoir pages for every event.

The data model must allow richer Chronicle presentation later.

---

# 19. V1 Home navigation

Recommended primary navigation:

- **Home**
- **Quests**
- **World**
- **Hero**
- **More**

## Home

- Today
- Next Best Action
- Active Campaign
- Momentum
- Important world/story prompt
- Relevant Deal Scout card when applicable

## Quests

- Actions
- Quests
- Campaigns
- Habits
- Focus

## World

- Persistent 3D hub
- Story
- Buildings
- NPCs
- Companion presence
- World progression

## Hero

- Character
- Life Skills
- Equipment/cosmetics
- Companion
- Achievements

## More

- Chronicle
- Deal Scout
- Account
- Privacy
- Notifications
- Accessibility
- Settings

Navigation can evolve after usability testing, but V1 must not bury the core loop.

---

# 20. V1 Deal Scout

V1 includes a **limited commerce beta** because the long-term business model benefits from real conversion evidence.

Required V1 boundaries:

- Recommendations begin from explicit or high-confidence player needs.
- Shopping list items may become Need Graph entries.
- Active Campaign needs may become Need Graph entries with player confirmation.
- The player can save/dismiss offers.
- The player can say Already Bought / Not Interested / Too Expensive.
- Partner/affiliate relationship is disclosed.
- Effective price may include known sale/coupon/partner reward.
- The product may tell the player to wait when an offer is not good enough.

V1 commerce should prioritize **online/deep-link affiliate offers and coupon/deal feeds** that do not require a stored-value wallet.

---

# 21. Not in V1 commerce

Explicitly post-V1 unless a launch partner materially changes the economics:

- User cashback wallet
- Card linking
- Card-linked transaction ingestion
- Automated receipt OCR at scale
- Merchant-funded cash payout engine
- Direct merchant campaign portal
- Item-level purchase verification
- Complex refund ledger across multiple providers
- Large brand-sponsored seasonal campaigns

The architecture must leave room for them.

---

# 22. V1 location

V1 does **not** depend on continuous background location.

Required:

- Location can be requested on demand for a player-facing feature if needed.
- Permission denial does not break core gameplay.
- Location state/consent UI is clear.
- Sensitive-place inference is prohibited.

The full automatic Sponsored Quest / geofence / cashback validation system is post-V1.

This prevents the first release from making background location reliability, store policy review, and financial fraud prevention prerequisites for proving the core game.

---

# 23. V1 Nearby mode

**Not required for launch.**

A basic on-demand nearby merchant/deal discovery surface may be included only if it does not threaten schedule, privacy clarity, or core-loop polish.

The full Pokémon-GO-like real-world Adventure/Nearby map remains a post-V1 milestone.

---

# 24. V1 monetization

V1 may monetize through:

- Optional premium subscription
- Additional AI/planning allowance
- Cosmetic character items
- Companion cosmetics
- World cosmetics/themes where appropriate
- Affiliate commissions from Deal Scout

Non-negotiable:

- No selling XP
- No purchasing major progression advantages
- No required purchase to finish first Saga
- No canon quest requiring a Sponsored Quest
- No loot-box dependency for meaningful progression

Exact pricing is deferred.

---

# 25. V1 offline-first requirement

Required:

- Create/edit/complete ordinary Actions offline
- View cached Quests/Campaigns
- View cached progression/world state
- Queue eligible local events
- Sync later
- Idempotent event IDs
- Clear sync state when necessary

Not every server-dependent feature works offline.

Examples that may require connection:

- AI generation
- Purchases
- Affiliate deal refresh
- Entitlement verification
- Account recovery
- Live content refresh

Offline failure must not duplicate rewards.

---

# 26. V1 server authority

Server validates/owns:

- Account progression
- Life Skill progression
- Important reward grants
- Valuable currency/resources
- Inventory grants
- Entitlements
- Story/world authoritative progress where needed
- Economy-sensitive events

The client may optimistically display safe provisional feedback but cannot manufacture authoritative value.

---

# 27. V1 authentication/account

Required:

- Secure account creation/sign-in
- Apple-compliant sign-in options where applicable
- Account recovery
- Cross-device cloud sync
- Account deletion flow
- Privacy controls
- Device/session management appropriate to threat model

Guest/offline-first onboarding may be considered during architecture work, but permanent account ownership and recovery must be solved before public release.

---

# 28. V1 notifications

Required:

- Due reminders
- Optional habit/recurring reminders
- Campaign reminder controls
- Reward/story notifications where useful

Must be configurable.

V1 must not become a notification harassment simulator.

---

# 29. V1 accessibility

Required from launch:

- Scalable text in app UI
- Reduced motion
- Adjustable VFX intensity
- Independent music/SFX controls
- Color-independent state communication
- Touch targets appropriate for mobile
- Captions for meaningful voiced/cinematic content if any
- Non-combat progression path
- Companion-free/minimal presentation option

Full accessibility audit remains part of validation.

---

# 30. V1 graphics/performance

Required:

- Automatic sensible device default
- Reduced/Standard/High quality classes at minimum
- Scalable particles
- Scalable shadows
- Scalable post-processing
- Graceful reduction of weather/environment effects
- Stable mobile thermal/battery behavior as a validation target

Ultra/Cinematic presentation may be exposed only where devices support it reliably.

---

# 31. V1 analytics/observability

Required product telemetry must be privacy-conscious and answer:

- Where onboarding fails
- Whether players complete first Action
- Whether real-life actions produce world engagement
- Campaign completion rate
- Retention
- Crash/error rates
- Sync failure rate
- AI feature usage/cost
- Deal Scout engagement
- Affiliate click/conversion data available to us
- Economy anomalies

Analytics must not become an excuse for indiscriminate data collection.

---

# 32. V1 content delivery

The first release should support data-driven content for at least:

- Quest/story definitions
- Dialogue
- Reward configuration
- Basic event configuration
- Offer/content flags
- Feature flags

This reduces dependence on app-store releases for ordinary content changes.

New 3D assets/code may still require a client update.

---

# 33. Explicit post-V1 backlog

The following are not required to declare V1 complete:

- Fantasy Identity World
- Science Fiction Identity World
- Dark / Arcane Identity World
- Full Modern / Minimal Identity World
- Large multi-world collection meta
- Deep faction reputation systems
- Romance systems
- Deep combat meta
- PvP
- Trading
- Player marketplace
- Guilds
- Global chat
- MMO shared world
- Large co-op systems
- Mount system
- Large companion encyclopedia
- Companion breeding/trading
- Health Connect
- HealthKit
- Full real-world Adventure/Nearby map
- Background geofence reward automation
- Card-linked rewards
- Cashback wallet
- Receipt-processing pipeline
- Direct merchant portal
- Large sponsored world events
- BLE/NFC merchant proof
- Global community boss system
- Large-scale seasonal live operations
- Widgets
- Lock-screen/live-activity surfaces
- Web client
- Desktop client
- Console versions

These remain part of the long-term vision unless later evidence removes them.

---

# 34. V1 launch Definition of Done

V1 is not ready for public release merely because it builds.

The launch candidate must demonstrate:

## Product

- A new player can understand the core loop without external explanation.
- Real-life Actions/Quests/Campaigns are useful and reliable.
- A completed real-life Action visibly affects the 3D world.
- Both Founding Worlds feel intentionally different.
- Each Founding World has a complete first Saga.
- Character and companion systems feel production-quality.
- Chronicle records meaningful milestones.
- Momentum/rest/recovery behavior works without punishment traps.
- Deal Scout never becomes a random coupon wall.

## Technical

- Android and iOS release builds succeed.
- Account/cloud sync works.
- Offline queueing/reconciliation works.
- Duplicate completion attempts do not duplicate authoritative rewards.
- Server-authoritative reward boundaries are enforced.
- AI failure does not block core gameplay.
- Feature flags/content delivery work.
- Backup/recovery strategy is validated.
- Crash/error observability is active.

## Quality

- Targeted unit/integration tests pass.
- Relevant mobile runtime paths are tested.
- Accessibility pass is completed.
- Performance/thermal behavior is tested across representative devices.
- Privacy disclosures match actual behavior.
- Store policy requirements are re-verified at submission time.
- No debug/test secrets or development bypasses ship.
- Final diff/build contents are reviewed.

---

# 35. V1 success criteria

V1 succeeds if it can answer **yes** to these questions:

1. Do players complete more real-life actions because the world reacts?
2. Do players care about returning to their world?
3. Do the two Founding Worlds appeal to meaningfully different tastes?
4. Do players form attachment to their character/companion?
5. Can long goals become satisfying Campaigns rather than giant checkboxes?
6. Can AI improve planning without becoming required?
7. Can Deal Scout surface relevant value without damaging trust?
8. Can the architecture safely support later location/cashback/sponsored systems?
9. Can content be extended without rebuilding the application every time?

If those are true, expanding the universe is justified.

---

# 36. Scope rule

Any proposed V1 feature must answer:

> **Is this necessary to prove the real-life → persistent 3D world loop, ship a trustworthy consumer product, or validate the initial business model?**

If not, it belongs in the post-V1 backlog unless removing it creates an architectural dead end.

