# Life Played — Master Product Bible

**Status:** Authoritative product foundation  
**Product:** Life Played  
**Repository:** `UglyGameFace/Life-Played`  
**Platforms:** Android / Google Play and iOS / Apple App Store  
**Client direction:** Unity  
**Tagline:** **Your life builds the world.**

---

## 1. Purpose of this document

This document is the authoritative definition of **what Life Played is and is not**.

It consolidates the product vision, game identity, real-life utility, long-term narrative model, companion/world systems, commerce model, privacy boundaries, reward philosophy, and architectural constraints established before implementation.

When future ideas conflict with this document, the conflict must be resolved deliberately rather than allowing a second competing implementation or design rule to emerge.

Exact V1 scope, economy numbers, schemas, provider selections, API design, art production specifications, and implementation milestones are defined in later foundation documents. They must remain consistent with this Bible.

---

# 2. Product thesis

Life Played is a **persistent stylized 3D life RPG powered by the player's real-world progress**.

It is not a task manager with decorative XP.

It is not a shopping app wearing a game skin.

It is not a location tracker that happens to award points.

It is not a traditional RPG that requires hours of screen time to make progress.

The product connects real life and game life:

`Real Life → Goals & Actions → Quest Engine → Progression → Character & World Growth → Story / Adventure → New Motivation`

The player should be able to look at their world months or years later and recognize a visual history of what they actually accomplished.

---

# 3. Core promise

> **What you do in real life should matter inside the game.**

A player's real-world actions may affect:

- Character progression
- Life Skills
- Momentum
- Campaign progression
- Buildings
- Regions
- World state
- Companions
- Factions
- Achievements
- Chronicle entries
- Story progression
- Exploration opportunities
- Cosmetic unlocks
- Collectibles
- Real-world reward eligibility where applicable

Progress must create visible consequences rather than only incrementing numbers.

---

# 4. Product pillars

Life Played has four primary pillars.

## 4.1 Life OS

The real-life planning layer must be useful even if the player temporarily ignores the RPG.

It may include:

- Tasks
- Habits
- Projects
- Campaigns
- Recurring actions
- Deadlines
- Dependencies/subtasks
- Shopping lists
- Focus sessions
- Scheduling
- Notifications
- Calendar integration
- Next Best Action
- Rescheduling/recovery
- Planned rest

The productivity system may be themed differently by identity world, but the underlying utility must remain clear and dependable.

## 4.2 Living 3D RPG

The player inhabits a persistent stylized 3D world containing:

- A customizable player character
- Real 3D companions
- Buildings
- Locations
- NPCs
- Factions
- Exploration
- Campaigns
- Encounters/bosses where appropriate
- Equipment
- Collectibles
- Decorations
- World evolution
- Day/night
- Weather
- Ambient life
- Cinematic milestone moments

The world grows because the player's real life grows.

## 4.3 Personal Chronicle

Life Played maintains a meaningful long-term record of real accomplishments.

Significant events may create:

- Chronicle entries
- Trophies
- Landmarks
- Building changes
- Decorations
- Companion evolutions
- Campaign records
- Titles
- Achievement displays
- World-state changes

The Chronicle must feel like a personal history rather than an analytics log.

## 4.4 Intelligent Commerce

Optional commerce features help the player satisfy **existing or strongly indicated needs** more intelligently.

The product may surface:

- Price drops
- Deals
- Coupons
- Cashback
- Affiliate offers
- Relevant nearby merchants
- Sponsored Quests
- Merchant-funded real-world rewards

Commerce must serve the player's existing intention before serving developer revenue.

---

# 5. Core experience loop

A typical Life Played loop:

1. The player has a real-life goal, need, habit, project, errand, or milestone.
2. Life Played represents it as an Action, Quest, or Campaign.
3. The player completes meaningful real-world progress.
4. The client records the action locally and synchronizes when possible.
5. Authoritative systems validate valuable progression/rewards.
6. The player receives immediate satisfying feedback.
7. Their character, Life Skills, companion, campaign, or world changes.
8. The Chronicle records important accomplishments.
9. New game/story opportunities emerge.

Real-world shopping or sponsored experiences may optionally extend the loop:

