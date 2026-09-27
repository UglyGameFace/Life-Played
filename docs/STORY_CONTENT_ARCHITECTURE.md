# Life Played — Story & Content Architecture

**Status:** Authoritative narrative/content foundation  
**Depends on:** `docs/MASTER_PRODUCT_BIBLE.md`, `docs/V1_SCOPE.md`, `docs/CORE_DOMAIN_MODEL.md`, `docs/GAME_ECONOMY.md`

---

# 1. Narrative design goal

Life Played must deliver two things at the same time:

1. **Authored stories with real beginnings, escalation, endings, characters, consequences, and emotional payoff.**
2. **A world that can continue evolving for years after any one story ends.**

The core rule is:

> **Stories end. The world does not.**

Life Played must never fake longevity by refusing to resolve plotlines. Individual Sagas, companion stories, faction arcs, seasonal events, and player-generated Campaigns may all conclude cleanly.

Long-term continuity comes from new Sagas, new regions, new worlds, new relationships, new player-created Campaigns, and evolving world state.

---

# 2. Story stack

Life Played content exists at multiple layers.

## 2.1 Identity World

Highest authored narrative container.

Examples:

- Wild Renewal
- Gridfall

An Identity World owns:

- Canon
- Terminology
- Major locations
- NPC cast
- Factions
- Companion families
- Main Sagas
- Visual language
- Audio direction
- World transformation rules
- Identity-specific framing for player-created Campaigns

---

## 2.2 Saga

A complete authored major story arc.

A Saga has:

- Opening state
- Central conflict
- Escalation
- Major revelations
- Climax
- Resolution
- Permanent world consequence
- Post-Saga state

A Saga must be satisfying if no later Saga existed.

Future Sagas build on the resulting world rather than retroactively making the previous ending meaningless.

---

## 2.3 Chapter

A substantial unit of authored story progression.

Typical responsibilities:

- Introduce a location, NPC, mystery, system, or conflict
- Advance a relationship or faction
- Create a real-world gameplay objective
- Alter world state
- Build toward a larger Saga beat

V1 target remains approximately **6–10 substantive chapters per Founding World**, subject to production testing.

---

## 2.4 Story Beat

A smaller authored unit inside a Chapter.

Examples:

- Dialogue scene
- World discovery
- Character introduction
- Reveal
- Choice
- Companion reaction
- Structure upgrade
- Short cinematic moment

Story Beats should be composable in a data-driven content system.

---

## 2.5 Personal Campaign

A real-life player goal translated into identity-specific narrative framing.

Personal Campaigns are not canon-changing authored Sagas, but they should feel native to the chosen world.

Example real goal:

`Launch my first app`

Wild Renewal framing:

`Restore the Workshop Dream`

Gridfall framing:

`Operation: First Deployment`

The underlying Campaign remains the player's real-life goal.

---

## 2.6 Companion Story

A narrative line tied to one companion or companion family.

May include:

- Discovery/acquisition
- Bond milestones
- Memory/recovery arc
- Personality development
- Evolution choice
- Signature world event

Companion Stories may overlap with a Saga without being required to finish the Saga.

---

## 2.7 NPC Story

A character-specific authored arc.

May include:

- Trust/relationship growth
- Personal conflict
- Side objectives
- World consequences
- Unlocks
- Optional branching outcomes

---

## 2.8 Faction Arc

Longer-term reputation/story content for a group.

Faction systems are architecturally supported from V1 but can deepen post-V1.

Faction progression must not require one irreversible early choice that permanently deletes huge portions of content.

---

## 2.9 Seasonal Arc

Time-bounded live narrative content.

A Season may include:

- Special story
- Limited world state
- Community objective
- Seasonal companion/cosmetic
- Event-specific world decoration
- Optional Sponsored Quest layer

Important lore should enter the Archive later.

---

## 2.10 Live Event

Shorter event state layered on top of normal play.

Examples:

- Signal outage
- Restoration drive
- Limited NPC arrival
- World anomaly
- Community objective

