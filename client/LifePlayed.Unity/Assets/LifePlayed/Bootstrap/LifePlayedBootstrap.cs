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
        private IClientSettingsStore _settingsStore;
        private GraphicsQualityController _quality;
        private PrototypeWildRenewalHub _worldHub;
        private RuntimeDiagnosticsPresenter _diagnostics;
        private IMobilePlatformProfile _platformProfile;

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

            _quality = GetComponent<GraphicsQualityController>();
            if (_quality == null)
            {
                _quality = gameObject.AddComponent<GraphicsQualityController>();
            }

            _platformProfile = new RuntimeMobilePlatformProfile();
            _settingsStore = new PlayerPrefsClientSettingsStore();

            RestoreGraphicsTier(_quality);
            _quality.TierChanged += SaveGraphicsTier;

            var navigator = new RuntimeNavigator();
            _coordinator = new AppCoordinator(
                navigator,
                lifecycle);

            StartCoroutine(
                EnsurePresentation(_quality));
        }

        private void OnDestroy()
        {
            if (_quality != null)
            {
                _quality.TierChanged -= SaveGraphicsTier;
            }

            if (_coordinator != null)
            {
                _coordinator.RouteChanged -= OnRouteChanged;
            }
        }

        private void RestoreGraphicsTier(
            IGraphicsQualityController graphicsQuality)
        {
            var saved = _settingsStore.Read(GraphicsTierKey);
            GraphicsTier tier;

            if (System.Enum.TryParse(saved, out tier))
            {
                graphicsQuality.Apply(tier);
            }
        }

        private void SaveGraphicsTier(GraphicsTier tier)
        {
            _settingsStore.Write(
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

                _diagnostics =
                    shell.GetComponent<RuntimeDiagnosticsPresenter>();

                if (_diagnostics == null)
                {
                    _diagnostics =
                        shell.gameObject.AddComponent<RuntimeDiagnosticsPresenter>();
                }

                _diagnostics.Bind(
                    _coordinator,
                    graphicsQuality,
                    _platformProfile);
            }

            _worldHub =
                FindFirstObjectByType<PrototypeWildRenewalHub>();

            if (_worldHub != null)
            {
                _worldHub.BindGraphicsQuality(
                    graphicsQuality);
            }

            _coordinator.RouteChanged += OnRouteChanged;
            OnRouteChanged(_coordinator.CurrentRoute);

            yield return LoadContentManifest();

            _coordinator.Open(AppRoute.World);
        }

        private IEnumerator LoadContentManifest()
        {
            var gateway = new StreamingAssetsContentGateway();
            var task = gateway.GetActiveManifestAsync(
                System.Threading.CancellationToken.None);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (_diagnostics == null)
            {
                yield break;
            }

            if (task.IsFaulted)
            {
                _diagnostics.SetContentStatus(
                    "CONTENT ERROR");
                yield break;
            }

            _diagnostics.SetContentManifest(
                task.Result);
        }

        private void OnRouteChanged(AppRoute route)
        {
            var worldActive = route == AppRoute.World;

            if (_worldHub != null)
            {
                _worldHub.SetPresentationActive(
                    worldActive);
            }

            if (_diagnostics != null)
            {
                _diagnostics.SetWorldRenderingActive(
                    worldActive);
            }
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