`Existing Need → Deal Scout → Relevant Offer → Visit/Purchase Evidence → Real Savings + Game Reward → World/Chronicle Progress`

---

# 6. Core terminology

## Action

A small, concrete real-life step that can usually be completed in one sitting.

Examples:

- Call the dentist
- Complete a 25-minute focus session
- Pick up groceries
- Write the login tests

## Quest

A meaningful objective that may contain one or more Actions.

Examples:

- Deep-clean the apartment
- Finish onboarding
- Prepare for a trip

## Campaign

A substantial real-life goal lasting days, weeks, or months, structured into chapters/phases.

Examples:

- Launch an app
- Move into a new home
- Train for an event
- Build a home office

Campaigns can receive major story framing, bosses, permanent world consequences, and Chronicle recognition.

## Identity World

A complete aesthetic/narrative interpretation of Life Played.

Identity Worlds share foundational systems but have their own story, world, terminology, NPCs, factions, companions, architecture, music direction, and visual language.

## Life Skill

A persistent category of real-world growth such as Focus or Knowledge.

Exact launch skill taxonomy is defined later.

## Momentum

A consistency system that rewards sustainable engagement without destroying progress when a player misses a day.

Momentum is not a punitive streak.

## Companion

A real 3D sidekick, creature, construct, drone, spirit, animal, or other optional entity that can bond, evolve, animate, react, and participate in the player's world.

## Chronicle

The player's persistent record of meaningful accomplishments and world-changing events.

## Need Graph

A privacy-conscious representation of known or strongly indicated player needs derived primarily from explicit goals, lists, campaigns, preferences, and confirmed purchase state.

## Deal Scout

The system that searches, compares, filters, and ranks relevant commercial opportunities for known player needs.

## Sponsored Quest

An optional real-world quest funded by a merchant/brand or partner program.

Sponsored Quests must never be required for core progression.

## Evidence

Signals used to validate location, visit, purchase, or reward eligibility.

No single weak signal is automatically treated as absolute truth for meaningful monetary value.

---

# 7. Actions, Quests, and Campaigns

Life Played must distinguish between trivial actions and major life achievements.

## Actions

Fast, concrete, low narrative overhead.

## Quests

Multi-step objectives with meaningful game consequences.

## Campaigns

Large goals organized into phases or chapters.

A Campaign may become an RPG-style boss structure.

Example:

`Launch App → Plan → Prototype → Build → Test → Publish`

In one Identity World this may be framed as a system operation; in another it may be a fantasy trial. The underlying real-life structure remains the same.

The game must not pretend that every small checkbox is equally epic.

---

# 8. AI Quest Master

AI is an assistant to the player, not the authority over valuable game state.

## AI may:

- Break large goals into Actions/Quests
- Suggest dependencies
- Suggest realistic scheduling
- Recommend splitting overwhelming work
- Identify a useful Next Best Action
- Suggest recovery after missed deadlines
- Summarize progress
- Translate real-life campaigns into identity-specific narrative framing
- Produce safe dialogue/story variation under authored constraints

## AI must not directly decide:

- Authoritative XP
- Valuable currency
- Cash rewards
- Purchase qualification
- Affiliate attribution
- Premium entitlement
- Inventory grants with economic value
- Fraud outcomes
- Competitive/shared authoritative outcomes

Those systems must be deterministic or server-authoritative.

---

# 9. Progression philosophy

Progression must represent genuine engagement, not simply wealth or repetitive checkbox farming.

Intended progression layers:

- Account level
- Life Skills
- Momentum
- Campaign progress
- World progress
- Buildings/regions
- Faction reputation
- Companions
- Equipment/cosmetics
- Achievements
- Chronicle
- Legacy progression

## 9.1 Account level

Represents broad long-term progression.

It must not be the only meaningful number.

## 9.2 Life Skills

Different real-world actions may advance different categories.

Candidate skill families discussed include:

- Focus
- Knowledge
- Fitness
- Creativity
- Craft
- Finance
- Social
- Home

Final taxonomy and formulas are defined in the economy specification.

## 9.3 Momentum

Momentum:

- Rewards sustainable consistency
- Can rise through healthy regular engagement
- Can decay gradually
- Supports planned rest
- Does not erase months of progress because of one missed day