Live Events should not overwrite permanent player history.

---

# 3. Founding World 1 — Wild Renewal

## 3.1 Core fantasy

A neglected living world is slowly returning.

The player is not conquering it.

They are helping restore:

- habitat
- community
- craft
- memory
- balance

Wild Renewal must work beautifully for players who do not want combat.

---

## 3.2 Tone

- Warm
- Hopeful
- Curious
- Emotional without being childish
- Quiet mystery
- Nature-forward
- Community-driven
- Restorative

The world may contain danger or loss, but the emotional center is rebuilding.

---

## 3.3 Player role

The player becomes a **Waykeeper** working from an abandoned settlement called **Hearthwild** as the working V1 hub name.

Final names remain content-level decisions until naming review.

The Waykeeper helps reconnect places, people, creatures, and forgotten routes.

---

## 3.4 V1 first Saga working title

**Saga I: The Quiet Bloom**

Purpose:

Introduce:

- Hearthwild
- core NPCs
- first companion
- world restoration
- Chronicle
- Campaign framing
- the mystery behind why the region went quiet

---

## 3.5 Wild Renewal V1 chapter spine

### Chapter 1 — The Empty Hearth

Player arrives at a quiet, partially abandoned hub.

Gameplay goals:

- Create first real Action
- Complete first Action
- Trigger first visible world restoration
- Meet first core NPC

Permanent result:

- Campfire / central hearth restored
- First Chronicle entry

### Chapter 2 — Footprints in the Moss

Introduce first companion discovery.

Gameplay goals:

- Habit or repeating action introduced
- First companion Bond event
- Explore nearby path

Permanent result:

- Companion joins or optional companion path unlocks
- First nature route opens

### Chapter 3 — The Workshop Wakes

Introduces project/Campaign framing.

Gameplay goals:

- Create a Quest
- Create/accept a multi-step Campaign
- Complete first Campaign phase

Permanent result:

- Workshop structure restored to Tier I

### Chapter 4 — Voices Return

Introduces community/NPC continuity.

Gameplay goals:

- Multiple Life Skill categories
- One optional relationship side objective
- One planning/recovery moment

Permanent result:

- New NPC space becomes active
- Hub feels more populated

### Chapter 5 — The Faded Grove

First major mystery/reveal.

Gameplay goals:

- Sustained Campaign progress
- World discovery
- Companion reaction / story beat

Permanent result:

- Grove region begins restoration

### Chapter 6 — The Long Rain

Tests recovery instead of punishment.

Gameplay goals:

- Rest / reschedule / pause mechanics
- Story acknowledges interruption rather than failure

Permanent result:

- Weather/world transformation
- Player learns that rest is canonical, not punishment

### Chapter 7 — Roots Remember

Major history reveal.

Gameplay goals:

- Chronicle becomes narratively important
- Past accomplishments affect current scene presentation

Permanent result:

- Landmark representing the player's own prior progress

### Chapter 8 — The Quiet Bloom

Saga climax and resolution.

Gameplay goals:

- Finish a substantial personal Campaign or Saga-defined sequence
- Major world transformation
- First Saga completion celebration

Permanent result:

- Hearthwild visibly transformed
- Rare guardian companion opportunity
- Saga I Chronicle monument
- Post-Saga free-play state
- Tease next region without invalidating the ending

---

# 4. Founding World 2 — Gridfall

## 4.1 Core fantasy

A once-connected digital civilization fractured after a catastrophic systems collapse.

The player restores infrastructure while uncovering:

- rogue systems
- lost archives
- competing factions
- corrupted sectors
- the truth behind the Gridfall

---

## 4.2 Tone

- Smart
- Stylish
- Technical
- Mysterious
- Slightly edgy
- Aspirational
- High-energy at milestones
- Not parody “Hollywood hacker” nonsense

The world should feel like a place coders, builders, creators, and tech-minded players genuinely enjoy inhabiting.

