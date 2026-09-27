using System.Collections.Generic;
using LifePlayed.Client.Application;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace LifePlayed.Client.Presentation.UI
{
    [DisallowMultipleComponent]
    public sealed class AppShellPresenter : MonoBehaviour
    {
        private readonly Dictionary<AppRoute, Image> _buttonImages =
            new Dictionary<AppRoute, Image>();

        private readonly Dictionary<GraphicsTier, Image> _qualityImages =
            new Dictionary<GraphicsTier, Image>();

        private AppCoordinator _coordinator;
        private IGraphicsQualityController _graphicsQuality;
        private Text _title;
        private Text _subtitle;
        private Image _background;
        private GameObject _worldCard;
        private GameObject _qualityBar;

        private static readonly Color32 Background =
            new Color32(13, 21, 18, 242);

        private static readonly Color32 WorldVeil =
            new Color32(10, 19, 16, 62);

        private static readonly Color32 Panel =
            new Color32(24, 38, 31, 232);

        private static readonly Color32 Active =
            new Color32(106, 224, 145, 255);

        private static readonly Color32 Inactive =
            new Color32(76, 96, 87, 230);

        private static readonly Color32 TextColor =
            new Color32(238, 248, 241, 255);

        private static readonly Color32 SubtleText =
            new Color32(157, 191, 169, 255);

        private void Awake()
        {
            BuildUi();
        }

        public void Bind(
            AppCoordinator coordinator,
            IGraphicsQualityController graphicsQuality)
        {
            if (_coordinator != null)
            {
                _coordinator.RouteChanged -= OnRouteChanged;
            }

            if (_graphicsQuality != null)
            {
                _graphicsQuality.TierChanged -= OnTierChanged;
            }

            _coordinator = coordinator;
            _graphicsQuality = graphicsQuality;

            _coordinator.RouteChanged += OnRouteChanged;
            _graphicsQuality.TierChanged += OnTierChanged;

            OnTierChanged(_graphicsQuality.CurrentTier);
            OnRouteChanged(_coordinator.CurrentRoute);
        }

        private void OnDestroy()
        {
            if (_coordinator != null)
            {
                _coordinator.RouteChanged -= OnRouteChanged;
            }

            if (_graphicsQuality != null)
            {
                _graphicsQuality.TierChanged -= OnTierChanged;
            }
        }

        private void OnRouteChanged(AppRoute route)
        {
            var worldMode = route == AppRoute.World;

            if (_title != null)
            {
                _title.text = worldMode
                    ? "Wild Renewal"
                    : route.ToString();
            }

            if (_subtitle != null)
            {
                _subtitle.text = worldMode
                    ? "The Hearthwild • First World"
                    : "Your life builds the world";
            }

            if (_background != null)
            {
                _background.color = worldMode
                    ? WorldVeil
                    : Background;
            }

            if (_worldCard != null)
            {
                _worldCard.SetActive(worldMode);
            }

            if (_qualityBar != null)
            {
                _qualityBar.SetActive(worldMode);
            }

            foreach (var pair in _buttonImages)
            {
                pair.Value.color = pair.Key == route ? Active : Inactive;
            }
        }

        private void OnTierChanged(GraphicsTier tier)
        {
            foreach (var pair in _qualityImages)
            {
                pair.Value.color = pair.Key == tier ? Active : Inactive;
            }
        }

        private void BuildUi()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 2400f);
            scaler.matchWidthOrHeight = 0.5f;

            if (GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            EnsureEventSystem();

            _background = CreatePanel(
                "Background",
                transform,
                Vector2.zero,
                Vector2.one,
                Background);

            var safe = CreatePanel(
                "SafeArea",
                _background.transform,
                Vector2.zero,
                Vector2.one,
                Color.clear);
            safe.gameObject.AddComponent<SafeAreaFitter>();

            _title = CreateText(
                "Title",
                safe.transform,
                "Home",
                50,
                TextAnchor.MiddleLeft);
            SetAnchors(
                _title.rectTransform,
                new Vector2(0.05f, 0.90f),
                new Vector2(0.95f, 0.98f));

            _subtitle = CreateText(
                "Status",
                safe.transform,
                "Your life builds the world",
                26,
                TextAnchor.UpperLeft);
            _subtitle.color = SubtleText;
            SetAnchors(
                _subtitle.rectTransform,
                new Vector2(0.05f, 0.84f),
                new Vector2(0.95f, 0.90f));

            BuildWorldCard(safe.transform);
            BuildQualityBar(safe.transform);

            var nav = CreatePanel(
                "BottomNavigation",
                safe.transform,
                new Vector2(0.03f, 0.02f),
                new Vector2(0.97f, 0.11f),
                Panel);

            var routes = new[]
            {
                AppRoute.Home,
                AppRoute.Quests,
                AppRoute.World,
                AppRoute.Hero,
                AppRoute.More,
            };

            for (var index = 0; index < routes.Length; index++)
            {
                CreateNavigationButton(
                    nav.transform,
                    routes[index],
                    index,
                    routes.Length);
            }
        }

        private void BuildWorldCard(Transform parent)
        {
            var card = CreatePanel(
                "WorldStatusCard",
                parent,
                new Vector2(0.05f, 0.72f),
                new Vector2(0.58f, 0.82f),
                Panel);

            _worldCard = card.gameObject;

            var eyebrow = CreateText(
                "Eyebrow",
                card.transform,
                "THE QUIET BLOOM",
                19,
                TextAnchor.UpperLeft);
            eyebrow.color = Active;
            SetAnchors(
                eyebrow.rectTransform,
                new Vector2(0.07f, 0.58f),
                new Vector2(0.93f, 0.90f));

            var headline = CreateText(
                "Headline",
                card.transform,
                "The Hearthwild is stirring.",
                26,
                TextAnchor.MiddleLeft);
            SetAnchors(
                headline.rectTransform,
                new Vector2(0.07f, 0.15f),
                new Vector2(0.93f, 0.62f));
        }

        private void BuildQualityBar(Transform parent)
        {
            var bar = CreatePanel(
                "GraphicsQualityBar",
                parent,
                new Vector2(0.48f, 0.12f),
                new Vector2(0.97f, 0.18f),
                Panel);

            _qualityBar = bar.gameObject;

            var tiers = new[]
            {
                GraphicsTier.Reduced,
                GraphicsTier.Standard,
                GraphicsTier.High,
            };

            for (var index = 0; index < tiers.Length; index++)
            {
                CreateQualityButton(
                    bar.transform,
                    tiers[index],
                    index,
                    tiers.Length);
            }
        }

        private void CreateNavigationButton(
            Transform parent,
            AppRoute route,
            int index,
            int count)
        {
            var buttonObject = new GameObject(
                route + "Button",
                typeof(RectTransform));
            buttonObject.transform.SetParent(parent, false);

            var rect = (RectTransform)buttonObject.transform;
            var width = 1f / count;
            rect.anchorMin = new Vector2(index * width + 0.01f, 0.12f);
            rect.anchorMax = new Vector2((index + 1) * width - 0.01f, 0.88f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = buttonObject.AddComponent<Image>();
            image.color = Inactive;
            _buttonImages[route] = image;

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                if (_coordinator != null)
                {
                    _coordinator.Open(route);
                }
            });

            var label = CreateText(
                "Label",
                buttonObject.transform,
                route.ToString(),
                22,
                TextAnchor.MiddleCenter);
            Stretch(label.rectTransform);
        }

        private void CreateQualityButton(
            Transform parent,
            GraphicsTier tier,
            int index,
            int count)
        {
            var buttonObject = new GameObject(
                tier + "QualityButton",
                typeof(RectTransform));
            buttonObject.transform.SetParent(parent, false);

            var rect = (RectTransform)buttonObject.transform;
            var width = 1f / count;
            rect.anchorMin = new Vector2(index * width + 0.025f, 0.15f);
            rect.anchorMax = new Vector2((index + 1) * width - 0.025f, 0.85f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = buttonObject.AddComponent<Image>();
            image.color = Inactive;
            _qualityImages[tier] = image;

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                if (_graphicsQuality != null)
                {
                    _graphicsQuality.Apply(tier);
                }
            });

            var label = CreateText(
                "Label",
                buttonObject.transform,
                tier == GraphicsTier.Standard ? "Std" : tier.ToString(),
                18,
                TextAnchor.MiddleCenter);
            Stretch(label.rectTransform);
        }

        private static Image CreatePanel(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Color color)
        {
            var panelObject = new GameObject(
                name,
                typeof(RectTransform));
            panelObject.transform.SetParent(parent, false);

            var rect = (RectTransform)panelObject.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = panelObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Text CreateText(
            string name,
            Transform parent,
            string value,
            int fontSize,
            TextAnchor alignment)
        {
            var textObject = new GameObject(
                name,
                typeof(RectTransform));
            textObject.transform.SetParent(parent, false);

            var text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>(
                "LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = TextColor;
            text.raycastTarget = false;
            return text;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            var eventSystemObject = new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(InputSystemUIInputModule));
            DontDestroyOnLoad(eventSystemObject);
        }

        private static void SetAnchors(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void Stretch(RectTransform rect)
        {
            SetAnchors(
                rect,
                Vector2.zero,
                Vector2.one);
        }
    }
}
