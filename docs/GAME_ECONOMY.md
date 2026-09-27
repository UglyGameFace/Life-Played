# Life Played — Game Economy & Progression Specification

**Status:** Authoritative foundation, tunable by versioned balance data  
**Depends on:** `docs/MASTER_PRODUCT_BIBLE.md`, `docs/V1_SCOPE.md`, `docs/CORE_DOMAIN_MODEL.md`

---

# 1. Economy goals

The Life Played economy must:

1. Reward meaningful real-life progress.
2. Feel generous early without becoming trivial long-term.
3. Avoid punishing people for becoming more efficient.
4. Avoid rewarding spam, duplicate microtasks, fake timers, or spending money.
5. Make major accomplishments feel materially different from trivial Actions.
6. Keep the number of visible currencies low.
7. Allow future balancing through versioned rules.
8. Preserve the meaning of progression across Identity Worlds.
9. Avoid unhealthy streak/overwork incentives.
10. Keep monetary rewards and gameplay currency completely separate.

---

# 2. Economy layers

Life Played uses distinct progression layers:

- Account XP / Account Level
- Life Skill XP / Skill Levels
- Momentum
- World Progress
- Companion Bond
- Achievements / Chronicle
- One primary gameplay currency in V1
- Limited Identity World materials where necessary
- Real-money/cashback state, completely separate from game currency

No single layer should replace the others.

---

# 3. Account level curve

Account Level represents broad long-term progression.

For V1 balance, the **XP required for the next level** uses:

`next_level_xp(L) = round(100 + 35 × (L - 1)^1.35)`

where `L` is the current account level.

Approximate pacing:

| Current level | XP to next level |
|---:|---:|
| 1 | 100 |
| 5 | ~327 |
| 10 | ~779 |
| 25 | ~2,645 |
| 50 | ~6,775 |
| 100 | ~16,700 |

These values are initial balance targets, not hard-coded constants.

They must live in versioned balance configuration.

No meaningful productivity feature should require an extreme account level.

High levels primarily represent history, prestige, world/story unlock conditions, and cosmetic status.

---

# 4. Life Skill curve

Each Life Skill levels independently.

Initial next-level target:

`skill_next_xp(L) = round(60 + 20 × (L - 1)^1.25)`

Skills level faster than the overall Account early, then gradually require sustained activity.

Life Skill progression may unlock:

- World presentation
- Building stages
- Story gates
- Cosmetics
- Companion interactions
- Optional game-side abilities

It must not lock basic task/project functionality.

---

# 5. Initial Life Skill taxonomy

V1 foundation uses these eight skill families unless later usability work demonstrates a strong reason to merge/split them:

- **Focus** — sustained attention, concentrated work, focus sessions
- **Knowledge** — learning, studying, research
- **Fitness** — exercise and physical activity
- **Creativity** — art, writing, ideation, design
- **Craft** — building, making, coding, technical execution, hands-on creation
- **Finance** — budgeting, saving, financial organization
- **Social** — relationships, communication, community activity
- **Home** — cleaning, errands, household organization, domestic maintenance

An Action may contribute to more than one skill.

A completion should normally affect no more than **three** Life Skills to avoid meaningless “everything levels everything” behavior.

Skill weights for an Action sum to 1.0.

---

# 6. Action Effort Score

Rewards originate from a deterministic **Effort Score** rather than player-entered difficulty alone.

## 6.1 Duration component

Initial duration component:

`duration_score = 20 × ln(1 + expected_minutes / 10)`

Expected behavior:

| Expected effort | Duration score |
|---:|---:|
| 5 min | ~8 |
| 15 min | ~18 |
| 30 min | ~28 |
| 60 min | ~39 |
| 120 min | ~51 |
| 240 min | ~64 |

Duration alone cannot create unlimited reward value.

Expected duration is derived from player input plus normalized task/campaign context.

Actual timer duration is evidence, not a direct multiplier.

