# Life Played — Privacy, Security & Fraud Model

**Status:** Authoritative privacy/security/fraud foundation  
**Policy review snapshot:** 2026-09-27  
**Depends on:** Product Bible, V1 Scope, Core Domain Model, Backend Architecture, Commerce & Rewards Architecture

---

# 1. Security/privacy goals

Life Played handles unusually sensitive combinations of:

- private goals/tasks
- behavioral history
- optional location
- optional purchase/commercial data
- account identity
- game economy
- future cashback/rewards

Therefore the product must be designed so trust is structural, not marketing copy.

Core rules:

1. Collect the minimum data needed for a user-facing feature.
2. Keep private Life OS content private by default.
3. Use foreground/transactional location where practical.
4. Do not build a business around selling raw device location.
5. Separate high-sensitivity evidence from ordinary gameplay/profile data.
6. Never trust client-side fraud/security decisions.
7. Missing evidence is not automatically fraud.
8. High-value rewards require stronger evidence than low-value game feedback.
9. Every restriction/reward decision should have reason codes.
10. Players must have understandable controls and deletion/account-management paths.

---

# 2. Data classification

## Class A — Public/player-shared

Examples:

- chosen public display name
- explicitly shared achievement
- opted-in public world/character showcase

## Class B — Private product data

Examples:

- Actions
- Quests
- Campaigns
- Chronicle
- character/world state
- companion state
- preferences

Default: private.

## Class C — Sensitive commerce data

Examples:

- Needs
- purchase evidence
- affiliate conversion state
- merchant preference
- budgets

Access narrower than ordinary gameplay data.

## Class D — High-sensitivity evidence/security

Examples:

- precise location evidence
- receipt images
- device integrity
- fraud reason details
- raw provider evidence payload
- security audit records

Strict access and retention controls.

## Class E — Secrets/credentials

Examples:

- provider API keys
- signing keys
- database credentials
- store credentials

Never exposed to mobile client or logs.

---

# 3. Privacy-by-default rules

Default behavior:

- tasks/goal text is private
- Chronicle is private
- Need Graph is private
- location is off until a feature needs it and user permits it
- commercial personalization can be controlled
- social sharing is explicit
- merchants never receive raw task history
- merchants never receive raw Chronicle
- merchants never query a user's Need Graph directly

---

# 4. Consent model

Consent records are purpose-specific.

Possible purposes:

- product analytics
- commerce personalization
- foreground location feature
- future background location feature
- marketing communications
- cross-company tracking where applicable

Consent must be:

- informed
- explicit where required
- revocable
- recorded with policy/version
- separate from OS permission state

An OS permission grant is not automatically consent for unrelated secondary use.

---

# 5. Location policy

V1:

- no continuous background location dependency
- use foreground/on-demand access for player-facing functionality
- request only precision actually needed
- core Life OS/game works without location

Post-V1 background/geofence features require:

- clear user-facing core benefit
- current Google/Apple policy review
- prominent disclosure
- platform permission workflow
- alternative behavior when denied/unavailable
- retention/minimization plan

Location data may not be sold as raw device-location inventory.

---

# 6. Precise vs coarse location

Use coarse location where sufficient.

Precise location requires a concrete feature reason, such as:

- user-initiated nearby merchant discovery requiring store-level accuracy
- explicit visit validation where coarse location cannot establish venue

Do not request precise access globally because one future feature might use it.

---

# 7. Location retention

Prefer storing normalized evidence:

- venue candidate
- timestamp
- confidence
- accuracy
- dwell summary

rather than continuous raw trails.

If raw points are temporarily required for fraud/verification:

- retention is short
- storage encrypted
- access restricted
- normalization occurs promptly
- raw data is deleted when no longer necessary

Exact durations are finalized before feature launch.

---

# 8. Sensitive-place exclusion

Location/commerce systems must not automatically classify or monetize visits that reveal sensitive categories.

Examples:

- medical/treatment
- religion
- political activity
- sexual-health services
- addiction treatment
- other sensitive personal categories

Such places should be excluded from commercial targeting pipelines.

---

# 9. Private goal exclusion

Private task text is not automatically fed to:

- advertiser targeting
- merchant analytics
- social feeds
- external AI beyond what is necessary for an explicitly requested AI feature

AI context should be minimized to the request.

---

# 10. AI privacy boundary

AI Planning requests:

- send minimum required context
- omit unrelated account history
- exclude commerce evidence unless needed
- exclude precise location unless feature explicitly requires it
- avoid secrets/payment data

Provider retention/training terms must be reviewed before production selection.

AI-generated output is validated before becoming canonical work.

---

# 11. App integrity — Android

Android sensitive/reward endpoints may use Play Integrity signals.

Potential signals:

- app recognition
- licensing/acquisition context
- device integrity
- optional recent activity/device labels

Server evaluates verdicts.

