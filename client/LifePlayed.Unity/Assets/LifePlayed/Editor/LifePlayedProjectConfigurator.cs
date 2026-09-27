using System.IO;
using LifePlayed.Client.Bootstrap;
using LifePlayed.Client.Presentation.UI;
using LifePlayed.Client.Presentation.World;
using UnityEditor;
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

        private static readonly string[] ScenePaths =
        {
            ScenesDirectory + "/Bootstrap.unity",
            ScenesDirectory + "/AppShell.unity",
            ScenesDirectory + "/WildRenewalHub.unity",
        };

        [MenuItem("Life Played/Configure Client Foundation")]
        public static void EnsureConfigured()
        {
            Directory.CreateDirectory(ScenesDirectory);
            Directory.CreateDirectory(SettingsDirectory);
            AssetDatabase.Refresh();

            EnsureRenderPipeline();
            EnsureScenes();
            EnsureMobileSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
                pipeline.LoadBuiltinRendererData();
                pipeline.renderScale = 0.90f;
                pipeline.useSRPBatcher = true;
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }

            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = pipeline;
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

        private static void EnsureMobileSettings()
        {
            PlayerSettings.productName = "Life Played";
            PlayerSettings.companyName = "Life Played";
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.runInBackground = false;

            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSettings.externalVersionControl = "Visible Meta Files";
        }
    }
}