---

## 4.3 Player role

The player becomes an **Operator** based in a damaged command hub called **Node Zero** as the working V1 hub name.

Final content names remain subject to creative review.

---

## 4.4 V1 first Saga working title

**Saga I: Boot Sequence**

Purpose:

Introduce:

- Node Zero
- core NPCs
- first drone/AI companion
- system restoration
- data fragments
- the initial Gridfall mystery

---

## 4.5 Gridfall V1 chapter spine

### Chapter 1 — Cold Start

Player enters a barely functioning Node Zero.

Gameplay goals:

- Create first real Action
- Complete first Action
- Restore basic hub power

Permanent result:

- Core console activates
- First Chronicle/system log entry

### Chapter 2 — Ping

A weak unknown signal appears.

Gameplay goals:

- Introduce companion/drone
- Introduce Habit or recurring Action
- Track one persistent real-life routine

Permanent result:

- Signal map opens
- Companion joins or alternate minimal assistant route

### Chapter 3 — Build Pipeline

Introduces Quest/Campaign structure.

Gameplay goals:

- Build a multi-step real-life project
- Complete first Campaign phase

Permanent result:

- Dev Lab comes online

### Chapter 4 — Ghost Process

Introduces first deeper mystery and recurring NPC.

Gameplay goals:

- Focus session
- Investigation-style story framing
- Next Best Action

Permanent result:

- Hidden process/data layer unlocks

### Chapter 5 — Black Circuit

Introduces faction tension.

Gameplay goals:

- Choice with limited consequence
- Player sees that different approaches alter dialogue/world flavor

Permanent result:

- Faction contact established
- No permanent giant content lockout

### Chapter 6 — System Drift

Narratively incorporates overload/recovery.

Gameplay goals:

- Pause/reschedule/rest
- Demonstrate recovery mechanics
- Optional companion interaction

Permanent result:

- Node Zero stabilizes instead of “punishing” inactivity

### Chapter 7 — Root Archive

Major Gridfall reveal.

Gameplay goals:

- Chronicle/log history
- Past player progress referenced
- World state responds to player specialization

Permanent result:

- Archive district opens

### Chapter 8 — Reboot

Saga climax.

Gameplay goals:

- Complete a substantial personal Campaign or Saga-defined progression gate
- Major system restoration
- High-end VFX/cinematic reward moment

Permanent result:

- Node Zero transforms
- First Saga resolves
- Advanced companion opportunity
- New network layer teased

---

# 5. Shared chapter contract

Although Wild Renewal and Gridfall are different stories, their chapter architecture uses shared capabilities.

Every Chapter definition may include:

- Chapter ID
- World ID
- Content release/version
- Prerequisites
- Opening Story Beats
- Required/optional real-life objective hooks
- Story Nodes
- NPC involvement
- Companion involvement
- World-state mutations
- Reward definition references
- Chronicle trigger
- Completion condition
- Post-completion state

The exact schema is defined later.

---

# 6. Shared system, different story

The same real-life system may receive radically different narrative framing.

Example:

Real feature:

`Focus Session`

Wild Renewal:

`Enter the Quiet Grove`

Gridfall:

`Deep Work: System Trace`

Real feature:

`Campaign Phase Complete`

Wild Renewal:

`A new root takes hold`

Gridfall:

`Deployment stage verified`

Narrative flavor may differ.

Authoritative progression rules do not.

This prevents separate economies for each World.

---

# 7. Content independence rule

Identity Worlds may share:

- System code
- Content authoring format
- Animation framework
- Reward framework
- Quest/Campaign domain model
- Dialogue engine
- Chronicle engine
- World-state engine
- Live Ops tooling

Identity Worlds must not lazily share:

- Main story
- Main NPC cast
- Main companion cast
- Core world conflict
- Major landmarks
- World terminology
- Emotional tone
- Art identity

If players can accurately describe one world as “the other one with different colors,” the content architecture has failed.

---

# 8. Player-generated Campaign framing

