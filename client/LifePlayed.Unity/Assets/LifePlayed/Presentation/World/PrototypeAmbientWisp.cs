using UnityEngine;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class PrototypeAmbientWisp : MonoBehaviour
    {
        private Vector3 _center;
        private float _radius;
        private float _speed;
        private float _phase;
        private float _bobHeight;

        public void Configure(
            Vector3 center,
            float radius,
            float speed,
            float phase,
            float bobHeight)
        {
            _center = center;
            _radius = radius;
            _speed = speed;
            _phase = phase;
            _bobHeight = bobHeight;
            Apply(0f);
        }

        private void Update()
        {
            Apply(Time.time);
        }

        private void Apply(float time)
        {
            var angle = (_phase + time * _speed) * Mathf.Deg2Rad;
            var vertical = Mathf.Sin(angle * 1.7f) * _bobHeight;

            transform.localPosition =
                _center +
                new Vector3(
                    Mathf.Cos(angle) * _radius,
                    vertical,
                    Mathf.Sin(angle) * _radius);
        }
    }
}