Client cannot self-report “integrity passed.”

Integrity uses tiered enforcement; one missing/unevaluated signal is not automatically a ban.

---

# 12. App integrity — iOS

Sensitive/reward endpoints may use Apple App Attest / DeviceCheck.

Pattern:

1. server issues challenge
2. legitimate app instance produces attestation/assertion
3. server validates
4. server binds verified key/context to account/device risk state

Sensitive decisions happen server-side.

---

# 13. Integrity fallback

Attestation can fail for legitimate reasons.

Possible outcomes:

- normal access for low-risk Life OS
- retry
- reduced reward trust
- require stronger purchase evidence
- manual review for high-value case

Do not lock a player out of their private task list merely because attestation is temporarily unavailable.

---

# 14. Authentication security

Required:

- standards-based authentication
- short-lived access tokens where architecture supports
- secure refresh/session handling
- revocable sessions
- server-side account authorization
- rate-limited auth endpoints
- account recovery
- account deletion

Do not use device ID as authentication.

---

# 15. Authorization

Every private resource check confirms authenticated Account ownership.

Do not accept `account_id` from request body as authorization proof.

Privileged support/admin operations require:

- role authorization
- audit logging
- reason
- least privilege

---

# 16. Local device storage

Sensitive tokens stored using platform secure storage/keystore/keychain.

Local database:

- protected by platform sandbox
- encrypt sensitive subsets where justified
- never stores backend secrets

Precise-location/raw-receipt evidence should not remain casually in normal cache.

---

# 17. Transport security

All production network traffic uses TLS.

No sensitive production endpoint supports cleartext HTTP.

Certificate/platform hardening decisions are revisited during implementation.

---

# 18. Secrets

Secrets never committed.

Production secrets:

- managed outside repository
- scoped per environment
- rotated
- auditable

Provider keys with client-public roles are treated differently from server secrets, but exposure is still minimized.

---

# 19. Logging policy

Never log by default:

- full auth tokens
- passwords
- precise raw GPS trails
- full receipt/payment payloads
- private task contents unless necessary for explicit debug and protected
- API secrets

Use IDs/reason codes.

Sensitive diagnostic logging must be time-limited and controlled.

---

# 20. Fraud philosophy

Fraud controls protect:

- game economy
- merchants
- developer revenue
- future cashback funds
- legitimate players

Fraud detection must not become “anything unusual is guilty.”

The system distinguishes:

- absence of proof
- technical failure
- suspicious behavior
- strong contradictory evidence

---

# 21. Risk tiers

Conceptual account/event risk:

## Low

Normal ordinary gameplay.

## Medium

Unusual patterns; additional verification for valuable rewards.

## High

Strong conflicting/integrity/reuse indicators.

## Restricted

Repeated confirmed abuse of real-world reward system.

Restriction may target commerce/rewards while leaving private Life OS data accessible unless account security itself is compromised.

---

# 22. Evidence trust tiers

## Strong

Examples:

- verified settled transaction
- authenticated merchant proof
- signed provider confirmation
- strong device integrity + coherent evidence

## Moderate

Examples:

- plausible foreground location + dwell + venue match
- affiliate conversion pending approval

## Weak

Examples:

- single stale location point
- user claim without external support

High-value real-money rewards never rely solely on weak evidence.

---

# 23. Location spoofing defenses

Potential checks:

- Android mock-location indicators
- iOS simulation source indicators where available
- Play Integrity/App Attest
- impossible travel
- movement plausibility
- accuracy/freshness
- account/device history
- venue/purchase agreement

No single spoof flag is treated as perfect.

---

# 24. Replay defenses

Use:

- idempotency keys
- server nonce/challenge
- webhook event IDs
- attestation counters/challenges where applicable
- timestamp windows
- duplicate transaction/receipt fingerprints

Replayed requests return prior result or reject safely.

---

# 25. Multi-account abuse

Potential signals:

- repeated evidence across accounts
- shared transaction IDs
- repeated receipt fingerprint
- impossible device/account patterns
- referral-ring patterns post-V1

Do not use household/device sharing as automatic guilt.

Shared family devices exist.

---

# 26. Reward velocity

Monitor:

- XP per time
- duplicate Actions
- Sponsored Quest claims
- cashback volume
- conversion reuse
- receipt reuse
- device request activity

Velocity alerts trigger review/risk elevation, not automatic permanent ban alone.

---

# 27. Device trust

Device trust is contextual.

Do not maintain a simplistic forever flag:

`trusted_device = true`

Instead maintain evidence/history:

- integrity results
- recent failures
- account relationships
- abuse history
- last successful attestation

Risk can recover over time.

---

# 28. False-positive protection

Before denying a valuable legitimate claim, attempt available corroboration:

- provider transaction
- affiliate conversion
- receipt
- loyalty record
- late location backfill
- merchant proof

If evidence is incomplete:

