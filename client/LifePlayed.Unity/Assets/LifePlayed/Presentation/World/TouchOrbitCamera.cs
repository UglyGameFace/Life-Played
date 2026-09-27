using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class TouchOrbitCamera : MonoBehaviour
    {
        private const float BottomUiBlockFraction = 0.20f;

        [SerializeField]
        private float distance = 9f;

        [SerializeField]
        private float minimumDistance = 6.5f;

        [SerializeField]
        private float maximumDistance = 12.5f;

        [SerializeField]
        private float yaw = 18f;

        [SerializeField]
        private float pitch = 23f;

        [SerializeField]
        private float dragSensitivity = 0.10f;

        [SerializeField]
        private float pinchSensitivity = 0.008f;

        private Transform _target;
        private float _lastPinchDistance = -1f;

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            _lastPinchDistance = -1f;
            EnhancedTouchSupport.Disable();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            Snap();
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            if (!HandlePinchZoom())
            {
                HandleOrbitDrag();
            }

            Snap();
        }

        private bool HandlePinchZoom()
        {
            var touches = EnhancedTouch.activeTouches;
            if (touches.Count < 2)
            {
                _lastPinchDistance = -1f;
                return false;
            }

            var first = touches[0].screenPosition;
            var second = touches[1].screenPosition;

            if (IsOverBottomUi(first) ||
                IsOverBottomUi(second))
            {
                _lastPinchDistance = -1f;
                return true;
            }

            var pinchDistance = Vector2.Distance(
                first,
                second);

            if (_lastPinchDistance >= 0f)
            {
                var delta =
                    pinchDistance -
                    _lastPinchDistance;

                distance = Mathf.Clamp(
                    distance -
                    delta * pinchSensitivity,
                    minimumDistance,
                    maximumDistance);
            }

            _lastPinchDistance = pinchDistance;
            return true;
        }

        private void HandleOrbitDrag()
        {
            var pointer = Pointer.current;
            if (pointer == null ||
                !pointer.press.isPressed)
            {
                return;
            }

            var position = pointer.position.ReadValue();
            if (IsOverBottomUi(position))
            {
                return;
            }

            var delta = pointer.delta.ReadValue();
            yaw += delta.x * dragSensitivity;
            pitch -= delta.y * dragSensitivity;
            pitch = Mathf.Clamp(
                pitch,
                8f,
                55f);
        }

        private static bool IsOverBottomUi(
            Vector2 screenPosition)
        {
            return Screen.height > 0 &&
                screenPosition.y <=
                    Screen.height *
                    BottomUiBlockFraction;
        }

        private void Snap()
        {
            if (_target == null)
            {
                return;
            }

            var rotation = Quaternion.Euler(
                pitch,
                yaw,
                0f);

            var focus =
                _target.position +
                Vector3.up * 1.4f;

            transform.position =
                focus +
                rotation *
                new Vector3(
                    0f,
                    0f,
                    -distance);

            transform.rotation =
                Quaternion.LookRotation(
                    focus - transform.position,
                    Vector3.up);
        }
    }
}
