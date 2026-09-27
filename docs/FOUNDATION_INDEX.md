# Life Played — Foundation Index

This directory contains the authoritative pre-implementation foundation for Life Played.

## Authority order

When two documents appear to conflict, use this precedence and resolve the lower-level document rather than silently creating competing behavior:

1. **MASTER_PRODUCT_BIBLE.md** — product identity, principles, long-term boundaries
2. **V1_SCOPE.md** — what is and is not required for the first public release
3. **CORE_DOMAIN_MODEL.md** — entities, ownership, lifecycles, authority
4. **GAME_ECONOMY.md** — progression/economy rules
5. **STORY_CONTENT_ARCHITECTURE.md** — narrative/content rules
6. **UNITY_MOBILE_ARCHITECTURE.md** — client/mobile/runtime rules
7. **BACKEND_ARCHITECTURE.md** — server/data/service rules
8. **COMMERCE_REWARDS_ARCHITECTURE.md** — Need Graph, Deal Scout, affiliate/reward rules
9. **PRIVACY_SECURITY_FRAUD.md** — privacy, consent, security, risk, evidence
10. **VALIDATION_STRATEGY.md** — evidence required to claim work complete
11. **IMPLEMENTATION_PLAN.md** — milestone sequence and first vertical slice

`../ACTIVE_TASK.md` is the operational source of truth for the single current task and its status.

## Important locked decisions

- Product: **Life Played** (working name pending formal legal/store/domain clearance)
- Core promise: **Your life builds the world.**
- Platforms: Android + iOS
- Client direction: Unity, stylized 3D
- Rendering direction: URP
- Architecture: offline-first client + server-authoritative valuable state
- Backend direction: ASP.NET Core / modern .NET + PostgreSQL, modular monolith first
- V1 Founding Worlds: **Wild Renewal** and **Gridfall**
- V1 commerce: limited Need Graph / Deal Scout / affiliate beta
- Post-V1: full Nearby real-world adventure map, background/geofence reward automation, card-linked cashback, merchant portal, major social/guild systems
- Unity cloud builds: milestone-gated to protect free-tier quota
- First vertical slice: **Wild Renewal — First Action → First Change**

## Change control

Foundation documents are not immutable forever.

A future change must:

1. identify the affected authoritative document
2. explain why evidence/requirements changed
3. update dependent documents in the same task when necessary
4. update tests/implementation contracts where applicable
5. avoid leaving an older contradictory rule active

Do not create a second competing specification in a random note or issue.
