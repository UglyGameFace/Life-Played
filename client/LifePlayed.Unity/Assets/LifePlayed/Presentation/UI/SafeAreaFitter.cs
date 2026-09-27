using UnityEngine;

namespace LifePlayed.Client.Presentation.UI
{
    [DisallowMultipleComponent]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _lastSafeArea;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            Apply();
        }

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
            {
                Apply();
            }
        }

        private void Apply()
        {
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }

            if (_rectTransform == null || Screen.width <= 0 || Screen.height <= 0)
            {
                return;
            }

            _lastSafeArea = Screen.safeArea;
            var anchorMin = _lastSafeArea.position;
            var anchorMax = _lastSafeArea.position + _lastSafeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }
    }
}
