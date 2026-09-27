# Life Played — Commerce & Rewards Architecture

**Status:** Authoritative commerce/rewards foundation  
**Depends on:** Product Bible, V1 Scope, Core Domain Model, Game Economy, Backend Architecture

---

# 1. Purpose

Life Played may become a meaningful commerce platform, but commerce must remain subordinate to player intent.

The governing rule:

> **Fulfill a real need first. Monetize the match second.**

The system should help the player:

- identify what they actually need
- find relevant merchants/products
- compare offers
- find coupons/promotions
- estimate effective cost
- decide whether to buy now or wait
- receive optional game rewards
- eventually receive merchant-funded cashback/rewards

Developer revenue is a valid ranking input only after player relevance/value thresholds are satisfied.

---

# 2. Commerce layers

Life Played commerce is separated into:

1. Need Graph
2. Offer ingestion/normalization
3. Deal Scout
4. Ranking
5. Affiliate attribution
6. Sponsored Quests
7. Evidence/verification
8. Real-world reward lifecycle
9. Reconciliation/refunds
10. Merchant analytics
11. Future merchant portal

V1 ships only the subset defined in `docs/V1_SCOPE.md`.

---

# 3. V1 commerce boundary

V1 includes a limited Deal Scout / affiliate beta.

Required V1 capabilities:

- explicit/high-confidence Need creation
- offer ingestion from approved providers
- coupons/promotions where supported
- price/effective-cost display
- affiliate deep links
- partner disclosure
- offer freshness/expiry handling
- save/dismiss/already-bought feedback
- basic attribution status
- conversion data when provider returns it

V1 does **not** require:

- stored cashback wallet
- card linking
- continuous background location
- receipt processing at scale
- direct merchant portal
- automated store-visit cash rewards

Those remain architecturally supported but inactive.

---

# 4. Need Graph

A Need is the central personalization primitive.

A Need can originate from:

## Strong sources

- explicit shopping-list item
- explicit player request
- saved product
- Campaign requirement confirmed by player

## Medium sources

- recurring purchase pattern
- accepted AI suggestion
- related task/Quest category

## Weak sources

- offer browsing
- commercial location visit
- category inference

Weak sources should not independently cause aggressive recommendations.

---

# 5. Need states

`ACTIVE → SATISFIED | PAUSED | DISMISSED | EXPIRED`

Important behavior:

- a confirmed purchase can satisfy a Need
- the player can mark Already Bought
- satisfied Needs stop driving primary recommendations
- related accessories require a separate justified Need or explicit player interest

The system must not repeatedly recommend the same item after purchase.

---

# 6. Need confidence

Each Need carries:

- source type
- confidence
- evidence references
- freshness
- optional budget/timing

Example confidence guidance:

- explicit list item: very high
- active Campaign requirement: high
- saved product: high
- recurring timing prediction: medium
- location-only inference: low

Confidence is an interpretable product signal, not a hidden sensitive-profile score.

---

# 7. Sensitive-need exclusion

Need Graph must not automatically infer sensitive personal categories from:

- medical locations
- treatment centers
- religious locations
- political locations
- sexual-health locations
- other sensitive places/purchases

Explicit player-entered tasks can remain private Life OS content without being converted into commerce targeting.

---

# 8. Offer normalization

Provider offers are normalized into internal `Offer` entities.

Normalized attributes may include:

- merchant
- product/category
- title
- offer type
- list/current price
- coupon code
- discount
- cashback/reward
- eligibility terms
- geography
- start/end
- landing/deep link
- provider
- provider offer ID
- tracking metadata
- validation/freshness timestamp
- commission/economic metadata

Raw provider payloads remain adapter data.

---

# 9. Offer freshness

Every offer has a freshness state:

- VERIFIED_CURRENT
- RECENT
- STALE
- EXPIRED
- INVALID

Before high-intent presentation/checkout handoff, refresh where provider APIs support it.

Expired offers are removed from recommendation immediately.

Never present stale cached discounts as guaranteed.

---

# 10. Coupon confidence

Coupon/promo codes must carry provenance.

Priority:

1. merchant-confirmed
2. affiliate-network confirmed
3. recently validated partner source
4. unverified external source

V1 should avoid random scraped-code spam unless separately validated.

The UI distinguishes guaranteed/verified from uncertain codes.

---

# 11. Effective price

Deal Scout compares **effective player cost**.

Conceptual:

`effective_cost = purchase_price - confirmed_discount - confirmed_cashback - confirmed_player_reward`

Potential future components:

- sale price
- coupon
- merchant cashback
- card-linked reward
- Life Played-funded reward

Developer commission is **not** subtracted from player price unless actually shared.

---

# 12. Historical price context

Future/where data permits:

- current price
- recent typical price
- recent low
- promotion history

The product should not treat inflated MSRP strike-through pricing as genuine savings.

When historical data is insufficient, label savings conservatively.

---

# 13. Offer candidate filtering

Before ranking, exclude candidates that:

- do not match active Need
- violate budget hard limit where player set one
- are expired
- are ineligible for region/account
- have unacceptable merchant quality
- lack required disclosure/tracking
- conflict with user brand/category blocks

Profitability may be a minimum eligibility constraint for a commercial surface, but cannot override materially better player value once candidate set is formed.

---

# 14. Ranking model

Conceptual score considers:

- Need relevance
- Need confidence
- player savings
- product fit
- merchant preference
- timing relevance
- distance/travel cost
- offer freshness
- verification confidence
- expected conversion
- return/refund risk
- notification fatigue
- merchant quality
- developer revenue

Recommended policy:

1. Ensure Need relevance.
2. Ensure player-value threshold.
3. Rank by value/fit.
4. Use revenue as secondary/tie-break factor.

No “highest commission wins” rule.

---

# 15. Why am I seeing this?

Every personalized offer must be explainable.

Examples:

- “This monitor is on your Home Office list.”
- “This store is one you previously selected.”
- “Your dog-food Need is approaching its normal refill window.”

Do not reveal hidden/sensitive inference categories.

The player can dismiss or correct the reason.

---

# 16. Player feedback

Offer feedback includes:

- Save
- Hide
- Not interested
- Too expensive
- Already bought
- Do not show this brand
- Do not show this category
- Prefer online
- Prefer nearby

Feedback affects future ranking.

---

# 17. Notification policy

Deal notifications require:

- high-confidence Need
- material value
- appropriate frequency
- player permission

Examples worthy of push:

- saved item crosses target price
- significant verified price drop
- explicit shopping-list item gets unusually strong deal

Ordinary affiliate inventory does not justify push.

---

# 18. Affiliate attribution

Outbound affiliate action receives internal attribution ID.

Conceptual state:

`CLICKED → PROVIDER_PENDING → CONVERTED → APPROVED → PAID`

Alternative:

- REVERSED
- DECLINED
- EXPIRED

Provider event IDs are deduplicated.

Attribution state is separate from player reward state.

---

# 19. Developer revenue

Track separately:

- gross commission
- expected commission
- approved commission
- paid commission
- reversal
- player-funded reward cost if any
- net contribution

Do not mix developer revenue with player game currency.

---

# 20. V1 game reward for affiliate commerce

V1 may award a small themed game reward for a confirmed qualifying partner action only when:

- disclosure is clear
- reward is capped
- reward does not scale directly with purchase price
- ordinary non-sponsored play can earn comparable progression

Large commissions may fund better cosmetics/promotions, not giant XP.

---

# 21. Future Sponsored Quest

A Sponsored Quest is a merchant-funded optional Quest.

Lifecycle:

`AVAILABLE → ACTIVATED → IN_PROGRESS → EVIDENCE_PENDING → QUALIFIED → REWARD_PENDING → REWARD_FINAL`

Alternate states:

- EXPIRED
- FAILED_TERMS
- MANUAL_REVIEW
- REVERSED

No Sponsored Quest is required for canon Saga completion.

---

# 22. Visit verification principle

A geofence entry is a **candidate signal**, not a verified visit.

Potential evidence:

- location freshness
- accuracy radius
- dwell
- venue confidence
- motion plausibility
- foreground check-in
- merchant proof
- purchase evidence

Verification strength scales with reward value.

---

# 23. Drive-by protection

Store proximity without meaningful presence is insufficient for higher-value visit rewards.

Mitigation:

- dwell windows
- venue confidence
- motion history summary
- merchant category-specific rules
- purchase evidence

Gas stations/drive-through/curbside use different rules than a department store.

---

# 24. Dense-location handling

Malls/shared buildings require additional evidence.

A coordinate inside a mall cannot independently prove which merchant was visited.

Possible signals:

- explicit check-in
- merchant transaction
- loyalty confirmation
- short-lived merchant QR
- BLE/NFC future
- venue prediction

---

# 25. Honest-user fallback

If location detection fails:

1. Look for verified purchase/affiliate conversion.
2. Check loyalty/merchant evidence when available.
3. Accept receipt evidence post-V1.
4. Accept late provider/backfill evidence.
5. Manual review only where economically justified.

Missing automatic location is not fraud by itself.

---

# 26. Evidence confidence tiers

Example conceptual tiers:

## Tier A — Strong

- purchase + device integrity + coherent venue/time

May allow fast qualification.

## Tier B — Good

- strong visit evidence + trusted account/device

May allow small visit reward.

## Tier C — Incomplete

- plausible but missing key signal

State remains Pending / request more evidence.

## Tier D — Conflicting

- impossible timing
- reused proof
- simulated location
- invalid transaction

No monetary reward until resolved.

---

# 27. Purchase evidence

Normalized purchase evidence may come from:

- affiliate conversion
- card-linked provider
- merchant POS
- loyalty provider
- receipt
- direct merchant API

Purchase evidence distinguishes:

- authorization
- clearing/settlement
- refund
- partial refund
- chargeback/reversal

---