Player-generated Campaigns are first-class content.

Pipeline:

1. Player enters real goal.
2. AI or deterministic templates suggest structure.
3. Player approves/edits.
4. Campaign becomes canonical Life OS data.
5. Identity World selects narrative frame.
6. World receives milestones/visual hooks.
7. Completion creates Chronicle/world event.

The player's real goal title remains available privately.

The game may show an identity-specific title publicly or in presentation.

---

# 9. Campaign archetypes

To make player-generated Campaigns feel authored, the narrative layer maps them into archetypes.

Initial archetypes:

- Build / Create
- Learn / Master
- Restore / Organize
- Train / Improve
- Prepare / Plan
- Explore / Travel
- Save / Financial
- Connect / Social
- Maintain / Routine
- Launch / Publish

Each Identity World owns narrative templates per archetype.

Example:

`Build / Create`

Wild Renewal:
- Workshop restoration
- Crafting journey

Gridfall:
- Build pipeline
- Deployment operation

This gives AI a constrained vocabulary rather than inventing lore randomly.

---

# 10. AI narrative boundaries

AI may:

- Rephrase personal Campaigns into identity-specific language
- Generate optional flavor text
- Summarize progress
- Produce non-canon dialogue variants within approved voice constraints
- Suggest side-mission presentation

AI may not:

- Rewrite canon
- Invent permanent factions without authored approval
- Change world-state rules
- Grant items/rewards
- Create unbounded story dependencies
- Contradict completed Saga outcomes

Canon remains authored and versioned.

---

# 11. NPC architecture

NPCs have three layers:

## 11.1 Definition

Authored identity:

- Name
- Role
- Appearance
- Voice/tone
- World
- Story relationships
- Dialogue pools
- animation/emote profile

## 11.2 Story state

Per-player facts:

- Met/not met
- Story chapter state
- Relationship flags
- Optional reputation/affinity

## 11.3 Ambient state

Non-critical presentation:

- Current location
- idle routine
- ambient dialogue set

Ambient presentation can vary without mutating canonical story state.

---

# 12. NPC relationship philosophy

Relationships should feel remembered but not become a spreadsheet.

V1 may use simple milestones:

- Unknown
- Known
- Trusted
- Close Ally

Post-V1 can deepen this.

NPCs may react to:

- Saga progress
- Personal Campaign milestones
- Life Skill emphasis
- Companion choice
- World upgrades

Relationships must not require daily login streaks.

---

# 13. Companion story architecture

Every major companion should have:

- Acquisition context
- Distinct personality
- Core animation/emote set
- Bond milestones
- At least one meaningful personal story beat
- Evolution or presentation milestone where applicable

V1 aspirational companions receive deeper arcs than basic early companions.

Companion story progression cannot be purchased directly.

---

# 14. Companion evolution choice

Where branching evolution exists:

- Choices must be understandable
- No option should be objectively mandatory
- Choice may affect appearance, animation, world interaction, or light gameplay flavor
- Paid cosmetic ownership may decorate forms but does not unlock canonical Bond state

Evolution is a memory of the relationship, not a storefront tier.

---

# 15. Faction architecture

Factions exist primarily to create:

- worldview tension
- replayability
- cosmetics
- optional side arcs
- alternate dialogue
- regional flavor

Faction choices may have consequences but should avoid huge permanent content deletion.

Recommended model:

- Multiple reputations
- Soft alignment
- Temporary tension
- Reconverging main Saga

This keeps choices meaningful without requiring players to create alternate accounts to see half the game.

---

# 16. Story choices

Three classes:

## Flavor choice

Changes dialogue/presentation only.

## Relationship choice

Affects NPC/faction state and later reactions.

## Structural choice

Changes a local story outcome, world detail, or optional branch.

Structural choices are used sparingly.

No V1 choice should accidentally lock the player out of core Life OS capability.

---

# 17. Consequence persistence

When a choice matters, record it explicitly.