Leaving a timer running does not manufacture XP.

---

# 7. Effort modifiers

The initial conceptual reward formula is:

`effort_score = duration_score × complexity × campaign × repetition × integrity`

## Complexity multiplier

Target range:

`0.80 – 1.30`

Inputs may include:

- Number of meaningful steps
- Dependency depth
- Cognitive/physical complexity category
- Campaign context
- Historical task template

The player may suggest difficulty, but cannot directly choose the multiplier.

## Campaign multiplier

Target range:

`1.00 – 1.20`

Campaign-linked Actions may receive a modest bonus because structured long-term progress is desirable.

The multiplier remains small to prevent “put every task in a campaign” farming.

## Repetition multiplier

Target range:

`0.10 – 1.00`

Normal scheduled HabitOccurrences at their intended cadence are not treated as spam.

Unscheduled duplicate-like manual Actions rapidly diminish.

## Integrity multiplier

Normally `1.0`.

It is not a secret “punishment score.”

For ordinary Life OS activity, integrity mostly determines whether authoritative reward is granted.

High-risk real-world monetary events use the separate Evidence/Reward domain rather than silently altering ordinary XP.

---

# 8. Base reward conversion

Initial V1 target:

`account_xp = round(clamp(effort_score, 5, 120))`

Typical ranges:

| Activity class | Typical account XP |
|---|---:|
| Tiny Action | 5–15 |
| Short Action | 15–30 |
| Standard Action | 30–55 |
| Significant Action | 55–85 |
| Major Action | 85–120 |
| Quest completion bonus | 25–150 |
| Campaign phase completion | 75–250 |
| Major Campaign completion | 250–750 |

Campaign completion rewards are separate milestone grants and are not simply the sum of inflated child rewards.

These are initial tuning targets.

---

# 9. Skill XP allocation

For a normal Action:

`total_skill_xp = round(account_xp × 0.75)`

This pool is distributed by skill weights.

Example:

A 60 XP coding Action:

- Craft weight 0.55
- Focus weight 0.30
- Knowledge weight 0.15

Produces approximately:

- Craft +25
- Focus +14
- Knowledge +7

Skill XP does not need to equal account XP.

This keeps Account Level and specialization meaningfully different.

---

# 10. Efficiency protection

Life Played must not reduce rewards merely because a player gets better at something.

Example:

A task historically requiring 60 minutes begins taking 25 minutes because the player learned the skill.

The expected effort model may preserve much of the prior task-template value rather than immediately collapsing reward.

Important distinction:

- **Efficiency improvement:** desirable.
- **False effort inflation:** exploitable.

Historical task templates and completion patterns can help distinguish them.

Actual elapsed time is never the only measure.

---

# 11. Duplicate / microtask anti-farming

No global daily XP cap should punish a genuinely productive day.

Instead, diminishing returns apply to suspiciously duplicate reward sources.

A duplicate cluster may be identified by:

- Same recurrence/template
- Normalized title/content similarity
- Same parent context
- Same category
- Completion timing
- Repeated creation/completion patterns

Initial unscheduled duplicate multiplier sequence within a short window:

1. first equivalent Action: `1.00`
2. second: `0.50`
3. third: `0.25`
4. later equivalent completions: floor near `0.10`

Scheduled HabitOccurrences at legitimate cadence are exempt from this duplicate sequence.

Example:

Legitimate:

- Morning medication occurrence
- Evening medication occurrence if intentionally scheduled

Not legitimate:

- “Clean desk 1”
- “Clean desk 2”
- “Clean desk 3”
- “Clean desk 4”

created solely to multiply rewards.

---

# 12. Task-splitting protection

Breaking a large project into useful Actions is encouraged.

Breaking one Action into meaningless fragments purely for reward is not.

Therefore:

- Child Action rewards are evaluated normally.
- Parent Quest/Campaign completion bonuses consider already rewarded child work.
- Total milestone value is bounded.
- Creating more child rows does not linearly multiply the milestone bonus.

