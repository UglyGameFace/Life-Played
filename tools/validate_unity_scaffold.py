#!/usr/bin/env python3
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
UNITY = ROOT / "client" / "LifePlayed.Unity"

required = [
    UNITY / "ProjectSettings" / "ProjectVersion.txt",
    UNITY / "Packages" / "manifest.json",
    UNITY / "Assets" / "LifePlayed" / "DomainBridge" / "LifePlayed.Client.DomainBridge.asmdef",
    UNITY / "Assets" / "LifePlayed" / "Application" / "LifePlayed.Client.Application.asmdef",
    UNITY / "Assets" / "LifePlayed" / "Infrastructure" / "LifePlayed.Client.Infrastructure.asmdef",
    UNITY / "Assets" / "LifePlayed" / "Presentation" / "UI" / "LifePlayed.Client.Presentation.UI.asmdef",
    UNITY / "Assets" / "LifePlayed" / "Presentation" / "World" / "LifePlayed.Client.Presentation.World.asmdef",
    UNITY / "Assets" / "LifePlayed" / "Platform" / "LifePlayed.Client.Platform.asmdef",
    UNITY / "Assets" / "LifePlayed" / "Platform" / "RuntimeMobilePlatformProfile.cs",
    UNITY / "Assets" / "LifePlayed" / "Bootstrap" / "LifePlayed.Client.Bootstrap.asmdef",
    UNITY / "Assets" / "LifePlayed" / "Tests" / "EditMode" / "LifePlayed.Client.Tests.EditMode.asmdef",
    UNITY / "Assets" / "LifePlayed" / "Tests" / "PlayMode" / "LifePlayed.Client.Tests.PlayMode.asmdef",
    UNITY / "Assets" / "LifePlayed" / "Bootstrap" / "LifePlayedBootstrap.cs",
    UNITY / "Assets" / "LifePlayed" / "Infrastructure" / "RuntimeServices.cs",
    UNITY / "Assets" / "LifePlayed" / "Presentation" / "UI" / "AppShellPresenter.cs",
    UNITY / "Assets" / "LifePlayed" / "Presentation" / "UI" / "SafeAreaFitter.cs",
    UNITY / "Assets" / "LifePlayed" / "Presentation" / "UI" / "RuntimeDiagnosticsPresenter.cs",
    UNITY / "Assets" / "LifePlayed" / "Presentation" / "World" / "GraphicsQualityController.cs",
    UNITY / "Assets" / "LifePlayed" / "Presentation" / "World" / "PrototypeWildRenewalHub.cs",
    UNITY / "Assets" / "LifePlayed" / "Presentation" / "World" / "PrototypeAmbientWisp.cs",
    UNITY / "Assets" / "LifePlayed" / "Editor" / "LifePlayedProjectConfigurator.cs",
    UNITY / "Assets" / "LifePlayed" / "Editor" / "LifePlayedAndroidBuild.cs",
    UNITY / "Assets" / "LifePlayed" / "Tests" / "EditMode" / "OfflineStoreTests.cs",
]

missing = [str(path.relative_to(ROOT)) for path in required if not path.is_file()]
if missing:
    raise SystemExit("Missing Unity scaffold files:\n- " + "\n- ".join(missing))

version = (UNITY / "ProjectSettings" / "ProjectVersion.txt").read_text(encoding="utf-8")
if "6000.3.25f1" not in version:
    raise SystemExit("Unity Editor version must remain pinned to 6000.3.25f1.")

manifest = json.loads((UNITY / "Packages" / "manifest.json").read_text(encoding="utf-8"))
deps = manifest.get("dependencies", {})
expected = {
    "com.unity.addressables": "2.10.3",
    "com.unity.addressables.android": "1.1.0",
    "com.unity.inputsystem": "1.20.0",
    "com.unity.render-pipelines.universal": "17.3.0",
}
for package, expected_version in expected.items():
    actual = deps.get(package)
    if actual != expected_version:
        raise SystemExit(
            f"{package} must be pinned to {expected_version}, got {actual!r}."
        )

asmdefs = {}
for path in UNITY.glob("Assets/**/*.asmdef"):
    data = json.loads(path.read_text(encoding="utf-8"))
    name = data.get("name")
    if not name:
        raise SystemExit(f"Assembly definition has no name: {path.relative_to(ROOT)}")
    if name in asmdefs:
        raise SystemExit(f"Duplicate Unity assembly name: {name}")
    asmdefs[name] = (path, data)

for pure_name in (
    "LifePlayed.Client.DomainBridge",
    "LifePlayed.Client.Application",
):
    data = asmdefs[pure_name][1]
    if data.get("noEngineReferences") is not True:
        raise SystemExit(f"{pure_name} must remain free of UnityEngine references.")

app_refs = set(asmdefs["LifePlayed.Client.Application"][1].get("references", []))
if app_refs != {"LifePlayed.Client.DomainBridge"}:
    raise SystemExit(
        "Client Application may only reference Client DomainBridge at this stage."
    )

generated = [
    "Library",
    "Temp",
    "Obj",
    "Build",
    "Builds",
    "Logs",
    "UserSettings",
]
present_generated = [
    name for name in generated if (UNITY / name).exists()
]
if present_generated:
    raise SystemExit(
        "Generated Unity directories are committed/present: "
        + ", ".join(present_generated)
    )

