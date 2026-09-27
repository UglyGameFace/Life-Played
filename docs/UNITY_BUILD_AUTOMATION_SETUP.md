# Life Played — Unity Build Automation Setup

**Status:** Milestone 4 validation configuration  
**Purpose:** Run the first purposeful Android validation build without enabling commit-by-commit Unity builds.

## Build target

Configure one temporary validation target in Unity Dashboard:

- **Target name:** `Life Played - Android Prototype`
- **Platform:** Android
- **Branch:** `feat/unity-client-foundation`
- **Project subfolder path:** `client/LifePlayed.Unity`
- **Unity version:** Auto-detect from `ProjectSettings/ProjectVersion.txt`
- Expected editor: **6000.3.25f1**
- **Builder OS:** Windows 11 24H2
- **Machine:** Micro where available
- **Auto-build:** OFF
- **Scheduled builds:** OFF
- **Development Build:** ON
- **Autoconnect Profiler:** OFF
- **Deep Profiling:** OFF

Android is supported on the current Windows 11 24H2 Build Automation image. The project subfolder is required because the Unity project's `Assets` and `ProjectSettings` directories are nested below the repository root.

## Advanced settings

### Script hook

Set **Pre-Export Method** to:

`LifePlayed.Client.Editor.LifePlayedProjectConfigurator.PreExport`

Do not add parentheses.

The hook runs after Unity script compilation and before player export. It:

1. creates/repairs the mobile URP asset
2. creates/registers URP global settings
3. generates the Bootstrap, AppShell, and WildRenewalHub scenes
4. installs those scenes into Editor build settings
5. applies portrait/mobile project settings
6. fails the build before export if required generated state is missing

### Tests

Enable:

- Run project unit tests when building
- Run EditMode tests
- Run PlayMode tests
- Mark build as failed if any test fails

### Addressables

For the **first prototype validation build**, leave automatic Addressables content building disabled. Addressables is installed and architecturally available, but Milestone 4 does not yet have a production remote catalog/content group that justifies building one.

### Android format

Use **APK** for the first prototype validation build so it can be installed directly on Android test hardware.

The pre-export hook also forces `EditorUserBuildSettings.development = true` and `buildAppBundle = false`. This guarantees the development-only runtime diagnostics HUD is present even if the dashboard configuration drifts. Profiler auto-connect and deep profiling remain disabled to avoid unnecessary runtime/build overhead.

Do not use the Play Store AAB path for this milestone.

## What this build must prove

The build only counts as the Milestone 4 validation build if it successfully proves:

- Unity package resolution/import
- Unity C# compilation
- URP generation/registration
- generated Bootstrap/AppShell/WildRenewalHub scenes
- EditMode tests
- PlayMode tests
- Android player export
- installable APK artifact

Runtime device validation must then record:

- launch succeeds
- portrait/safe-area UI behaves correctly
- Home / Quests / World / Hero / More controls respond
- Wild Renewal prototype renders
- Waykeeper placeholder renders
- starter companion placeholder renders/animates
- touch orbit camera responds
- app suspend/resume does not crash
- Reduced / Standard / High quality switching does not crash

## Quota policy

Do not turn on automatic builds after this validation.

Only launch another Unity build for:

- a blocker discovered by this build
- a meaningful runtime integration milestone
- a native/mobile platform change
- a release candidate

Keep a free-tier buffer for failed builds and hotfix validation.

## If the first build fails

Do not repeatedly retry unchanged configuration.

Capture:

- build number/id
- exact commit SHA
- editor version
- package resolution output
- first compiler/import error
- test summary
- Android/Gradle error if compilation succeeded

Fix the first root cause in PR #6, rerun static validation, then spend another Unity build only when the defect is materially corrected.

## Official Unity references

- Configure Build Automation: https://docs.unity.com/en-us/build-automation/basic-build-configuration/overview
- Pre-export methods: https://docs.unity.com/en-us/build-automation/advanced-build-configuration/run-custom-scripts-during-the-build-process
- Unit tests in Build Automation: https://docs.unity.com/en-us/build-automation/reference/unit-tests
- Supported builder OS/platforms: https://docs.unity.com/en-us/build-automation/reference/supported-platforms-on-each-builder-os