The player should be rewarded for planning, not database row count.

---

# 13. Habit economy

Habits receive rewards from scheduled HabitOccurrences.

Reward value considers:

- Intended frequency
- Expected effort
- Skill mapping
- Consistency
- Whether the occurrence is valid for the schedule

Repeated taps do not create new occurrences.

Missing an occurrence does not remove previously earned XP.

Planned rest can neutralize expected cadence where appropriate.

---

# 14. Focus economy

Focus sessions reward genuine focused effort without encouraging pathological timer farming.

Initial principles:

- First 4 hours of qualified focused time per local day earn normal Focus-related progression.
- Hours 4–8 remain trackable but receive reduced incremental XP.
- Beyond 8 rewardable hours, focus tracking may continue for records, but no additional grind bonus should encourage unhealthy overwork.

Exact taper is configurable.

Suggested V1 taper:

- 0–4h: 100% focus reward rate
- 4–8h: 50%
- 8h+: 0% incremental focus-time reward

Completing actual associated Actions may still award their normal reward.

This avoids telling a legitimate worker their work “doesn't count” while refusing to gamify extreme hours.

---

# 15. Momentum

Momentum measures recent consistency, not an unbroken streak.

Range:

`0–100`

New accounts begin around a neutral midpoint rather than zero.

Momentum is derived from recent planned-vs-completed activity using a rolling weighted window.

Conceptual input:

- Did the player make progress on what they planned?
- Were planned Rest Days respected?
- Did the player recover after disruption?
- Was the plan itself realistic?

Momentum must not reward creating hundreds of trivial tasks.

## Planned Rest Day

A planned Rest Day is **neutral**, not failure.

## Missed day

A missed unplanned day causes modest decay, never catastrophic reset.

## Returning

Returning after inactivity can recover Momentum progressively.

---

# 16. Momentum benefits

Momentum benefits remain modest.

V1 allowed benefits:

- Small capped account XP bonus, maximum target **5%**
- Cosmetic world ambience
- Minor exploration/world bonuses
- Special non-economic encounters
- Celebration/status presentation

Momentum must not become a compounding advantage where high-Momentum players permanently outpace returning players.

---

# 17. Quest completion bonuses

A Quest may grant a completion bonus based on:

- Number of meaningful completed Actions
- Quest duration/span
- Difficulty class
- Campaign significance
- Whether child Actions already generated rewards

Quest bonus is capped to prevent nested-structure farming.

Initial target bands:

- Small Quest: 25–50 XP bonus
- Standard Quest: 50–90
- Major Quest: 90–150

---

# 18. Campaign milestone rewards

Campaigns create the largest meaningful non-commercial progression moments.

Suggested target bands:

- Phase completion: 75–250 account XP
- Campaign completion: 250–750 account XP
- World transformation
- Chronicle entry
- Achievement/title where appropriate
- Companion/world reward where appropriate

Very long campaigns should obtain value through multiple legitimate phases, not one gigantic reward multiplier.

---

# 19. Achievements

Achievements are primarily:

- Recognition
- Chronicle/history
- Cosmetic/world unlock
- Modest one-time reward

Achievements must not become infinitely repeatable reward faucets unless explicitly designed as capped seasonal systems.

---

# 20. V1 gameplay currency

V1 uses **one primary universal gameplay currency**.

The final player-facing name is not locked in this document.

Properties:

- Earned through normal game progression
- Spent on ordinary game-side customization/crafting/upgrades
- Cannot be withdrawn for cash
- Cannot be converted to cashback
- Does not purchase real-world products
- Does not bypass real-life achievement gates

V1 should avoid a premium gem currency unless there is a concrete store-design requirement.

Direct store purchases are preferable to inventing another currency solely because other games do it.

---

# 21. Identity World materials

