using LifePlayed.Client.Application;
using UnityEngine;
using UnityEngine.UI;

namespace LifePlayed.Client.Presentation.UI
{
    [DisallowMultipleComponent]
    public sealed class RuntimeDiagnosticsPresenter : MonoBehaviour
    {
        private const float UpdateIntervalSeconds = 0.5f;

        private AppCoordinator _coordinator;
        private IGraphicsQualityController _graphicsQuality;
        private IMobilePlatformProfile _platformProfile;
        private Text _label;
        private float _elapsed;
        private float _smoothedFps;
        private bool _worldRenderingActive;
        private string _contentStatus = "CONTENT …";
        private string _runtimeStatus = "READY";

        public void Bind(
            AppCoordinator coordinator,
            IGraphicsQualityController graphicsQuality,
            IMobilePlatformProfile platformProfile)
        {
            _coordinator = coordinator;
            _graphicsQuality = graphicsQuality;
            _platformProfile = platformProfile;

            if (Debug.isDebugBuild || Application.isEditor)
            {
                EnsureUi();
                Refresh();
            }
        }

        public void SetWorldRenderingActive(bool active)
        {
            _worldRenderingActive = active;
            Refresh();
        }

        public void SetContentManifest(
            LifePlayed.Client.DomainBridge.ClientContentManifest manifest)
        {
            _contentStatus =
                manifest.releaseVersion +
                " • schema " +
                manifest.schemaVersion;
            Refresh();
        }

        public void SetContentStatus(string status)
        {
            _contentStatus = status;
            Refresh();
        }

        public void SetRuntimeStatus(string status)
        {
            _runtimeStatus = status;
            Refresh();
        }

        private void Update()
        {
            if (_label == null)
            {
                return;
            }

            var delta = Time.unscaledDeltaTime;
            if (delta > 0.0001f)
            {
                var instantFps = 1f / delta;
                _smoothedFps = _smoothedFps <= 0f
                    ? instantFps
                    : Mathf.Lerp(
                        _smoothedFps,
                        instantFps,
                        0.10f);
            }

            _elapsed += delta;
            if (_elapsed >= UpdateIntervalSeconds)
            {
                _elapsed = 0f;
                Refresh();
            }
        }

        private void EnsureUi()
        {
            if (_label != null)
            {
                return;
            }

            var panelObject = new GameObject(
                "RuntimeDiagnostics",
                typeof(RectTransform));
            panelObject.transform.SetParent(
                transform,
                false);

            var rect = (RectTransform)panelObject.transform;
            rect.anchorMin = new Vector2(0.54f, 0.77f);
            rect.anchorMax = new Vector2(0.97f, 0.89f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var panel = panelObject.AddComponent<Image>();
            panel.color = new Color32(
                11,
                18,
                15,
                205);
            panel.raycastTarget = false;

            var textObject = new GameObject(
                "DiagnosticsText",
                typeof(RectTransform));
            textObject.transform.SetParent(
                panelObject.transform,
                false);

            var textRect =
                (RectTransform)textObject.transform;
            textRect.anchorMin = new Vector2(0.05f, 0.08f);
            textRect.anchorMax = new Vector2(0.95f, 0.92f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            _label = textObject.AddComponent<Text>();
            _label.font = Resources.GetBuiltinResource<Font>(
                "LegacyRuntime.ttf");
            _label.fontSize = 16;
            _label.alignment = TextAnchor.MiddleLeft;
            _label.color = new Color32(
                184,
                221,
                196,
                255);
            _label.raycastTarget = false;
        }

        private void Refresh()
        {
            if (_label == null ||
                _coordinator == null ||
                _graphicsQuality == null ||
                _platformProfile == null)
            {
                return;
            }

            var safeArea = Screen.safeArea;
            var fps = Mathf.RoundToInt(_smoothedFps);

            _label.text =
                "DEV  " +
                _platformProfile.Platform +
                "  " +
                _coordinator.CurrentRoute +
                "  " +
                _graphicsQuality.CurrentTier +
                "\nFPS " +
                fps +
                "  Target " +
                Application.targetFrameRate +
                "  World " +
                (_worldRenderingActive ? "ON" : "OFF") +
                "\nContent " +
                _contentStatus +
                "  " +
                _runtimeStatus +
                "\nSafe " +
                Mathf.RoundToInt(safeArea.width) +
                "x" +
                Mathf.RoundToInt(safeArea.height) +
                "  Pause " +
                (_coordinator.IsPaused ? "YES" : "NO");
        }
    }
}