## 9.4 Reward anti-farming

Reward value may consider:

- Estimated effort
- Duration
- Complexity
- Frequency
- Dependencies
- Repetition
- Similarity to recent tasks
- Historical player behavior
- Campaign context
- Completion patterns

Users may suggest difficulty, but they do not directly assign authoritative reward value.

---

# 10. Spending must not equal winning

A larger purchase may justify:

- More cashback
- Better merchant-funded real-world reward
- A more memorable cosmetic or world event
- A thematic Chronicle moment
- A special decoration
- A merchant-specific collectible

It must **not** simply convert dollars into proportionally massive XP.

Core progression cannot become:

`Spend More Money → Become Stronger Than Everyone`

The meaning of real-life progress must survive monetization.

---

# 11. Persistent world

The player's world is not decorative wallpaper.

It is a persistent visual representation of progress.

Possible world changes include:

- New buildings
- Building tiers
- New districts
- New NPCs
- New paths
- New regions
- Monuments
- Trophy displays
- Companion spaces
- Gardens
- Workshops
- Lighting/environment changes
- Seasonal states
- Story consequences

Significant real accomplishments can create permanent landmarks.

Examples:

- Completing a major software project may alter a Dev Lab.
- Completing a long fitness campaign may create a training monument.
- Completing a major learning goal may expand an archive/library.
- Completing a major financial milestone may transform a market/business district.

The exact representation depends on Identity World.

---

# 12. Visual direction

## Master visual rule

**Cozy while living. Cinematic while achieving.**

Normal play should feel:

- Inviting
- Comfortable
- Readable
- Aspirational
- Alive

Major milestones should allow:

- Camera movement
- VFX
- Lighting changes
- Transformations
- Companion reactions
- Reward reveals
- Short cinematic sequences

## Rendering direction

- Stylized 3D
- Premium but mobile-scalable
- Strong silhouettes
- Expressive animation
- Readable materials
- Avoid dependence on photorealism
- Quality tiers for a broad range of supported phones

Detailed art production rules are defined later.

---

# 13. Identity Worlds

Players must not be forced into one visual personality.

Initial directions:

## 13.1 Cozy / Nature

Themes:

- Restoration
- Community
- Exploration
- Ecology
- Craft
- Warmth

Visual language may include forests, cabins, gardens, bridges, rivers, lanterns, wildlife, and warm natural materials.

Companions may include animals, spirits, or nature constructs.

## 13.2 Fantasy / Adventure

Themes:

- Shattered realms
- Guardians
- Ancient civilizations
- Magic
- Kingdom restoration
- Exploration

Companions may include dragons, familiars, magical beasts, spirits, and constructs.

## 13.3 Coder / Hacker / Tech

Themes:

- Systems
- Digital rebuilding
- Networks
- Rogue AI
- Cyber cities
- Infrastructure
- Technical factions

Visual language may include command centers, neon architecture, Dev Labs, holograms, drones, smart equipment, and digital districts.

Companions may include recon drones, AI orbs, mini-mechs, holographic creatures, signal ravens, and cyber constructs.

## 13.4 Science Fiction

Themes:

- Colonization
- Exploration
- Alien ruins
- Research
- Planetary development
- Star systems

Companions may include robotic explorers, engineered creatures, alien life, and small droids.

## 13.5 Dark / Arcane

Themes:

- Forbidden knowledge
- Corruption
- Mystery
- Gothic restoration
- Ritual
- Hidden power

Companions may include ravens, wisps, spectral creatures, night drakes, rune familiars, and shadow constructs.

## 13.6 Modern / Minimal

Themes:

- Contemporary growth
- Career/project progress
- Modern city life
- Design
- Entrepreneurship
- Personal development

Visual language may include lofts, studios, modern city districts, clean architecture, smart homes, and refined public spaces.

Companions may be optional and may include realistic animals, sleek robots, light constructs, or no companion at all.

---

# 14. Identity personalization

Identity selection must not permanently trap the player.

Account-wide progress can remain while a player changes or adds worlds.

Potential long-term model:

`Account → Multiple Identity Worlds → Separate Story/World Progress → Shared Real-Life History`