Each V1 Founding World may use **no more than two core world materials** unless design testing proves more are necessary.

Materials exist for:

- Building upgrades
- World restoration
- Story crafting

Materials are not allowed to create five overlapping currencies with different icons and identical behavior.

---

# 22. Currency earning

Initial ordinary currency grants scale by reward tier, not raw dollars or actual elapsed time.

Suggested target:

| Reward class | Primary currency |
|---|---:|
| Tiny | 1–2 |
| Short | 2–4 |
| Standard | 4–8 |
| Significant | 8–12 |
| Major | 12–18 |
| Quest completion | 10–30 |
| Campaign milestone | 20–60 |

Exact values remain balance-configurable.

---

# 23. Spending sinks

Healthy V1 sinks may include:

- Character cosmetics earned/unlocked through gameplay
- Companion accessories
- World decorations
- Non-authoritative crafting
- Building cosmetic variants
- Optional convenience within game presentation

Do not require currency to create or complete real-life tasks.

The Life OS remains functional independent of grind.

---

# 24. Building progression

A building upgrade may require a combination of:

- Relevant Life Skill threshold/progress
- Story/Campaign milestone
- Earned gameplay currency/material
- Specific Achievement

Currency alone cannot buy past the real-life progression requirement.

Example:

A Gridfall Dev Lab Tier III might require:

- Craft skill milestone
- Complete a major project Campaign
- Earned world material

not merely:

- Pay 5,000 currency

---

# 25. Companion Bond

Companion Bond represents relationship/history, not purchased power.

Bond may increase through:

- Meaningful Action/Quest progress
- Campaign milestones
- Story interactions
- Focus milestones
- Companion-specific events

Low-value repeated Actions have capped Bond contribution.

Purchasing products or cosmetics does not directly buy Bond.

A merchant quest may unlock a themed cosmetic or event, but does not purchase emotional progression.

---

# 26. Companion evolution

Evolution milestones require combinations such as:

- Bond threshold
- Story progress
- Meaningful Achievement
- Choice

Evolution must not require cash purchase.

Paid cosmetics may change appearance without changing authoritative Bond/evolution power.

---

# 27. World progression

World progression is a synthesis of:

- Real-life progression
- Campaigns
- Life Skills
- Story
- Achievements
- Earned resources

It must not be reducible to one spendable meter.

This preserves the feeling that the world reflects the player's actual history.

---

# 28. Reward rarity

V1 may use presentation tiers such as:

- Common
- Uncommon
- Rare
- Epic
- Legendary
- Mythic

Rarity represents scarcity/presentation, not guaranteed gameplay dominance.

Higher rarity may mean:

- More distinctive model
- VFX
- Animation
- Story significance
- Cosmetic prestige

Avoid massive stat inflation.

---

# 29. Random rewards

V1 may use earned random reward moments only if:

- They are not purchased as loot boxes.
- They do not contain essential progression.
- Odds/pools are controlled server-side.
- Duplicate cosmetics are prevented or converted fairly.
- A player cannot spend real money to repeatedly roll for power.

Prefer deterministic milestone rewards for important achievements.

---

# 30. Purchase / commerce game rewards

Real-world purchases are not a shortcut to account power.

A qualifying merchant purchase may grant:

- Themed cosmetic
- Merchant/event collectible
- Chronicle entry
- Limited world decoration
- Capped ordinary XP equivalent
- Capped world resource bonus

The XP portion is capped based on the quest/action class, **not purchase price**.

A $1,500 laptop does not grant 75× the XP of a $20 purchase.

The real-money side may scale with merchant economics; the game-power side does not.

---

# 31. Sponsored Quest reward cap

For V1/post-V1 design, a Sponsored Quest's ordinary progression reward should not exceed what a comparable meaningful non-sponsored Quest could earn.

This ensures:

`paying / shopping ≠ fastest leveling method`

Sponsored content competes on:

- Real savings
- Unique cosmetics
- Themed experiences
- Merchant-funded rewards

