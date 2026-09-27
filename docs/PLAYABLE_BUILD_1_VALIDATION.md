# Playable Build #1 — Android Validation

**Milestone:** 4 — Unity client foundation  
**Build:** First World / Android prototype  
**Source:** PR #6, `feat/unity-client-foundation`  
**Rule:** One purposeful build. Do not retry an unchanged failure.

## 1. Record build evidence

Before installing the APK, record:

- Build Automation build ID
- exact commit SHA
- Unity version shown in build logs
- Android target
- EditMode test result
- PlayMode test result
- final artifact name and whether an APK was produced

The pre-export log must contain:

`[Life Played Build Preflight] PASS`

Expected preflight facts:

- Unity 6000.3.25f1
- Android active build target
- Input System backend enabled
- three generated build scenes
- mobile URP assigned
- Wild Renewal content staged
- Development APK
- ARM64-only
- IL2CPP
- IL2CPP Debug C++ compiler configuration

## 2. Launch / authored content

Install and launch the APK.

Pass only if:

- app reaches the Life Played shell without crash
- portrait layout is used
- content diagnostics show **1.0.0 • schema 1**
- diagnostics report Android
- no **CONTENT ERROR** state appears
- Wild Renewal / Hearthwild renders on World

## 3. Primary navigation

Tap each tab:

- Home
- Quests
- World
- Hero
- More

Pass only if:

- every tab responds on first intentional tap
- each non-World tab displays its own route body
- World displays the 3D Hearthwild
- diagnostics show **World OFF** on Home / Quests / Hero / More
- diagnostics show **World ON** on World

## 4. Safe area

Inspect the top and bottom UI around the display cutout/rounded corners.

Pass only if:

- title is not under the cutout/status area
- bottom navigation is fully tappable
- no important label/button is clipped
- 3D rendering can extend behind the cutout while UI remains inside `Screen.safeArea`

## 5. World touch controls

On World:

- drag one finger in the world area
- drag over the bottom navigation/quality-control area
- pinch with two fingers

Pass only if:

- world orbit responds outside the UI control zone
- touching/dragging navigation does not rotate the world
- pinch zooms smoothly
- zoom stays bounded rather than clipping into or flying away from Hearthwild

## 6. Leafglow interaction

Tap **Call Leafglow**.

Pass only if:

- Leafglow Fox reacts immediately
- hop/spin/pulse completes cleanly
- repeated calls do not crash or permanently distort the companion
- no reward/XP/progression is granted; this is presentation-only in Milestone 4

## 7. Graphics tiers

Test all three settings.

### Reduced

Expected:

- target: 30 FPS
- no directional shadows
- hearth point light off
- Bloom Wisps off
- lower render scale

### Standard

Expected:

- target: 30 FPS
- hard shadows
- Bloom Wisps on
- standard hearth light
- 0.90 render scale

### High

Expected:

- target: 60 FPS
- soft shadows
- Bloom Wisps on
- brighter hearth presentation
- full render scale

Pass only if switching tiers does not crash or corrupt the scene.

## 8. Persistence

Select a non-default graphics tier and a non-default tab.

Force-close the app and reopen it.

Pass only if:

- graphics tier restores
- last selected route restores
- app still loads Wild Renewal content correctly

The durable mutation queue/sync-cursor store is primarily verified by EditMode tests in this milestone because the Action UI belongs to Milestone 5.

## 9. Suspend / resume

While on World:

1. send the app to background
2. return to the app

Pass only if:

- app does not crash
- diagnostics indicate paused/suspended state during the lifecycle transition where observable
- world presentation resumes
- camera/companion/world remain coherent
- selected route and graphics tier remain intact

## 10. Performance sanity

This is not the final optimization pass.

Record:

- approximate FPS shown by diagnostics in Reduced
- approximate FPS in Standard
- approximate FPS in High
- visible stutter during orbit/pinch
- device heat if obviously abnormal
- any input latency

A first prototype is not required to hold 60 FPS in High on every phone. It is required to avoid catastrophic stalls, runaway rendering on utility screens, or broken quality switching.

## 11. Failure capture

If the cloud build fails, capture the **first root cause**, not 200 lines of consequences:

- package/import error
- first C# compiler error
- failed EditMode/PlayMode test
- pre-export validation error
- IL2CPP error
- Gradle/Android export error

If the APK builds but runtime fails, record:

- exact action taken
- route/quality tier
- visible diagnostics state
- screenshot or screen recording where possible
- whether it reproduces after relaunch

Do not launch another cloud build until that specific failure is materially changed.

## Merge gate

PR #6 remains draft until:

- cloud build succeeds
- EditMode tests pass
- PlayMode tests pass
- ARM64 APK is produced
- APK installs and launches
- navigation/touch/safe-area checks pass
- content release loads
- quality controls pass
- restart persistence passes
- suspend/resume passes

Only then can Milestone 4 be closed and Milestone 5 begin.
