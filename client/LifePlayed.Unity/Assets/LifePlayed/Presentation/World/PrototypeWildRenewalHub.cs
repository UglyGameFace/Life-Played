using UnityEngine;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class PrototypeWildRenewalHub : MonoBehaviour
    {
        private Material _grass;
        private Material _wood;
        private Material _stone;
        private Material _glow;
        private Material _hero;

        private void Awake()
        {
            Build();
        }

        private void Build()
        {
            CreateMaterials();
            CreateLight();
            CreateGround();
            CreateHearth();
            CreateTrees();
            CreateWaystones();

            var heroRoot = new GameObject("Waykeeper");
            heroRoot.transform.SetParent(transform, false);
            heroRoot.transform.position = new Vector3(0f, 0.3f, 0f);
            var character = heroRoot.AddComponent<PrototypeCharacterPresenter>();
            character.Build(_hero, _wood);

            var companionRoot = new GameObject("StarterCompanion");
            companionRoot.transform.SetParent(transform, false);
            companionRoot.transform.position = new Vector3(1.6f, 1.15f, 0.75f);
            var companion = companionRoot.AddComponent<PrototypeCompanionPresenter>();
            companion.Build(_grass, _glow);

            var camera = EnsureCamera();
            var orbit = camera.GetComponent<TouchOrbitCamera>();
            if (orbit == null)
            {
                orbit = camera.gameObject.AddComponent<TouchOrbitCamera>();
            }

            orbit.SetTarget(heroRoot.transform);
        }

        private void CreateMaterials()
        {
            _grass = CreateMaterial(new Color32(59, 118, 76, 255), 0.15f, 0.18f);
            _wood = CreateMaterial(new Color32(130, 84, 53, 255), 0.05f, 0.28f);
            _stone = CreateMaterial(new Color32(93, 108, 101, 255), 0.0f, 0.20f);
            _glow = CreateMaterial(new Color32(109, 245, 174, 255), 0.0f, 0.08f);
            _hero = CreateMaterial(new Color32(63, 73, 68, 255), 0.0f, 0.35f);
        }

        private void CreateGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ground.name = "HearthwildIsland";
            ground.transform.SetParent(transform, false);
            ground.transform.localPosition = new Vector3(0f, -0.30f, 0f);
            ground.transform.localScale = new Vector3(7.6f, 0.28f, 7.6f);
            ground.GetComponent<Renderer>().sharedMaterial = _grass;
        }

        private void CreateHearth()
        {
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "CentralHearth";
            ring.transform.SetParent(transform, false);
            ring.transform.localPosition = new Vector3(0f, 0.05f, 2.25f);
            ring.transform.localScale = new Vector3(1.1f, 0.18f, 1.1f);
            ring.GetComponent<Renderer>().sharedMaterial = _stone;

            var bloom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bloom.name = "QuietBloom";
            bloom.transform.SetParent(transform, false);
            bloom.transform.localPosition = new Vector3(0f, 0.62f, 2.25f);
            bloom.transform.localScale = Vector3.one * 0.48f;
            bloom.GetComponent<Renderer>().sharedMaterial = _glow;
        }

        private void CreateTrees()
        {
            for (var index = 0; index < 9; index++)
            {
                var angle = index * Mathf.PI * 2f / 9f;
                var radius = 5.2f + (index % 2) * 0.7f;
                var position = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius);

                var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                trunk.name = "RenewalTree_" + index;
                trunk.transform.SetParent(transform, false);
                trunk.transform.localPosition = position + Vector3.up * 1.0f;
                trunk.transform.localScale = new Vector3(0.32f, 1.0f + index % 3 * 0.12f, 0.32f);
                trunk.GetComponent<Renderer>().sharedMaterial = _wood;

                var crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                crown.name = "Crown";
                crown.transform.SetParent(trunk.transform, false);
                crown.transform.localPosition = new Vector3(0f, 1.45f, 0f);
                crown.transform.localScale = new Vector3(2.6f, 1.8f, 2.6f);
                crown.GetComponent<Renderer>().sharedMaterial = _grass;
            }
        }

        private void CreateWaystones()
        {
            for (var index = 0; index < 6; index++)
            {
                var angle = index * Mathf.PI * 2f / 6f + 0.4f;
                var stone = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stone.name = "Waystone_" + index;
                stone.transform.SetParent(transform, false);
                stone.transform.localPosition = new Vector3(
                    Mathf.Cos(angle) * 3.8f,
                    0.55f,
                    Mathf.Sin(angle) * 3.8f);
                stone.transform.localScale = new Vector3(0.45f, 1.1f, 0.32f);
                stone.transform.localRotation = Quaternion.Euler(
                    4f * index,
                    -angle * Mathf.Rad2Deg,
                    index % 2 == 0 ? 5f : -6f);
                stone.GetComponent<Renderer>().sharedMaterial =
                    index == 0 ? _glow : _stone;
            }
        }

        private void CreateLight()
        {
            if (FindFirstObjectByType<Light>() != null)
            {
                return;
            }

            var lightObject = new GameObject("Sunlight");
            lightObject.transform.SetParent(transform, false);
            lightObject.transform.rotation = Quaternion.Euler(42f, -28f, 0f);

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            light.color = new Color32(255, 238, 207, 255);
            light.shadows = LightShadows.Soft;
        }

        private static Camera EnsureCamera()
        {
            if (Camera.main != null)
            {
                return Camera.main;
            }

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(25, 45, 39, 255);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 120f;
            return camera;
        }

        private static Material CreateMaterial(
            Color color,
            float metallic,
            float smoothness)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            var material = new Material(shader);
            material.color = color;

            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", metallic);
            }

            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", smoothness);
            }

            return material;
        }
    }
}
