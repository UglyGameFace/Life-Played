using UnityEngine;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class PrototypeCompanionPresenter : MonoBehaviour
    {
        private Vector3 _origin;
        private Transform _tailRoot;
        private float _reactionStartedAt = -10f;
        private const float ReactionDuration = 1.15f;

        public bool IsReacting =>
            Time.time - _reactionStartedAt <
            ReactionDuration;

        public void Build(Material bodyMaterial, Material glowMaterial)
        {
            _origin = transform.position;

            CreatePart(
                "LeafglowBody",
                PrimitiveType.Sphere,
                new Vector3(0f, 0.34f, 0f),
                new Vector3(0.88f, 0.55f, 1.15f),
                Vector3.zero,
                bodyMaterial);

            CreatePart(
                "LeafglowChest",
                PrimitiveType.Sphere,
                new Vector3(0f, 0.48f, 0.50f),
                new Vector3(0.62f, 0.66f, 0.62f),
                Vector3.zero,
                bodyMaterial);

            CreatePart(
                "LeafglowHead",
                PrimitiveType.Sphere,
                new Vector3(0f, 0.70f, 0.82f),
                new Vector3(0.66f, 0.62f, 0.70f),
                Vector3.zero,
                bodyMaterial);

            CreatePart(
                "LeafglowMuzzle",
                PrimitiveType.Sphere,
                new Vector3(0f, 0.58f, 1.18f),
                new Vector3(0.34f, 0.28f, 0.38f),
                Vector3.zero,
                glowMaterial);

            CreatePart(
                "LeftEar",
                PrimitiveType.Cylinder,
                new Vector3(-0.24f, 1.10f, 0.84f),
                new Vector3(0.13f, 0.30f, 0.13f),
                new Vector3(15f, 0f, -20f),
                glowMaterial);

            CreatePart(
                "RightEar",
                PrimitiveType.Cylinder,
                new Vector3(0.24f, 1.10f, 0.84f),
                new Vector3(0.13f, 0.30f, 0.13f),
                new Vector3(15f, 0f, 20f),
                glowMaterial);

            CreateLeg("FrontLeftPaw", new Vector3(-0.28f, 0.02f, 0.48f), bodyMaterial);
            CreateLeg("FrontRightPaw", new Vector3(0.28f, 0.02f, 0.48f), bodyMaterial);
            CreateLeg("BackLeftPaw", new Vector3(-0.30f, 0.02f, -0.42f), bodyMaterial);
            CreateLeg("BackRightPaw", new Vector3(0.30f, 0.02f, -0.42f), bodyMaterial);

            var tailRootObject = new GameObject("LeafglowTail");
            tailRootObject.transform.SetParent(transform, false);
            tailRootObject.transform.localPosition = new Vector3(0f, 0.48f, -0.72f);
            _tailRoot = tailRootObject.transform;

            CreatePart(
                "TailBase",
                PrimitiveType.Capsule,
                new Vector3(0f, 0.30f, -0.22f),
                new Vector3(0.24f, 0.62f, 0.24f),
                new Vector3(48f, 0f, 0f),
                bodyMaterial,
                _tailRoot);

            CreatePart(
                "TailTip",
                PrimitiveType.Capsule,
                new Vector3(0f, 0.78f, -0.58f),
                new Vector3(0.21f, 0.48f, 0.21f),
                new Vector3(56f, 0f, 0f),
                glowMaterial,
                _tailRoot);

            CreatePart(
                "BloomCharm",
                PrimitiveType.Sphere,
                new Vector3(0f, 0.34f, 1.02f),
                Vector3.one * 0.18f,
                Vector3.zero,
                glowMaterial);
        }

        public void PlayReaction()
        {
            _reactionStartedAt = Time.time;
        }

        private void Update()
        {
            var time = Time.time;
            var bob = Mathf.Sin(time * 2.2f) * 0.06f;
            var reactionProgress = Mathf.Clamp01(
                (time - _reactionStartedAt) /
                ReactionDuration);

            var reacting =
                time - _reactionStartedAt <
                ReactionDuration;

            var hop = reacting
                ? Mathf.Sin(
                    reactionProgress * Mathf.PI) *
                    0.42f
                : 0f;

            var spin = reacting
                ? reactionProgress * 360f
                : Mathf.Sin(time * 0.85f) * 7f;

            transform.position =
                _origin +
                Vector3.up * (bob + hop);

            transform.localRotation =
                Quaternion.Euler(
                    0f,
                    spin,
                    0f);

            transform.localScale =
                Vector3.one *
                (reacting
                    ? 1f +
                        Mathf.Sin(
                            reactionProgress *
                            Mathf.PI) *
                        0.08f
                    : 1f);

            if (_tailRoot != null)
            {
                _tailRoot.localRotation = Quaternion.Euler(
                    0f,
                    Mathf.Sin(time * 2.8f) * 18f,
                    Mathf.Sin(time * 2.2f) * 8f);
            }
        }

        private void CreateLeg(
            string name,
            Vector3 localPosition,
            Material material)
        {
            CreatePart(
                name,
                PrimitiveType.Capsule,
                localPosition,
                new Vector3(0.18f, 0.28f, 0.18f),
                Vector3.zero,
                material);
        }

        private void CreatePart(
            string name,
            PrimitiveType primitiveType,
            Vector3 localPosition,
            Vector3 localScale,
            Vector3 localEuler,
            Material material,
            Transform parentOverride = null)
        {
            var part = GameObject.CreatePrimitive(primitiveType);
            part.name = name;
            part.transform.SetParent(parentOverride != null ? parentOverride : transform, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            part.transform.localRotation = Quaternion.Euler(localEuler);
            part.GetComponent<Renderer>().sharedMaterial = material;

            var collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
        }
    }
}