`PENDING`

is preferable to false `REJECTED`.

---

# 29. Manual review

Manual review is reserved for:

- high-value disputes
- contradictory evidence
- provider mismatch
- suspected fraud with material impact

Reviewer sees only data necessary for decision.

All decisions audited.

Manual review is not scalable enough to be normal claim flow.

---

# 30. User-facing fraud messages

Do not expose detailed exploit-detection logic.

Good:

“Your reward is still being verified. We need one more confirmation before it becomes available.”

Avoid:

“We detected rule X because your velocity threshold was 3.7.”

Support can provide meaningful resolution without publishing anti-fraud thresholds.

---

# 31. Restrictions

Possible graduated enforcement:

- additional verification
- real-world reward delay
- Sponsored Quest restriction
- cashback restriction
- affiliate reward restriction
- account security lock for compromised account
- full suspension only for serious/repeated violation

Do not erase private productivity history as a punishment mechanism.

---

# 32. Appeal/support

For meaningful financial/reward restrictions, support must be able to:

- locate decision
- see reason code
- see evidence references
- correct error
- issue explicit adjustment/reversal

Support cannot directly edit balances without audit event.

---

# 33. Account deletion

Account deletion flow must:

- revoke sessions
- stop future processing
- delete/anonymize ordinary private data according to policy
- handle legally required financial/security retention separately
- disclose retention exceptions clearly

Deletion is a workflow, not one cascading SQL statement.

---

# 34. Data export

Architecture should support user data export for:

- account/profile
- Life OS data
- Chronicle
- world/progression data
- relevant purchase/entitlement records

Exact regulatory requirements are reviewed before launch.

---

# 35. Retention categories

Retention schedules are defined by data purpose.

Examples:

- ordinary private task data: account lifetime/deletion policy
- telemetry: bounded analytics window
- raw location evidence: short verification/fraud window
- normalized reward evidence: longer where dispute/accounting requires
- financial settlement: statutory/business requirement
- security audit: defined security window

No “keep everything forever just in case.”

---

# 36. Third-party SDK governance

Before adding SDK:

- purpose documented
- data collected documented
- privacy/store declarations reviewed
- network endpoints understood
- update policy
- security reputation
- removal path

Avoid SDK sprawl.

A coupon SDK does not get free access to location because it asked nicely.

---

# 37. Platform disclosure

Store privacy declarations must match runtime behavior.

Changes to:

- data collection
- tracking
- location
- third-party SDKs
- commerce

trigger a privacy/store metadata review.

---

# 38. Cross-company tracking

If future ad/affiliate behavior qualifies as cross-company tracking on iOS or comparable platform policy:

- implement required platform consent
- degrade gracefully when declined
- core gameplay remains available

Do not condition ordinary game access on consent to unrelated tracking.

---

# 39. Analytics minimization

Prefer event metadata like:

- feature
- success/failure
- duration
- category

over full private content.

Example:

Good:
`campaign_completed category=build duration_bucket=weeks`

Not:
`campaign_title="File for divorce"`

unless explicit product function genuinely requires the text.

---

# 40. Security incident readiness

Before production launch:

- security contact
- credential rotation procedure
- incident severity levels
- account-session revocation mechanism
- provider credential revocation
- log preservation policy
- user notification process where required

---

# 41. Dependency security

Required:

- pinned/locked package versions where practical
- dependency scanning
- review native Unity plugins carefully
- no abandoned binary SDK without source/provenance
- SBOM/dependency inventory where feasible

---

# 42. Abuse-safe content inputs

User-entered text/AI content:

- treated as data, not executable instructions
- safely rendered
- no untrusted HTML/script execution
- URLs handled safely
- content size limits

Remote content definitions cannot execute arbitrary code.

---

# 43. Economy exploit separation

Ordinary game-economy abuse and financial fraud are different risk classes.

A player spamming tiny Actions should receive diminishing XP.

That does not automatically label them a financial fraudster.

A reused merchant transaction is a different issue with stronger evidence requirements.

---

# 44. V1 security/privacy acceptance criteria

Before V1:

- auth/session model reviewed
- authorization tests pass
- private data defaults private
- no continuous background location dependency
- secrets absent from repository/client
- AI provider credentials server-only
- commerce can be fully disabled
- account deletion path exists
- privacy controls exist
- logs avoid sensitive payloads
- dependency scan passes
- server-authoritative economy verified
- duplicate/replay tests pass

---

# 45. Future location/cashback launch gate

Before enabling automatic visit/cash rewards:

- current Google/Apple policy review completed
- location permission UX reviewed
- data-retention schedule approved
- Play Integrity/App Attest paths implemented
- spoof/replay/duplicate controls implemented
- provider signatures verified
- reconciliation tested
- manual-review/support path exists
- refund/reversal tested
- privacy disclosures/store labels updated
- independent security review considered for high-value flows

