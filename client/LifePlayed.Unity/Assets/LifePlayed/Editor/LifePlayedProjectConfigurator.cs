using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using LifePlayed.Client.Bootstrap;
using LifePlayed.Client.Presentation.UI;
using LifePlayed.Client.Presentation.World;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace LifePlayed.Client.Editor
{
    public static class LifePlayedProjectConfigurator
    {
        private const string ScenesDirectory = "Assets/LifePlayed/Scenes";
        private const string SettingsDirectory = "Assets/LifePlayed/Generated";
        private const string PipelinePath =
            SettingsDirectory + "/LifePlayedMobileURP.asset";
        private const string GlobalSettingsPath =
            SettingsDirectory + "/LifePlayedURPGlobalSettings.asset";
        private const string GeneratedContentDirectory =
            "Assets/StreamingAssets/LifePlayed/Content/wild-renewal-v1";

        [System.Serializable]
        private sealed class ManifestHeader
        {
            public int schemaVersion;
            public string releaseId = string.Empty;
            public string releaseVersion = string.Empty;
        }

        [System.Serializable]
        private sealed class VerificationRecord
        {
            public int schemaVersion;
            public string releaseId = string.Empty;
            public string releaseVersion = string.Empty;
            public string contentSha256 = string.Empty;
        }

        private static readonly string[] ScenePaths =
        {
            ScenesDirectory + "/Bootstrap.unity",
            ScenesDirectory + "/AppShell.unity",
            ScenesDirectory + "/WildRenewalHub.unity",
        };

        public static void PreExport()
        {
            EnsureConfigured();
            ValidateForBuild();
        }

        [MenuItem("Life Played/Configure Client Foundation")]
        public static void EnsureConfigured()
        {
            Directory.CreateDirectory(ScenesDirectory);
            Directory.CreateDirectory(SettingsDirectory);
            CopyAuthoritativeContent();
            AssetDatabase.Refresh();

            EnsureRenderPipeline();
            EnsureScenes();
            EnsureMobileSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CopyAuthoritativeContent()
        {
            var repositoryRoot = Path.GetFullPath(
                Path.Combine(
                    Application.dataPath,
                    "..",
                    "..",
                    ".."));

            var sourceDirectory = Path.Combine(
                repositoryRoot,
                "content",
                "releases",
                "wild-renewal-v1");

            var sourceManifest = Path.Combine(
                sourceDirectory,
                "manifest.json");

            var sourceContent = Path.Combine(
                sourceDirectory,
                "content.json");

            if (!File.Exists(sourceManifest) ||
                !File.Exists(sourceContent))
            {
                throw new System.InvalidOperationException(
                    "Authoritative Wild Renewal release is missing.");
            }

            var manifestJson =
                File.ReadAllText(
                    sourceManifest);

            var header =
                JsonUtility.FromJson<ManifestHeader>(
                    manifestJson);

            if (header == null ||
                header.schemaVersion <= 0 ||
                string.IsNullOrWhiteSpace(
                    header.releaseId) ||
                string.IsNullOrWhiteSpace(
                    header.releaseVersion))
            {
                throw new System.InvalidOperationException(
                    "Authoritative content manifest header is invalid.");
            }

            var hashMatch = Regex.Match(
                manifestJson,
                "\\\"content\\.json\\\"\\s*:\\s*\\\"([A-Fa-f0-9]{64})\\\"");

            if (!hashMatch.Success)
            {
                throw new System.InvalidOperationException(
                    "Authoritative manifest is missing the content.json SHA-256.");
            }

            var contentBytes =
                File.ReadAllBytes(
                    sourceContent);

            string actualHash;
            using (var sha256 = SHA256.Create())
            {
                actualHash = System.BitConverter
                    .ToString(
                        sha256.ComputeHash(
                            contentBytes))
                    .Replace(
                        "-",
                        string.Empty);
            }

            var expectedHash =
                hashMatch.Groups[1].Value;

            if (!string.Equals(
                actualHash,
                expectedHash,
                System.StringComparison.OrdinalIgnoreCase))
            {
                throw new System.InvalidOperationException(
                    "Authoritative content.json does not match its manifest SHA-256.");
            }

            if (Directory.Exists(
                GeneratedContentDirectory))
            {
                Directory.Delete(
                    GeneratedContentDirectory,
                    true);
            }

            Directory.CreateDirectory(
                GeneratedContentDirectory);

            File.Copy(
                sourceManifest,
                Path.Combine(
                    GeneratedContentDirectory,
                    "manifest.json"),
                true);

            File.Copy(
                sourceContent,
                Path.Combine(
                    GeneratedContentDirectory,
                    "content.json"),
                true);

            var verification =
                new VerificationRecord
                {
                    schemaVersion =
                        header.schemaVersion,
                    releaseId =
                        header.releaseId,
                    releaseVersion =
                        header.releaseVersion,
                    contentSha256 =
                        actualHash,
                };

            File.WriteAllText(
                Path.Combine(
                    GeneratedContentDirectory,
                    "verification.json"),
                JsonUtility.ToJson(
                    verification,
                    false));
        }

        private static void EnsureRenderPipeline()
        {
            var pipeline =
                AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(
                    PipelinePath);

            if (pipeline == null)
            {
                pipeline = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
                pipeline.name = "LifePlayedMobileURP";
                var rendererData = pipeline.LoadBuiltinRendererData();
                AssetDatabase.CreateAsset(pipeline, PipelinePath);

                if (rendererData != null)
                {
                    rendererData.name = "LifePlayedMobileRenderer";
                    AssetDatabase.AddObjectToAsset(rendererData, pipeline);
                }

            }

            pipeline.renderScale = 0.90f;
            pipeline.useSRPBatcher = true;
            pipeline.supportsHDR = false;
            pipeline.supportsCameraDepthTexture = false;
            pipeline.supportsCameraOpaqueTexture = false;
            pipeline.supportsDynamicBatching = true;
            pipeline.maxAdditionalLightsCount = 2;
            pipeline.msaaSampleCount = 2;
            pipeline.shadowDistance = 28f;
            pipeline.shadowCascadeCount = 2;
            EditorUtility.SetDirty(pipeline);

            EnsureRenderPipelineGlobalSettings();
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
        }

        private static void EnsureRenderPipelineGlobalSettings()
        {
            var existing =
                EditorGraphicsSettings.GetRenderPipelineGlobalSettingsAsset(
                    typeof(UniversalRenderPipeline));

            if (existing != null)
            {
                return;
            }

            var settingsType = System.Type.GetType(
                "UnityEngine.Rendering.Universal.UniversalRenderPipelineGlobalSettings, " +
                "Unity.RenderPipelines.Universal.Runtime");

            if (settingsType == null)
            {
                throw new System.InvalidOperationException(
                    "URP global settings type could not be resolved.");
            }

            var settings = ScriptableObject.CreateInstance(settingsType)
                as RenderPipelineGlobalSettings;

            if (settings == null)
            {
                throw new System.InvalidOperationException(
                    "URP global settings could not be created.");
            }

            settings.name = "LifePlayedURPGlobalSettings";
            settings.Initialize(null);
            AssetDatabase.CreateAsset(
                settings,
                GlobalSettingsPath);

            EditorGraphicsSettings.PopulateRenderPipelineGraphicsSettings(
                settings);
            EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset(
                typeof(UniversalRenderPipeline),
                settings);
        }

        private static void EnsureScenes()
        {
            CreateSceneIfMissing(
                ScenePaths[0],
                "BootstrapRoot",
                root => root.AddComponent<LifePlayedBootstrap>());

            CreateSceneIfMissing(
                ScenePaths[1],
                "AppShell",
                root => root.AddComponent<AppShellPresenter>());

            CreateSceneIfMissing(
                ScenePaths[2],
                "WildRenewalHub",
                root => root.AddComponent<PrototypeWildRenewalHub>());

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePaths[0], true),
                new EditorBuildSettingsScene(ScenePaths[1], true),
                new EditorBuildSettingsScene(ScenePaths[2], true),
            };
        }

        private static void CreateSceneIfMissing(
            string path,
            string rootName,
            System.Action<GameObject> configure)
        {
            if (File.Exists(path))
            {
                return;
            }

            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            var root = new GameObject(rootName);
            configure(root);

            EditorSceneManager.SaveScene(scene, path);
        }

        private static void ValidateForBuild()
        {
#if !ENABLE_INPUT_SYSTEM
            throw new System.InvalidOperationException(
                "Active Input Handling must enable the Input System package. " +
                "Set Player > Other Settings > Active Input Handling to " +
                "Input System Package (New) or Both, restart the Editor, " +
                "then rerun the build.");
#endif

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                throw new System.InvalidOperationException(
                    "Playable Build #1 must run with Android as the active build target.");
            }

            if (!Application.unityVersion.StartsWith("6000.3.25f1"))
            {
                throw new System.InvalidOperationException(
                    "Unexpected Unity editor version: " +
                    Application.unityVersion);
            }

            var pipeline =
                GraphicsSettings.defaultRenderPipeline
                    as UniversalRenderPipelineAsset;

            if (pipeline == null)
            {
                throw new System.InvalidOperationException(
                    "URP is not assigned in Graphics Settings.");
            }

            if (pipeline.supportsHDR ||
                pipeline.supportsCameraDepthTexture ||
                pipeline.supportsCameraOpaqueTexture)
            {
                throw new System.InvalidOperationException(
                    "The mobile URP asset has expensive features enabled unexpectedly.");
            }

            if (PlayerSettings.defaultInterfaceOrientation !=
                UIOrientation.Portrait)
            {
                throw new System.InvalidOperationException(
                    "Life Played V1 must remain portrait-first.");
            }

            if (!PlayerSettings.Android.renderOutsideSafeArea ||
                !PlayerSettings.Android.optimizedFramePacing ||
                !PlayerSettings.Android.startInFullscreen)
            {
                throw new System.InvalidOperationException(
                    "Required Android display/frame-pacing settings are missing.");
            }

            if (!EditorUserBuildSettings.development ||
                EditorUserBuildSettings.connectProfiler ||
                EditorUserBuildSettings.buildWithDeepProfilingSupport ||
                EditorUserBuildSettings.buildAppBundle)
            {
                throw new System.InvalidOperationException(
                    "Playable Build #1 must be a diagnostic Development APK " +
                    "without profiler auto-connect or deep profiling.");
            }

            if (PlayerSettings.GetScriptingBackend(
                    UnityEditor.Build.NamedBuildTarget.Android) !=
                ScriptingImplementation.IL2CPP ||
                PlayerSettings.GetIl2CppCompilerConfiguration(
                    UnityEditor.Build.NamedBuildTarget.Android) !=
                Il2CppCompilerConfiguration.Debug ||
                PlayerSettings.Android.targetArchitectures !=
                AndroidArchitecture.ARM64)
            {
                throw new System.InvalidOperationException(
                    "Playable Build #1 must use ARM64-only IL2CPP with " +
                    "the faster Debug C++ compiler configuration.");
            }

            var generatedManifest = Path.Combine(
                GeneratedContentDirectory,
                "manifest.json");

            var generatedContent = Path.Combine(
                GeneratedContentDirectory,
                "content.json");

            var generatedVerification = Path.Combine(
                GeneratedContentDirectory,
                "verification.json");

            if (!File.Exists(generatedManifest) ||
                !File.Exists(generatedContent) ||
                !File.Exists(generatedVerification))
            {
                throw new System.InvalidOperationException(
                    "Authoritative Wild Renewal content was not staged.");
            }

            foreach (var scenePath in ScenePaths)
            {
                if (!File.Exists(scenePath))
                {
                    throw new System.InvalidOperationException(
                        "Required scene was not generated: " + scenePath);
                }
            }

            if (EditorBuildSettings.scenes.Length != ScenePaths.Length)
            {
                throw new System.InvalidOperationException(
                    "Editor build settings do not contain the expected Life Played scenes.");
            }

            Debug.Log(
                "[Life Played Build Preflight] PASS | Editor=" +
                Application.unityVersion +
                " | Target=" +
                EditorUserBuildSettings.activeBuildTarget +
                " | Scenes=" +
                EditorBuildSettings.scenes.Length +
                " | URP renderScale=" +
                pipeline.renderScale +
                " | Content=wild-renewal-v1");
        }

        private static void EnsureMobileSettings()
        {
            PlayerSettings.productName = "Life Played";
            PlayerSettings.companyName = "Life Played";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.runInBackground = false;

            PlayerSettings.Android.renderOutsideSafeArea = true;
            PlayerSettings.Android.optimizedFramePacing = true;
            PlayerSettings.Android.startInFullscreen = true;

            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
            EditorUserBuildSettings.allowDebugging = false;
            EditorUserBuildSettings.buildAppBundle = false;

            PlayerSettings.SetScriptingBackend(
                UnityEditor.Build.NamedBuildTarget.Android,
                ScriptingImplementation.IL2CPP);

            PlayerSettings.SetIl2CppCompilerConfiguration(
                UnityEditor.Build.NamedBuildTarget.Android,
                Il2CppCompilerConfiguration.Debug);

            PlayerSettings.Android.targetArchitectures =
                AndroidArchitecture.ARM64;

            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSettings.externalVersionControl = "Visible Meta Files";
        }
    }
}