Do not infer consequence from old dialogue logs.

Persistent choice data must support:

- Later callbacks
- Chronicle references
- NPC reactions
- World variation
- Content migration

---

# 18. Chronicle integration

Authored story and real-life history intersect through Chronicle.

Chronicle can record:

- Saga completion
- Chapter milestone
- Major personal Campaign completion
- Companion evolution
- Landmark creation
- Important NPC/faction choice
- Major world transformation

Chronicle entries link back to authoritative source events.

---

# 19. Seasonal content

A Season is a bounded release window, not a reset.

A Season may contain:

- Story arc
- World treatment
- New NPC/event character
- Companion/cosmetic content
- Community objective
- Optional commercial tie-in

Permanent account/world progress remains.

---

# 20. Seasonal Archive

When a Season ends:

- Core world reverts/advances to post-event state
- Important story enters Archive
- Later players can replay essential narrative
- Time-limited launch cosmetics/status may remain exclusive where appropriate
- Canon does not become incomprehensible for newcomers

Archived content can be presented as:

- memory
- simulation
- recovered record
- historical tale

depending on Identity World.

---

# 21. Live Events

Live Events are shorter and mechanically lighter than Sagas.

They may be:

- Weekend event
- Community restoration
- Network outage
- Visiting merchant/NPC
- Seasonal weather anomaly
- Discovery event

Live Events must be disableable by FeatureFlag.

No live event should make the base app unusable when backend/event services fail.

---

# 22. Community events

Post-V1 community events may aggregate non-sensitive contribution metrics.

Example:

`Restore the Ancient Bridge`

Contribution sources may include:

- Quest completion
- Focus sessions
- Campaign milestones
- world activity

Raw private task titles are never sent to public community surfaces.

---

# 23. Sponsored narrative content

Sponsored content must remain obviously optional.

Allowed:

- Merchant-themed side Quest
- Limited decoration
- Optional sponsored Adventure
- Campaign tie-in
- Sponsored cosmetic

Not allowed:

- Canon villain defeated only by buying product
- Mandatory purchase to finish Saga
- Sponsored content rewriting personal goals
- Hidden commercial placement disguised as neutral story

Disclosure remains clear even when presentation is thematic.

---

# 24. Content release model

Content is versioned by **ContentRelease**.

A release may contain:

- Story definitions
- Dialogue
- NPC configuration
- Reward references
- World-state definitions
- Event definitions
- Feature/config flags

Benefits:

- Staging
- Rollback
- QA
- Reproducibility
- Compatibility

---

# 25. Binary vs data content

## Data-driven

Should be remotely configurable where practical:

- Text
- Dialogue
- Conditions
- Story graph
- Reward references
- Campaign archetype framing
- NPC schedule metadata
- Event timing
- Feature flags

## Binary/client update required

Likely includes:

- New 3D models
- New animation sets
- Major shaders
- New VFX assets
- New scene geometry
- New gameplay code

Remote content does not become an excuse to download arbitrary executable behavior.

---

# 26. Content authoring tool requirements

Long-term internal authoring tooling must support:

- Create/edit Saga
- Create/edit Chapter
- Create Story Nodes
- Dialogue
- NPC references
- Companion references
- Conditions
- Choices
- World-state changes
- Reward references
- Chronicle triggers
- Localization keys
- Release targeting
- Preview per Identity World
- Validation
- Publish/stage/rollback

The authoring format must be source-controlled.

A visual editor may sit on top later.

---

# 27. Validation rules for authored content

Publishing content should fail validation when:

- Referenced NPC/companion/content ID does not exist
- Chapter prerequisites form impossible cycles
- A required branch has no completion path
- Reward reference is invalid
- Canonical chapter can be skipped unintentionally
- A story node references unsupported client content
- Localization key is missing for required launch locale
- A post-V1 feature is required by V1 content
- Sponsored content becomes mandatory for canon progression

---

# 28. Localization architecture

Narrative text must use localization keys from the beginning.

