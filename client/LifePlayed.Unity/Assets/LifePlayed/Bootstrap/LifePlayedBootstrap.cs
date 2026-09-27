using System.Collections;
using LifePlayed.Client.Application;
using LifePlayed.Client.Infrastructure;
using LifePlayed.Client.Platform;
using LifePlayed.Client.Presentation.UI;
using LifePlayed.Client.Presentation.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LifePlayed.Client.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class LifePlayedBootstrap : MonoBehaviour
    {
        private static LifePlayedBootstrap _instance;

        private const string GraphicsTierKey = "settings.graphics_tier";

        private AppCoordinator _coordinator;
        private ILocalStateStore _localState;

        public static LifePlayedBootstrap Instance => _instance;

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureBootstrap()
        {
            if (_instance != null)
            {
                return;
            }

            var root = new GameObject("LifePlayedRuntime");
            DontDestroyOnLoad(root);
            root.AddComponent<LifePlayedBootstrap>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            var lifecycle = GetComponent<PlatformLifecycleBridge>();
            if (lifecycle == null)
            {
                lifecycle = gameObject.AddComponent<PlatformLifecycleBridge>();
            }

            var quality = GetComponent<GraphicsQualityController>();
            if (quality == null)
            {
                quality = gameObject.AddComponent<GraphicsQualityController>();
            }

            _localState = new PlayerPrefsLocalStateStore();
            RestoreGraphicsTier(quality);
            quality.TierChanged += SaveGraphicsTier;

            var navigator = new RuntimeNavigator();
            _coordinator = new AppCoordinator(
                navigator,
                lifecycle);

            StartCoroutine(
                EnsurePresentation(quality));
        }

        private void OnDestroy()
        {
            var quality = GetComponent<GraphicsQualityController>();
            if (quality != null)
            {
                quality.TierChanged -= SaveGraphicsTier;
            }
        }

        private void RestoreGraphicsTier(
            IGraphicsQualityController graphicsQuality)
        {
            var saved = _localState.Read(GraphicsTierKey);
            GraphicsTier tier;

            if (System.Enum.TryParse(saved, out tier))
            {
                graphicsQuality.Apply(tier);
            }
        }

        private void SaveGraphicsTier(GraphicsTier tier)
        {
            _localState.Write(
                GraphicsTierKey,
                tier.ToString());
        }

        private IEnumerator EnsurePresentation(
            IGraphicsQualityController graphicsQuality)
        {
            yield return EnsureSceneOrFallback(
                "AppShell",
                () =>
                {
                    var uiRoot = new GameObject("AppShell");
                    return uiRoot.AddComponent<AppShellPresenter>();
                });

            yield return EnsureSceneOrFallback(
                "WildRenewalHub",
                () =>
                {
                    var worldRoot = new GameObject("WildRenewalHub");
                    return worldRoot.AddComponent<PrototypeWildRenewalHub>();
                });

            var shell = FindFirstObjectByType<AppShellPresenter>();
            if (shell != null)
            {
                shell.Bind(
                    _coordinator,
                    graphicsQuality);
            }

            _coordinator.Open(AppRoute.World);
        }

        private static IEnumerator EnsureSceneOrFallback<T>(
            string sceneName,
            System.Func<T> fallback)
            where T : Component
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                var scene = SceneManager.GetSceneByName(sceneName);
                if (!scene.isLoaded)
                {
                    var operation = SceneManager.LoadSceneAsync(
                        sceneName,
                        LoadSceneMode.Additive);

                    if (operation != null)
                    {
                        while (!operation.isDone)
                        {
                            yield return null;
                        }
                    }
                }
            }

            if (FindFirstObjectByType<T>() == null)
            {
                fallback();
            }
        }
    }
}
