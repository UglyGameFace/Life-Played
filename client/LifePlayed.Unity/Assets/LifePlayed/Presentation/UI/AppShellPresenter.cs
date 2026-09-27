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

        private AppCoordinator _coordinator;
        private Text _title;

        private static readonly Color32 Background = new Color32(13, 21, 18, 238);
        private static readonly Color32 Panel = new Color32(24, 38, 31, 245);
        private static readonly Color32 Active = new Color32(106, 224, 145, 255);
        private static readonly Color32 Inactive = new Color32(76, 96, 87, 255);
        private static readonly Color32 TextColor = new Color32(238, 248, 241, 255);

        private void Awake()
        {
            BuildUi();
        }

        public void Bind(AppCoordinator coordinator)
        {
            if (_coordinator != null)
            {
                _coordinator.RouteChanged -= OnRouteChanged;
            }

            _coordinator = coordinator;
            _coordinator.RouteChanged += OnRouteChanged;
            OnRouteChanged(_coordinator.CurrentRoute);
        }

        private void OnDestroy()
        {
            if (_coordinator != null)
            {
                _coordinator.RouteChanged -= OnRouteChanged;
            }
        }

        private void OnRouteChanged(AppRoute route)
        {
            if (_title != null)
            {
                _title.text = route == AppRoute.World
                    ? "Wild Renewal"
                    : route.ToString();
            }

            foreach (var pair in _buttonImages)
            {
                pair.Value.color = pair.Key == route ? Active : Inactive;
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

            var background = CreatePanel(
                "Background",
                transform,
                Vector2.zero,
                Vector2.one,
                Background);

            var safe = CreatePanel(
                "SafeArea",
                background.transform,
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
            var titleRect = _title.rectTransform;
            titleRect.anchorMin = new Vector2(0.05f, 0.90f);
            titleRect.anchorMax = new Vector2(0.95f, 0.98f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            var subtitle = CreateText(
                "Status",
                safe.transform,
                "Your life builds the world",
                26,
                TextAnchor.UpperLeft);
            subtitle.color = new Color32(157, 191, 169, 255);
            var subtitleRect = subtitle.rectTransform;
            subtitleRect.anchorMin = new Vector2(0.05f, 0.84f);
            subtitleRect.anchorMax = new Vector2(0.95f, 0.90f);
            subtitleRect.offsetMin = Vector2.zero;
            subtitleRect.offsetMax = Vector2.zero;

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
                CreateNavigationButton(nav.transform, routes[index], index, routes.Length);
            }
        }

        private void CreateNavigationButton(
            Transform parent,
            AppRoute route,
            int index,
            int count)
        {
            var buttonObject = new GameObject(route + "Button", typeof(RectTransform));
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
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;
        }

        private static Image CreatePanel(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Color color)
        {
            var panelObject = new GameObject(name, typeof(RectTransform));
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
            var textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);

            var text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
    }
}