Do not hard-code story text into C# scene scripts.

Content structures should separate:

- Semantic content ID
- Localized display text
- Voice/caption references where applicable

V1 launch language may begin with English, but the architecture must not require a rewrite to localize later.

---

# 29. Voice acting

Full voice acting is not required for V1.

If voices are introduced:

- Meaningful spoken content requires captions
- Story cannot depend on audio alone
- Voice files are asset references, not embedded logic
- Identity World voice direction stays consistent

Partial/critical-line voice treatment may be explored later.

---

# 30. Story skip/replay

Players should be able to:

- Skip replayable cinematic presentation
- Review important Chronicle/story summaries
- Revisit archived story where supported

Skipping presentation must not duplicate rewards.

Story replay is not the same as re-earning authoritative progression.

---

# 31. New player catch-up

A player joining years later should not face an impossible wall of expired content.

Catch-up principles:

- Foundational Sagas remain available
- Seasonal essential lore enters Archive
- Optional recap summarizes prior world events
- New player can reach current content without purchasing old seasons

The world may acknowledge historical events without requiring the player to have attended them live.

---

# 32. Returning player experience

Returning after months away should provide:

- Concise world recap
- What changed
- Current personal Campaign state
- Companion/NPC reaction where appropriate
- Suggested next Action
- No punishment montage

Return is an opportunity for narrative reconnection.

---

# 33. Content cadence target

Long-term target, subject to team capacity:

## Daily

- Personal Campaign activity
- Dynamic ambient activity
- Deal/offer refresh
- small personalized prompts

## Weekly

- Small side content
- challenges
- NPC/companion moments
- optional event beats

## Monthly

- Meaningful story/content release or event
- companion/cosmetic content
- world variation

## Quarterly

- Major Season / region / Saga portion
- substantial live event
- world expansion

## Annual

- Major expansion
- new World or large system
- major Saga

These are operational targets, not promises embedded into code.

---

# 34. V1 content production budget rule

V1 content must prioritize reusable systems and memorable quality.

For each Founding World:

- One persistent hub
- One complete Saga
- 4–7 significant NPCs
- 3 early companions
- 1 aspirational companion
- Multiple world transformations
- Reusable Campaign archetype framing

Avoid producing dozens of one-use environments before the core loop is proven.

---

# 35. Story/real-life synchronization rule

Canon story gates should usually depend on categories of legitimate player progress, not arbitrary private task content.

Example:

Good:

`Complete one meaningful Campaign phase`

Bad:

`The player must create a task titled "Defeat the Root Daemon"`

The player's real-life language remains theirs.

The story layer adapts around it.

---

# 36. Story privacy rule

NPCs/story systems may know enough to personalize the experience, but private real-life task names should not automatically become:

- public social content
- merchant content
- community event text
- NPC voice lines sent to third parties

Presentation can use generalized category framing unless the player explicitly opts into richer personalization.

---

# 37. Canon ownership

Canon story truth lives in versioned authored content plus explicit player StoryState.

AI output, client cache, analytics, and dialogue logs are not canonical sources of truth.

---

# 38. V1 story acceptance criteria

Before V1 story architecture is considered production-ready:

- Wild Renewal Saga I has a complete beginning/middle/end.
- Gridfall Saga I has a complete beginning/middle/end.
- Both Worlds feel narratively distinct.
- Shared engine capability is clearly separated from identity-specific content.
- Personal Campaign framing works for at least the initial Campaign archetypes.
- Companion story milestones can coexist with main Saga progress.
- Chronicle captures major authored and personal milestones.
- Story choices persist through explicit state.
- Seasonal/Archive architecture does not require resets.
- Sponsored content cannot become mandatory canon.
- Content can be validated/versioned before publication.

---

# 39. Long-term expansion rule

New content must extend one or more of:

- Story
- World
- Character relationship
- Companion relationship
- Personal Campaign expression
- Social/community context
- Real-world adventure

It should not exist merely to add another progress bar.