Shared account-level information may include:

- Account progression
- Life Skills
- Chronicle
- Universal achievements
- Purchases/entitlements
- Certain cosmetics
- Known preferences

Identity-specific information may include:

- Story state
- Factions
- NPC relationships
- Buildings
- Regions
- Identity companions
- Local world cosmetics

Exact cross-world boundaries are defined later.

---

# 15. Stories end; the world does not

Life Played must not rely on one literally endless plot.

Instead:

- Individual arcs end.
- Sagas end.
- Seasonal stories end.
- Campaigns end.
- New arcs begin.

Long-term structure can include:

- Main Sagas
- Regional arcs
- Faction stories
- NPC stories
- Companion stories
- Player-generated Campaigns
- Seasonal arcs
- Live world events
- Expansions
- Additional Identity Worlds
- Legacy progression

This creates closure without making the whole product finite.

---

# 16. Seasonal/live content

Live service content may include:

- Seasonal story arcs
- Community objectives
- Limited live events
- Special companions/cosmetics
- World changes
- Merchant campaigns
- New discoveries

Important narrative content should be archivable so later players can experience essential lore.

Limited-time exclusivity should focus more on participation cosmetics/status than permanently deleting important story.

Live Ops must eventually be data-driven so ordinary content releases do not require a new app binary.

---

# 17. Companions

Companions are first-class game entities.

They may have:

- Real 3D models
- Rigs
- Idle/walk/run/fly animations
- Emotional reactions
- Bond progression
- Appearance customization
- VFX
- Story arcs
- Evolution/branching forms
- World interactions
- Light game utility

Players may prefer:

- Cute/fuzzy
- Realistic
- Mechanical
- Cyber
- Fantasy
- Dark
- Elemental
- Minimal
- No companion

No visual preference should be treated as the default personality for every player.

Companion rarity may influence visual presentation, but rarity cannot substitute for good character design.

---

# 18. Combat and non-combat play

Combat should not be mandatory for players who do not enjoy it.

Possible presentation modes:

## Adventure-oriented

- Battles
- Bosses
- Abilities
- Equipment
- Combat encounters

## Cozy/restoration-oriented

- Exploration
- Restoration
- Building
- Discovery
- Relationships

## Minimal/productivity-oriented

- Clean planning experience
- Subtle progression
- Optional world interaction

The same underlying real-life accomplishment may receive different presentation without changing its legitimacy.

---

# 19. Focus sessions

Focus should be transformed into a meaningful in-world activity.

A focus session may be presented as:

- Expedition
- Deep Work mission
- Archive dive
- Simulation
- Ritual
- Build sprint

The player receives feedback after completing the session.

The product should encourage focus without coercively locking the user's device or shaming interruptions.

---

# 20. Rest and recovery

Rest is legitimate.

Life Played must support:

- Planned rest days
- Pausing campaigns
- Rescheduling
- Recovery after inactivity
- Returning after long absence

Return itself may be positively recognized.

The product must not treat human inconsistency as moral failure.

---

# 21. Real-world Adventure / Nearby mode

Life Played may contain a location-aware layer where the real world becomes an optional adventure surface.

Potential entities:

- Participating merchants
- Landmarks
- Parks/public places
- Sponsored locations
- Limited-time events
- Nearby discoveries
- Real-world quest markers

The experience must use original Life Played branding/content and must not imitate another game's intellectual property.

Location access must be scoped to actual player-facing functionality.

---

# 22. Location is evidence, not proof

Mobile location is imperfect.

The system must anticipate:

- Delayed geofence events
- Approximate location
- Weak indoor GPS
- Battery saver
- OS background throttling
- App termination
- No connectivity
- Reboots/reinstalls
- Shared buildings
- Dense malls
- Drive-by events
- Curbside pickup
- Drive-through
- Delivery
- Short valid visits
- Provider outages
- Late/backfilled signals

A missing geofence event cannot be the only reason a legitimate reward is lost.

---

# 23. Real-world reward validation

Reward strength must be proportional to evidence strength.

Potential evidence:

- App authenticity
- Device integrity
- Location freshness/accuracy
- Mock/simulation indicators
- Motion plausibility
- Dwell
- Venue confidence
- Purchase authorization
- Purchase settlement
- Loyalty confirmation
- Receipt validation
- Merchant QR/NFC/BLE proof
- Server challenge/nonce
- Duplicate detection
- Account/device risk history

Examples:

- A low-value park discovery may tolerate weaker location evidence.
- A small sponsored store visit requires stronger visit confidence.
- Cash reward for a purchase requires purchase evidence.
- High-value promotions may require settlement and stronger integrity/fraud checks.

---

# 24. Honest-user fallback

Anti-fraud must not punish ordinary platform failures.

When automatic visit validation fails, Life Played may fall back to:

- Purchase confirmation
- Loyalty transaction
- Receipt
- Provider backfill
- Merchant proof
- Manual review where economically justified

The system should distinguish **missing evidence** from **evidence of fraud**.

---

# 25. Reward lifecycle

Real-world reward state must be explicit.

Conceptual states include:

`AVAILABLE → ACTIVATED → VISIT_CANDIDATE → NEEDS_VALIDATION / VISIT_VERIFIED → PURCHASE_PENDING → PURCHASE_VERIFIED → REWARD_PENDING → REWARD_FINAL → REVERSED`

Not every reward uses every state.

Game feedback may happen earlier than cash settlement when confidence is sufficient.

Cashback may remain Pending while the RPG recognizes the action.

Refunds and partial refunds must be reconcilable.

---

# 26. Server authority and event integrity

The mobile client is not authoritative for valuable state.

Server-authoritative or server-validated state includes:

- XP
- Skill progression
- Valuable currencies
- Reward grants
- Inventory grants
- Premium entitlements
- Purchase qualification
- Cashback state
- Fraud outcomes
- Shared/competitive authoritative progress

Offline actions require:

- Client-generated unique IDs
- Durable local queueing
- Idempotent server handling
- Reconciliation
- Duplicate prevention

Repeated taps or retries must not create repeated rewards.

---

# 27. Commerce Intelligence

Commerce must begin from player relevance.

Possible input signals, when appropriate:

- Explicit Need Graph entries
- Shopping lists
- Campaign requirements
- Saved items
- User budgets
- Preferred merchants
- Preferred brands
- Rejected brands/categories
- Confirmed purchase history
- Offer interactions
- Recurring purchase timing
- Commercial location preferences
- Explicit feedback

The system should not infer sensitive personal traits from visited places or purchases.

---

# 28. Need confidence

Not all inferred needs are equal.

General hierarchy:

1. Explicit player statement/list item
2. Explicit campaign requirement
3. Saved item
4. Confirmed recurring purchase pattern
5. Behavioral inference
6. Weak location-only inference

Weak inferences should not trigger aggressive recommendations.

The system may ask the player to confirm an inferred need.

---

# 29. Deal Scout

Deal Scout exists to improve purchase decisions.

Potential capabilities:

- Search partner offers
- Search coupons/promotions
- Compare retailer pricing
- Include cashback
- Include merchant-funded rewards
- Calculate effective price
- Watch saved needs
- Watch price thresholds
- Alert on meaningful drops
- Recommend waiting when a price is poor

Deal Scout must distinguish:

- Merchant/network-confirmed offers
- Recently validated offers
- Unverified offers
- Expired offers

The product should prefer authoritative merchant/network data when available.

---

# 30. Offer ranking

Commercial ranking should not simply maximize developer commission.

A conceptual ranking considers:

- Need relevance
- User savings
- Product/merchant fit
- Timing
- Purchase probability
- Distance/travel burden
- Verification confidence
- Return risk
- Notification fatigue
- Merchant quality
- Developer revenue

A materially worse deal should not outrank a significantly better player deal merely because commission is higher.

Where user value is approximately equivalent, economics may be used as a tie-breaker.

---

# 31. Purchase-awareness

Once the player buys something, the system should stop treating that exact need as unresolved.

Related-product recommendations require contextual justification rather than immediate cross-selling.

Example:

Buying a TV does not automatically justify pushing four accessories.

An active `Mount TV` quest may justify surfacing a wall-mount offer.

---

# 32. Commercial transparency

