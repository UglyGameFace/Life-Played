using UnityEngine;
using UnityEngine.InputSystem;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class TouchOrbitCamera : MonoBehaviour
    {
        private const float BottomUiBlockFraction = 0.20f;

        [SerializeField]
        private float distance = 9f;

        [SerializeField]
        private float yaw = 18f;

        [SerializeField]
        private float pitch = 23f;

        [SerializeField]
        private float dragSensitivity = 0.10f;

        private Transform _target;

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

            var pointer = Pointer.current;
            if (pointer != null &&
                pointer.press.isPressed &&
                !IsOverBottomUi(pointer.position.ReadValue()))
            {
                var delta = pointer.delta.ReadValue();
                yaw += delta.x * dragSensitivity;
                pitch -= delta.y * dragSensitivity;
                pitch = Mathf.Clamp(pitch, 8f, 55f);
            }

            Snap();
        }

        private static bool IsOverBottomUi(Vector2 screenPosition)
        {
            return Screen.height > 0 &&
                screenPosition.y <= Screen.height * BottomUiBlockFraction;
        }

        private void Snap()
        {
            if (_target == null)
            {
                return;
            }

            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            var focus = _target.position + Vector3.up * 1.4f;
            transform.position =
                focus +
                rotation * new Vector3(0f, 0f, -distance);
            transform.rotation = Quaternion.LookRotation(
                focus - transform.position,
                Vector3.up);
        }
    }
}
