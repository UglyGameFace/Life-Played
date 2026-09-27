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

        private readonly Dictionary<AppRoute, GameObject> _routePanels =
            new Dictionary<AppRoute, GameObject>();

        private readonly Dictionary<GraphicsTier, Image> _qualityImages =
            new Dictionary<GraphicsTier, Image>();

        private AppCoordinator _coordinator;
        private IGraphicsQualityController _graphicsQuality;
        private Text _title;
        private Text _subtitle;
        private Image _background;
        private GameObject _worldCard;
        private GameObject _qualityBar;

        public event System.Action CompanionCallRequested = delegate { };

        private static readonly Color32 Background =
            new Color32(13, 21, 18, 242);

        private static readonly Color32 WorldVeil =
            new Color32(10, 19, 16, 62);

        private static readonly Color32 Panel =
            new Color32(24, 38, 31, 232);

        private static readonly Color32 Card =
            new Color32(31, 49, 40, 238);

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
                _subtitle.text = SubtitleFor(route);
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
                _qualityBar.SetActive(
                    worldMode ||
                    route == AppRoute.More);
            }

            SetRoutePanelVisibility(route);

            foreach (var pair in _buttonImages)
            {
                pair.Value.color =
                    pair.Key == route
                        ? Active
                        : Inactive;
            }
        }

        private void OnTierChanged(GraphicsTier tier)
        {
            foreach (var pair in _qualityImages)
            {
                pair.Value.color =
                    pair.Key == tier
                        ? Active
                        : Inactive;
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

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution =
                new Vector2(1080f, 2400f);
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
                SubtitleFor(AppRoute.Home),
                26,
                TextAnchor.UpperLeft);
            _subtitle.color = SubtleText;
            SetAnchors(
                _subtitle.rectTransform,
                new Vector2(0.05f, 0.84f),
                new Vector2(0.95f, 0.90f));

            BuildUtilityRoutePanels(
                safe.transform);

            BuildWorldCard(
                safe.transform);

            BuildQualityBar(
                safe.transform);

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

            for (var index = 0;
                index < routes.Length;
                index++)
            {
                CreateNavigationButton(
                    nav.transform,
                    routes[index],
                    index,
                    routes.Length);
            }

            SetRoutePanelVisibility(
                AppRoute.Home);

            _worldCard.SetActive(false);
            _qualityBar.SetActive(false);
        }

        private void BuildUtilityRoutePanels(
            Transform parent)
        {
            _routePanels[AppRoute.Home] =
                CreateRoutePanel(
                    parent,
                    AppRoute.Home,
                    "TODAY",
                    "Life Played is ready to become useful.",
                    "The mobile foundation now separates fast utility screens from the 3D world.",
                    new RouteCard(
                        "OFFLINE READY",
                        "Durable local queue",
                        "Pending mutations and the sync cursor survive an app restart without turning PlayerPrefs into a database."),
                    new RouteCard(
                        "FIRST WORLD",
                        "Wild Renewal",
                        "The validated Wild Renewal release is staged into the build and loaded as the current authored world."));

            _routePanels[AppRoute.Quests] =
                CreateRoutePanel(
                    parent,
                    AppRoute.Quests,
                    "QUEST JOURNAL",
                    "Your real life becomes the adventure.",
                    "This foundation exposes the authored story shape without pretending the Milestone 5 Action loop is already finished.",
                    new RouteCard(
                        "SAGA I",
                        "The Quiet Bloom",
                        "Eight validated chapters lead from The Empty Hearth to the Quiet Bloom finale."),
                    new RouteCard(
                        "CAMPAIGNS",
                        "Built for real goals",
                        "Campaign framing exists for building, learning, restoring, training, planning, travel, finance, social goals, routines, and launches."));

            _routePanels[AppRoute.Hero] =
                CreateRoutePanel(
                    parent,
                    AppRoute.Hero,
                    "WAYKEEPER",
                    "A persistent identity, not a disposable avatar.",
                    "Character presentation stays separate from authoritative progression so visual upgrades cannot rewrite game rules.",
                    new RouteCard(
                        "FOUNDING WORLD",
                        "Wild Renewal",
                        "The Hearthwild is the first persistent world tied to this player identity."),
                    new RouteCard(
                        "COMPANION",
                        "Leafglow Fox • Starter",
                        "The first authored companion is represented in 3D now, with its final production model still intentionally deferred."));

            _routePanels[AppRoute.More] =
                CreateRoutePanel(
                    parent,
                    AppRoute.More,
                    "CLIENT",
                    "Mobile-first settings and diagnostics.",
                    "The first APK is designed to tell us what is actually happening on-device instead of making us guess.",
                    new RouteCard(
                        "DISPLAY",
                        "Portrait + safe area",
                        "Cutouts, rounded corners, frame pacing, and quality tiers are part of the build gate."),
                    new RouteCard(
                        "POWER",
                        "World sleeps off-route",
                        "Home, Quests, Hero, and More suspend the 3D world so ordinary planning screens do not waste GPU time."));
        }

        private GameObject CreateRoutePanel(
            Transform parent,
            AppRoute route,
            string eyebrow,
            string headline,
            string description,
            RouteCard firstCard,
            RouteCard secondCard)
        {
            var panel = CreatePanel(
                route + "RoutePanel",
                parent,
                new Vector2(0.05f, 0.20f),
                new Vector2(0.95f, 0.80f),
                Panel);

            var tag = CreateText(
                "Eyebrow",
                panel.transform,
                eyebrow,
                19,
                TextAnchor.UpperLeft);
            tag.color = Active;
            SetAnchors(
                tag.rectTransform,
                new Vector2(0.05f, 0.87f),
                new Vector2(0.95f, 0.96f));

            var title = CreateText(
                "Headline",
                panel.transform,
                headline,
                34,
                TextAnchor.UpperLeft);
            SetAnchors(
                title.rectTransform,
                new Vector2(0.05f, 0.72f),
                new Vector2(0.95f, 0.88f));

            var body = CreateText(
                "Description",
                panel.transform,
                description,
                22,
                TextAnchor.UpperLeft);
            body.color = SubtleText;
            SetTextWrapping(body);
            SetAnchors(
                body.rectTransform,
                new Vector2(0.05f, 0.57f),
                new Vector2(0.95f, 0.73f));

            CreateInfoCard(
                panel.transform,
                "PrimaryCard",
                new Vector2(0.05f, 0.30f),
                new Vector2(0.95f, 0.53f),
                firstCard);

            CreateInfoCard(
                panel.transform,
                "SecondaryCard",
                new Vector2(0.05f, 0.05f),
                new Vector2(0.95f, 0.28f),
                secondCard);

            return panel.gameObject;
        }

        private void CreateInfoCard(
            Transform parent,
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax,
            RouteCard card)
        {
            var panel = CreatePanel(
                name,
                parent,
                anchorMin,
                anchorMax,
                Card);

            var tag = CreateText(
                "Tag",
                panel.transform,
                card.Tag,
                17,
                TextAnchor.UpperLeft);
            tag.color = Active;
            SetAnchors(
                tag.rectTransform,
                new Vector2(0.04f, 0.68f),
                new Vector2(0.96f, 0.92f));

            var title = CreateText(
                "Title",
                panel.transform,
                card.Title,
                27,
                TextAnchor.MiddleLeft);
            SetAnchors(
                title.rectTransform,
                new Vector2(0.04f, 0.42f),
                new Vector2(0.96f, 0.70f));

            var body = CreateText(
                "Body",
                panel.transform,
                card.Body,
                19,
                TextAnchor.UpperLeft);
            body.color = SubtleText;
            SetTextWrapping(body);
            SetAnchors(
                body.rectTransform,
                new Vector2(0.04f, 0.08f),
                new Vector2(0.96f, 0.43f));
        }

        private void BuildWorldCard(
            Transform parent)
        {
            var card = CreatePanel(
                "WorldStatusCard",
                parent,
                new Vector2(0.05f, 0.72f),
                new Vector2(0.74f, 0.82f),
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
                new Vector2(0.62f, 0.90f));

            var headline = CreateText(
                "Headline",
                card.transform,
                "The Hearthwild is stirring.",
                26,
                TextAnchor.MiddleLeft);
            SetAnchors(
                headline.rectTransform,
                new Vector2(0.07f, 0.15f),
                new Vector2(0.62f, 0.62f));

            CreateWorldActionButton(
                card.transform);
        }

        private void CreateWorldActionButton(
            Transform parent)
        {
            var buttonObject = new GameObject(
                "LeafglowCallButton",
                typeof(RectTransform));

            buttonObject.transform.SetParent(
                parent,
                false);

            var rect =
                (RectTransform)buttonObject.transform;

            SetAnchors(
                rect,
                new Vector2(0.65f, 0.18f),
                new Vector2(0.95f, 0.82f));

            var image =
                buttonObject.AddComponent<Image>();
            image.color = Active;

            var button =
                buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(
                () => CompanionCallRequested());

            var label = CreateText(
                "Label",
                buttonObject.transform,
                "Call Leafglow",
                18,
                TextAnchor.MiddleCenter);

            label.color = Background;
            Stretch(label.rectTransform);
        }

        private void BuildQualityBar(
            Transform parent)
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

            for (var index = 0;
                index < tiers.Length;
                index++)
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
            buttonObject.transform.SetParent(
                parent,
                false);

            var rect =
                (RectTransform)buttonObject.transform;

            var width = 1f / count;
            rect.anchorMin =
                new Vector2(
                    index * width + 0.01f,
                    0.12f);

            rect.anchorMax =
                new Vector2(
                    (index + 1) * width - 0.01f,
                    0.88f);

            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image =
                buttonObject.AddComponent<Image>();
            image.color = Inactive;
            _buttonImages[route] = image;

            var button =
                buttonObject.AddComponent<Button>();
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
            buttonObject.transform.SetParent(
                parent,
                false);

            var rect =
                (RectTransform)buttonObject.transform;

            var width = 1f / count;
            rect.anchorMin =
                new Vector2(
                    index * width + 0.025f,
                    0.15f);

            rect.anchorMax =
                new Vector2(
                    (index + 1) * width - 0.025f,
                    0.85f);

            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image =
                buttonObject.AddComponent<Image>();
            image.color = Inactive;
            _qualityImages[tier] = image;

            var button =
                buttonObject.AddComponent<Button>();
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
                tier == GraphicsTier.Standard
                    ? "Std"
                    : tier.ToString(),
                18,
                TextAnchor.MiddleCenter);
            Stretch(label.rectTransform);
        }

        private void SetRoutePanelVisibility(
            AppRoute route)
        {
            foreach (var pair in _routePanels)
            {
                pair.Value.SetActive(
                    pair.Key == route);
            }
        }

        private static string SubtitleFor(
            AppRoute route)
        {
            switch (route)
            {
                case AppRoute.Home:
                    return "Today • Life OS";
                case AppRoute.Quests:
                    return "Campaigns, habits & real-life quests";
                case AppRoute.World:
                    return "The Hearthwild • First World";
                case AppRoute.Hero:
                    return "Waykeeper • Wild Renewal";
                case AppRoute.More:
                    return "Settings • Mobile foundation";
                default:
                    return "Your life builds the world";
            }
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

            panelObject.transform.SetParent(
                parent,
                false);

            var rect =
                (RectTransform)panelObject.transform;

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image =
                panelObject.AddComponent<Image>();

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

            textObject.transform.SetParent(
                parent,
                false);

            var text =
                textObject.AddComponent<Text>();

            text.text = value;
            text.font =
                Resources.GetBuiltinResource<Font>(
                    "LegacyRuntime.ttf");

            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = TextColor;
            text.raycastTarget = false;

            return text;
        }

        private static void SetTextWrapping(
            Text text)
        {
            text.horizontalOverflow =
                HorizontalWrapMode.Wrap;
            text.verticalOverflow =
                VerticalWrapMode.Overflow;
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

            DontDestroyOnLoad(
                eventSystemObject);
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

        private static void Stretch(
            RectTransform rect)
        {
            SetAnchors(
                rect,
                Vector2.zero,
                Vector2.one);
        }

        private sealed class RouteCard
        {
            public RouteCard(
                string tag,
                string title,
                string body)
            {
                Tag = tag;
                Title = title;
                Body = body;
            }

            public string Tag { get; }

            public string Title { get; }

            public string Body { get; }
        }
    }
}