Players should be able to understand:

- Why an offer appeared
- Whether it is sponsored/affiliate-supported
- What they save
- What game reward applies
- Whether cashback is pending/final
- What data type contributed to the recommendation

Affiliate/partner relationships must be disclosed clearly in the product experience.

---

# 33. Merchant/affiliate strategy

Potential partner classes include:

- Affiliate networks
- Deal/coupon feeds
- Cashback networks
- Card-linked transaction providers
- Receipt/item-level providers
- Location/place providers
- Direct merchants

No individual provider is a permanent architectural assumption.

Provider integrations must be implemented behind adapters.

Long-term commercial path may be:

`Affiliate Publisher → Proven High-Intent Commerce Channel → Preferred/Exclusive Offers → Direct Merchant Campaign Platform`

---

# 34. Sponsored Quests

Sponsored Quests are optional merchant-funded experiences.

They may reward:

- Verified visit
- Qualifying purchase
- Category purchase
- Event participation
- Merchant interaction

They can provide:

- Cashback
- RPG reward
- Cosmetic
- World decoration
- Chronicle event
- Limited collectible

They must not become mandatory gates for canon story or meaningful core progression.

---

# 35. Merchant portal — long-term direction

At sufficient scale, Life Played may allow approved merchants to configure campaigns such as:

- Participating locations
- Budget
- Qualification rules
- Campaign dates
- Player reward
- Cashback
- Sponsored Quest theme
- Aggregate performance analytics

Campaign configuration must not bypass Life Played's relevance, privacy, safety, or economy controls.

---

# 36. Privacy principles

## Non-negotiable

- Core gameplay works without continuous location tracking.
- Location permissions are feature-specific.
- Commerce personalization must be understandable and controllable.
- Sensitive-location inference is prohibited as a product strategy.
- Data collection should be minimized to what the feature requires.
- Merchant reporting should prefer aggregated/pseudonymous measurement where possible.
- The product must not depend on selling raw precise location histories.

The player should feel assisted, not observed.

---

# 37. Sensitive inference exclusions

Life Played must not infer or target offers based on sensitive traits merely because of visited places or purchase patterns.

Examples include:

- Health/medical conditions
- Religion
- Political affiliation
- Sexual orientation
- Pregnancy
- Addiction/treatment status
- Other highly sensitive personal categories

Commerce does not need these categories to be useful.

---

# 38. Monetization philosophy

Potential revenue:

- Affiliate commissions
- Cashback margin/revenue share
- Sponsored Quests
- Sponsored real-world adventures
- Direct merchant campaigns
- Premium subscription
- Advanced planning/AI features
- Cosmetics
- Companion cosmetics
- World themes
- Premium story/expansion content

## Non-negotiable

Core progression cannot be pay-to-win.

Players cannot purchase legitimacy, accomplishment, or overwhelming XP advantage.

Monetization should enhance:

- Personalization
- Convenience
- Story breadth
- Cosmetic expression
- Useful commerce

rather than corrupt the meaning of real-life progress.

---

# 39. Social design philosophy

Long-term social systems should emphasize cooperation over toxic productivity ranking.

Possible systems:

- Guild/co-op campaigns
- Community restoration
- Shared world events
- Friend encouragement
- Co-op exploration

Private task content remains private unless the player explicitly shares it.

A social feed may say:

`Player completed a Major Campaign`

without revealing the private campaign title.

---

# 40. Accessibility

Accessibility is a foundation requirement, not cleanup.

The design should support, as applicable:

- Dynamic/large text
- Screen-reader labels
- Reduced motion
- Reduced visual effects
- High contrast
- Color-independent status communication
- Captions
- Haptic controls
- Sound controls
- One-handed mobile navigation
- Non-combat progression paths

---

# 41. Mobile performance philosophy

Life Played must not require a flagship phone to be enjoyable.

The client should support scalable tiers such as:

- Reduced
- Standard
- High
- Cinematic/Ultra where supported

Adjustable systems may include:

- Render scale
- Shadows
- Particles
- Post-processing
- Vegetation density
- Weather complexity
- Reflection quality
- Environmental effects

Important reward moments must degrade gracefully.

---

# 42. Offline-first philosophy