gitignore = (ROOT / ".gitignore").read_text(encoding="utf-8")
for pattern in (
    "/client/LifePlayed.Unity/[Ll]ibrary/",
    "/client/LifePlayed.Unity/[Tt]emp/",
    "/client/LifePlayed.Unity/[Oo]bj/",
    "/client/LifePlayed.Unity/[Uu]ser[Ss]ettings/",
):
    if pattern not in gitignore:
        raise SystemExit(f"Missing nested Unity ignore rule: {pattern}")

configurator = (
    UNITY
    / "Assets"
    / "LifePlayed"
    / "Editor"
    / "LifePlayedProjectConfigurator.cs"
).read_text(encoding="utf-8")

required_configurator_fragments = (
    "public static void PreExport()",
    "ScriptableObject.CreateInstance(settingsType)",
    "settings.Initialize(null);",
    "EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset(",
    "ValidateForBuild();",
)
for fragment in required_configurator_fragments:
    if fragment not in configurator:
        raise SystemExit(
            f"Unity Build Automation configurator is missing: {fragment}"
        )

if "pipeline.EnsureGlobalSettings()" in configurator:
    raise SystemExit(
        "Do not call protected RenderPipelineAsset.EnsureGlobalSettings() directly."
    )

if "RenderPipelineGlobalSettingsUtils.Create(" in configurator:
    raise SystemExit(
        "Do not rely on the undocumented RenderPipelineGlobalSettingsUtils.Create path."
    )

print(
    f"Unity scaffold OK: {len(asmdefs)} assemblies, "
    f"Editor 6000.3.25f1, required packages pinned."
)


# Milestone 4 architecture regressions.
application_code = (
    UNITY / "Assets" / "LifePlayed" / "Application" / "ClientApplication.cs"
).read_text(encoding="utf-8")
infrastructure_code = (
    UNITY / "Assets" / "LifePlayed" / "Infrastructure" / "RuntimeServices.cs"
).read_text(encoding="utf-8")
quality_code = (
    UNITY / "Assets" / "LifePlayed" / "Presentation" / "World" / "GraphicsQualityController.cs"
).read_text(encoding="utf-8")

if "ILocalStateStore" in application_code:
    raise SystemExit(
        "PlayerPrefs/settings storage must not masquerade as the application database."
    )

for fragment in (
    "IClientSettingsStore",
    "IClientOfflineStore",
):
    if fragment not in application_code:
        raise SystemExit(f"Missing client persistence boundary: {fragment}")

for fragment in (
    "PlayerPrefsClientSettingsStore",
    "JsonFileOfflineStore",
):
    if fragment not in infrastructure_code:
        raise SystemExit(f"Missing persistence adapter: {fragment}")

if "QualitySettings.shadowDistance" in quality_code:
    raise SystemExit(
        "URP shadow distance must be configured through UniversalRenderPipelineAsset."
    )

for fragment in (
    "pipeline.shadowDistance = shadowDistance;",
    "pipeline.msaaSampleCount = msaaSampleCount;",
    "pipeline.shadowCascadeCount = shadowCascadeCount;",
):
    if fragment not in quality_code:
        raise SystemExit(f"Missing URP runtime quality control: {fragment}")

for fragment in (
    "PlayerSettings.Android.renderOutsideSafeArea = true;",
    "PlayerSettings.Android.optimizedFramePacing = true;",
):
    if fragment not in configurator:
        raise SystemExit(f"Missing Android mobile setting: {fragment}")


# Authoritative content must be staged at pre-export, not duplicated in Git.
generated_content = (
    UNITY
    / "Assets"
    / "StreamingAssets"
    / "LifePlayed"
    / "Content"
)
if generated_content.exists():
    raise SystemExit(
        "Generated Unity StreamingAssets content must not be committed before pre-export."
    )

if "/client/LifePlayed.Unity/Assets/StreamingAssets/LifePlayed/Content/" not in gitignore:
    raise SystemExit(
        "Generated authoritative Unity content staging directory must be ignored."
    )

for fragment in (
    "CopyAuthoritativeContent();",
    "wild-renewal-v1",
):
    if fragment not in configurator:
        raise SystemExit(
            f"Unity pre-export content staging is missing: {fragment}"
        )

runtime_services = (
    UNITY
    / "Assets"
    / "LifePlayed"
    / "Infrastructure"
    / "RuntimeServices.cs"
).read_text(encoding="utf-8")

if "StreamingAssetsContentGateway" not in runtime_services:
    raise SystemExit(
        "Runtime authoritative content manifest gateway is missing."
    )


camera_code = (
    UNITY
    / "Assets"
    / "LifePlayed"
    / "Presentation"
    / "World"
    / "TouchOrbitCamera.cs"
).read_text(encoding="utf-8")

for fragment in (
    "EnhancedTouchSupport.Enable();",
    "EnhancedTouch.activeTouches",
    "minimumDistance",
    "maximumDistance",
):
    if fragment not in camera_code:
        raise SystemExit(
            f"Touch camera is missing mobile gesture support: {fragment}"
        )
