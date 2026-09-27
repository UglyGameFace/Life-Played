using UnityEngine;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class PrototypeCharacterPresenter : MonoBehaviour
    {
        public Transform Build(
            Material bodyMaterial,
            Material accentMaterial,
            Material glowMaterial)
        {
            CreatePart(
                "HeroBody",
                PrimitiveType.Capsule,
                new Vector3(0f, 1.20f, 0f),
                new Vector3(0.68f, 0.88f, 0.68f),
                Vector3.zero,
                bodyMaterial);

            CreatePart(
                "HeroChest",
                PrimitiveType.Sphere,
                new Vector3(0f, 1.55f, 0f),
                new Vector3(0.86f, 0.68f, 0.62f),
                Vector3.zero,
                bodyMaterial);

            CreatePart(
                "HeroHead",
                PrimitiveType.Sphere,
                new Vector3(0f, 2.28f, 0.02f),
                Vector3.one * 0.72f,
                Vector3.zero,
                accentMaterial);

            CreatePart(
                "WaykeeperMantle",
                PrimitiveType.Cylinder,
                new Vector3(0f, 1.84f, -0.01f),
                new Vector3(0.62f, 0.10f, 0.62f),
                Vector3.zero,
                accentMaterial);

            CreatePart(
                "LeftArm",
                PrimitiveType.Capsule,
                new Vector3(-0.52f, 1.32f, 0f),
                new Vector3(0.22f, 0.55f, 0.22f),
                new Vector3(0f, 0f, -10f),
                bodyMaterial);

            CreatePart(
                "RightArm",
                PrimitiveType.Capsule,
                new Vector3(0.52f, 1.32f, 0f),
                new Vector3(0.22f, 0.55f, 0.22f),
                new Vector3(0f, 0f, 10f),
                bodyMaterial);

            CreatePart(
                "LeftLeg",
                PrimitiveType.Capsule,
                new Vector3(-0.22f, 0.48f, 0f),
                new Vector3(0.27f, 0.58f, 0.27f),
                Vector3.zero,
                bodyMaterial);

            CreatePart(
                "RightLeg",
                PrimitiveType.Capsule,
                new Vector3(0.22f, 0.48f, 0f),
                new Vector3(0.27f, 0.58f, 0.27f),
                Vector3.zero,
                bodyMaterial);

            CreatePart(
                "WaykeeperPack",
                PrimitiveType.Cube,
                new Vector3(0f, 1.38f, -0.52f),
                new Vector3(0.62f, 0.78f, 0.28f),
                new Vector3(4f, 0f, 0f),
                accentMaterial);

            CreatePart(
                "WaykeeperSigil",
                PrimitiveType.Sphere,
                new Vector3(0f, 1.58f, 0.48f),
                Vector3.one * 0.14f,
                Vector3.zero,
                glowMaterial);

            return transform;
        }

        private void CreatePart(
            string name,
            PrimitiveType primitiveType,
            Vector3 localPosition,
            Vector3 localScale,
            Vector3 localEuler,
            Material material)
        {
            var part = GameObject.CreatePrimitive(primitiveType);
            part.name = name;
            part.transform.SetParent(transform, false);
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