Core productivity and appropriate local game interactions should remain usable during poor connectivity.

The client should maintain:

- Local persistent data
- Pending action/event queue
- Sync state
- Retry-safe identifiers

The backend reconciles authoritative state later.

Money, premium entitlement, valuable rewards, and shared authoritative state are not blindly trusted from offline client claims.

---

# 43. Architecture principles

Exact technologies beyond Unity remain unresolved until the architecture step.

However, the following constraints are authoritative:

- Unity is the current client direction.
- Backend providers must be replaceable where practical.
- Valuable game state is server-authoritative.
- Provider-specific commerce/location code sits behind adapters.
- Async/background workloads use queues/workers rather than blocking critical interactions.
- The data model should use structured relational concepts rather than one monolithic user blob.
- Events/rewards require idempotency.
- Observability, backup, recovery, and reconciliation are first-class requirements.
- Core gameplay must not depend on an AI provider being available.

---

# 44. Live content architecture principle

Long-term content must be data-driven where practical.

The eventual content system should be able to define:

- Sagas
- Chapters
- Missions
- Dialogue
- NPCs
- Conditions
- Rewards
- Companion content
- Faction content
- Events
- Merchant campaigns
- Release schedules

without requiring a full application release for ordinary content changes.

New engine functionality/assets may still require client updates.

---

# 45. V1 philosophy — provisional until dedicated scope lock

V1 must prove the core thesis:

> **A meaningful real-life action changes a compelling 3D game world.**

V1 should not attempt the entire long-term platform.

The dedicated V1 Scope document will decide exact inclusions.

Current principles for that cut:

- One coherent end-to-end loop is more important than breadth.
- At least one polished 3D home/world experience must prove persistence.
- Real-life Actions/Quests/Campaigns must be genuinely useful.
- Character and companion presentation must demonstrate the identity vision.
- Offline and sync boundaries must be correct from the beginning.
- The economy must be server-safe from the beginning.
- Commerce should not delay proving the core life-to-world loop unless required for launch strategy.
- Full multi-world breadth, merchant portal, large social systems, and deep live operations are candidates for later milestones unless explicitly promoted into V1.

This section is **not** the final V1 contract.

---

# 46. Explicitly unresolved decisions

The following are intentionally not locked by this document:

- Final trademark/domain clearance for Life Played
- Bundle identifiers
- Exact Unity version
- Exact backend/cloud provider
- Exact authentication provider
- Exact database provider
- Exact affiliate/location/payment partners
- Exact Life Skill taxonomy
- XP curves and reward formulas
- Final currency/resource list
- Final rarity names
- Final V1 identity-world count
- Final starter companion roster
- Final character customization slots
- Final monetization pricing
- Final subscription structure
- Final story titles/names
- Final art production specifications
- Final supported device floor
- Final merchant revenue splits

These require dedicated evidence and design work.

---

# 47. Product guardrails

Any future feature should be challenged against these questions:

1. Does it improve real-life utility?
2. Does it strengthen the living RPG?
3. Does it deepen the player's long-term history/world?
4. Does it help the player save money or complete an existing need?
5. Does it preserve trust and agency?
6. Can it scale without becoming a fragile special case?

Features that satisfy none of these should not enter the product merely because they are fashionable.

---

# 48. Non-goals

Life Played is not intended to become:

- A surveillance/location-data resale business
- A gambling product
- A pay-to-win RPG
- A generic coupon wall
- A social network that exposes private goals by default
- A punitive streak app
- A photorealistic open-world console game squeezed onto phones
- A platform whose basic gameplay collapses if AI is unavailable
- Six independent games with duplicated code and content infrastructure

---

# 49. Long-term identity

The long-term product can be summarized as:

**Life OS + Living RPG + Personal Chronicle + Intelligent Commerce**

The life layer provides genuine utility.

The RPG layer creates emotional payoff.

The Chronicle creates attachment and history.

The commerce layer saves the player money and creates sustainable revenue when it serves a real need.

---

# 50. Final product principle

> **Your life builds the world.**

Every major Life Played system should preserve that relationship.

A player's world should ultimately feel like something they earned, shaped, and remember — not something they merely clicked through.