# 28. Cash reward timing

Game feedback and cash availability are separate.

Example:

```
Purchase detected
  ↓
RPG Quest completion allowed
  ↓
Cashback = Pending
  ↓
Provider settlement
  ↓
Cashback = Final/Available
```

Do not make the player wait days for all game feedback.

Do not make cash final before required settlement/return conditions.

---

# 29. RealWorldReward lifecycle

Canonical states:

- PENDING
- QUALIFIED
- FINAL
- REVERSED
- REJECTED
- EXPIRED
- MANUAL_REVIEW

Each transition is event-backed.

No simple `paid=true` model.

---

# 30. Refund/reversal

A refund creates a settlement/reversal event.

Policies may include:

- full reward reversal
- partial reward adjustment
- no change for non-monetary cosmetic if campaign terms allow

Financial state must remain auditable.

Do not delete original qualification history.

---

# 31. Receipt evidence post-V1

Receipt flow:

`upload → private storage → extract/normalize → duplicate check → qualification → retention cleanup`

Duplicate detection uses structured fields/fingerprints.

Re-encoded/cropped copies should not become new rewards where sufficient matching data exists.

---

# 32. Merchant QR proof post-V1

Direct merchants may issue rotating signed proof.

Token includes conceptually:

- merchant/location
- campaign
- expiry
- nonce
- signature

Static screenshots should not remain valid indefinitely.

---

# 33. Provider webhook processing

Required:

- signature verification
- event-ID dedupe
- replay/timestamp validation where possible
- raw event capture for short debugging window where lawful/necessary
- normalized event creation
- async downstream processing

Never trust an unauthenticated webhook solely because URL is obscure.

---

# 34. Reconciliation

Scheduled jobs reconcile:

- clicks vs conversions
- conversions vs approved commissions
- purchase authorization vs settlement
- rewards vs refunds
- provider totals vs internal totals

Reconciliation is required before real cashback launches.

---

# 35. Provider adapter failover

Multiple providers may cover similar functions.

Architecture supports:

- primary offer provider
- secondary provider
- direct merchant feed
- provider-specific eligibility

Core ranking operates on normalized offers.

Provider outage can remove one source without disabling Deal Scout entirely.

---

# 36. Merchant quality score

Internal merchant quality may consider:

- tracking reliability
- coupon validity
- fulfillment/user feedback
- refund/return rate
- payout reliability
- offer freshness
- support dispute rate

Higher commission does not compensate for consistently bad player outcomes.

---

# 37. Sponsored-game reward design

Sponsored Quest game rewards may include:

- cosmetic
- themed decoration
- collectible
- Chronicle badge
- capped XP/resource
- special animation

Avoid:

- exclusive mandatory power
- huge XP
- Bond purchase
- Saga gates

---

# 38. Purchase-value scaling

Real-world economic reward may scale with merchant-funded purchase value.

Game progression does **not** scale linearly.

Example:

A larger purchase may fund:

- more cashback
- premium themed cosmetic
- unique decoration

but ordinary progression remains capped by the associated Quest class.

---

# 39. Commerce analytics

Aggregate funnel:

- eligible Need
- offer shown
- offer opened
- merchant click
- activation
- visit candidate
- purchase
- approved conversion
- refund
- repeat customer

Merchant analytics should default to aggregate reporting.

Do not expose private task titles.

---

# 40. Merchant portal post-V1

Future merchant campaign creation may include:

- locations
- dates
- budget
- qualifying action
- product/category
- cashback
- game reward theme
- targeting eligibility based on non-sensitive Need categories

All campaigns require platform validation before activation.

Merchants cannot directly query raw user Need Graphs.

---

# 41. Commercial safety controls

Global controls:

- provider kill switch
- merchant kill switch
- campaign pause
- payout pause
- offer expiry override
- maximum reward cap
- maximum campaign spend
- anomaly alerting

A faulty merchant integration must not threaten core gameplay.

---

# 42. Privacy boundary

Commerce providers receive minimum necessary data.

Prefer:

- pseudonymous attribution IDs
- category/offer context
- region where necessary

Avoid:

- complete Chronicle
- private task history
- unrelated location history
- sensitive Need categories

---

# 43. V1 commerce acceptance criteria

Before V1 Deal Scout ships:

- Need creation is explainable.
- player can dismiss/correct Need.
- offers normalize from provider data.
- expired/stale offers are handled.
- affiliate disclosure is clear.
- effective price is honest.
- player value outranks raw commission.
- outbound attribution is idempotent.
- Already Bought stops repeat targeting.
- core game works with all commerce disabled.

---

# 44. Future cash-reward acceptance criteria

Before real cashback/store-visit rewards ship:

- evidence model implemented
- device/app integrity implemented
- duplicate/replay protection tested
- provider webhook verification tested
- settlement/reversal tested
- refund reconciliation tested
- fallback validation tested
- support/admin audit tools exist
- regulatory/store-policy review current
- privacy retention policy active
- payout/accounting reconciliation validated

