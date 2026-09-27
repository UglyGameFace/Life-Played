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

        private AppCoordinator _coordinator;

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

            var navigator = new RuntimeNavigator();
            _coordinator = new AppCoordinator(
                navigator,
                lifecycle);

            StartCoroutine(
                EnsurePresentation(quality));
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