not overwhelming XP.

---

# 32. Premium monetization boundaries

Allowed monetization may include:

- Subscription features
- Additional AI allowance
- Cosmetic packs
- World cosmetic themes
- Companion cosmetics
- Premium story/expansion content

Not allowed as core model:

- Buying account levels
- Buying Life Skill XP
- Buying Momentum
- Buying Companion Bond
- Buying Campaign completion
- Paying to skip real-life requirements for meaningful world progression

---

# 33. Economy rule versioning

Every authoritative RewardGrant references:

- reward rule version
- source event
- idempotency key

Balance changes do not silently reinterpret previously granted rewards.

If retroactive corrections are ever necessary, use explicit adjustment events.

---

# 34. Economy observability

The backend must track aggregate metrics such as:

- XP earned by source type
- Skill XP distribution
- Duplicate/diminishing-return frequency
- Currency creation
- Currency spending
- Currency inflation
- Building upgrade pacing
- Companion Bond pacing
- Campaign completion rewards
- Sponsored vs non-sponsored progression
- Anomalous reward velocity

Monitoring must distinguish balance problems from fraud.

---

# 35. Abuse signals

Potential economy-abuse indicators:

- Extreme duplicate Action creation
- Impossible completion velocity
- Repeated reopen/complete cycles
- Repeated timer manipulation patterns
- Many nearly identical Campaigns
- Client event replay
- Cross-account receipt/transaction reuse
- Suspicious device/account clusters

A single unusual productive day is not itself fraud.

Enforcement belongs in the fraud/risk specification.

---

# 36. Player-facing reward clarity

The player should understand why a completion mattered.

Example:

`Quest complete`

- Account XP +60
- Craft +25
- Focus +14
- Dev Lab +3%

Avoid dumping eight currencies and fifteen microscopic numbers onto every completion.

The UI may summarize secondary grants.

---

# 37. Early-game pacing

The first session should produce visible progression quickly.

Targets:

- First meaningful Action can create an immediate world reaction.
- First Account Level should be reachable quickly.
- First building/area change should occur during onboarding or very early play.
- First Companion Bond feedback should occur early.
- First story unlock should occur in the first session.

Early generosity is presentation/progression pacing, not permanent inflation.

---

# 38. Long-term pacing

Progression cadence should operate at multiple scales:

## Minutes

- Action feedback
- XP
- Small world response

## Days

- Momentum
- Habit development
- Building progress
- Companion Bond

## Weeks

- Quest/Campaign completion
- Major upgrades
- Story chapters

## Months

- Major Campaigns
- Saga completion
- Landmarks
- Significant companion/world transformations

## Years

- Chronicle depth
- Multiple Identity Worlds
- Legacy progression
- Long-running collection/history

---

# 39. Rebalance policy

Balance data must be remotely versioned.

Rebalance may adjust:

- XP curves
- effort mappings
- reward bands
- currency grants
- building costs
- Momentum behavior
- companion pacing

Rules:

- Do not arbitrarily confiscate legitimate historical progress.
- Migration must be explicit when derived levels change.
- Financial rewards are governed by their own immutable settlement records.
- Major balance changes require regression tests.

---

# 40. V1 economy acceptance criteria

Before V1 economy can be considered implementation-ready:

- Reward formula produces sensible values for tiny, normal, major, and Campaign work.
- Duplicate task spam does not outperform legitimate work.
- Legitimate task decomposition remains worthwhile.
- Efficiency improvement is not punished.
- Momentum survives missed days and planned rest.
- Spending money is not the fastest route to XP/skills/world power.
- Focus rewards do not encourage extreme hours.
- Currency has enough sinks to avoid obvious inflation.
- Companion Bond cannot be bought.
- World progression requires more than currency.
- Reward events are deterministic/idempotent under the same rule version.
- Economy telemetry can detect inflation and abnormal reward velocity.

