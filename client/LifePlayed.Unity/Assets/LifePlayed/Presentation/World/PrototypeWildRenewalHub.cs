using LifePlayed.Client.Application;
using UnityEngine;
using UnityEngine.Rendering;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class PrototypeWildRenewalHub : MonoBehaviour
    {
        private Material _grass;
        private Material _grassLight;
        private Material _wood;
        private Material _stone;
        private Material _stoneDark;
        private Material _glow;
        private Material _hero;
        private Material _soil;
        private Material _water;
        private IGraphicsQualityController _graphicsQuality;
        private Light _sunLight;
        private Light _hearthLight;
        private GameObject _wispRoot;
        private PrototypeCompanionPresenter _leafglowFox;

        public bool IsPresentationActive { get; private set; } = true;

        private void Awake()
        {
            Build();
        }

        private void Build()
        {
            CreateMaterials();
            ConfigureAtmosphere();
            CreateLight();
            CreateIslandLayers();
            CreateMossPath();
            CreateHearth();
            CreateTrees();
            CreateWaystones();
            CreateDormantLandmarks();
            CreateAmbientWisps();

            var heroRoot = new GameObject("Waykeeper");
            heroRoot.transform.SetParent(transform, false);
            heroRoot.transform.position = new Vector3(0f, 0.34f, -1.45f);

            var character = heroRoot.AddComponent<PrototypeCharacterPresenter>();
            character.Build(_hero, _wood, _glow);

            var companionRoot = new GameObject("LeafglowFox");
            companionRoot.transform.SetParent(transform, false);
            companionRoot.transform.position = new Vector3(1.55f, 0.42f, -0.72f);

            _leafglowFox =
                companionRoot.AddComponent<PrototypeCompanionPresenter>();
            _leafglowFox.Build(
                _grassLight,
                _glow);

            var cameraFocus = new GameObject("WorldCameraFocus");
            cameraFocus.transform.SetParent(transform, false);
            cameraFocus.transform.position = new Vector3(0f, 0.45f, 0.62f);

            var camera = EnsureCamera();
            var orbit = camera.GetComponent<TouchOrbitCamera>();
            if (orbit == null)
            {
                orbit = camera.gameObject.AddComponent<TouchOrbitCamera>();
            }

            camera.transform.SetParent(transform, true);
            orbit.SetTarget(cameraFocus.transform);
        }

        public void BindGraphicsQuality(
            IGraphicsQualityController graphicsQuality)
        {
            if (_graphicsQuality != null)
            {
                _graphicsQuality.TierChanged -= OnTierChanged;
            }

            _graphicsQuality = graphicsQuality;
            _graphicsQuality.TierChanged += OnTierChanged;
            ApplyVisualQuality(_graphicsQuality.CurrentTier);
        }

        public void PlayCompanionReaction()
        {
            if (IsPresentationActive &&
                _leafglowFox != null)
            {
                _leafglowFox.PlayReaction();
            }
        }

        public void SetPresentationActive(bool active)
        {
            IsPresentationActive = active;

            for (var index = 0; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(active);
            }

            if (_graphicsQuality != null)
            {
                ApplyVisualQuality(_graphicsQuality.CurrentTier);
            }
        }

        private void OnDestroy()
        {
            if (_graphicsQuality != null)
            {
                _graphicsQuality.TierChanged -= OnTierChanged;
            }
        }

        private void OnTierChanged(GraphicsTier tier)
        {
            ApplyVisualQuality(tier);
        }

        private void ApplyVisualQuality(GraphicsTier tier)
        {
            if (_sunLight != null)
            {
                _sunLight.shadows = !IsPresentationActive
                    ? LightShadows.None
                    : tier == GraphicsTier.High
                        ? LightShadows.Soft
                        : tier == GraphicsTier.Standard
                            ? LightShadows.Hard
                            : LightShadows.None;
            }

            if (_hearthLight != null)
            {
                _hearthLight.enabled =
                    IsPresentationActive &&
                    tier != GraphicsTier.Reduced;

                _hearthLight.intensity =
                    tier == GraphicsTier.High
                        ? 2.2f
                        : 1.55f;
            }

            if (_wispRoot != null)
            {
                _wispRoot.SetActive(
                    IsPresentationActive &&
                    tier != GraphicsTier.Reduced);
            }
        }

        private void CreateMaterials()
        {
            _grass = CreateMaterial(
                new Color32(52, 116, 73, 255),
                0.05f,
                0.18f);

            _grassLight = CreateMaterial(
                new Color32(105, 166, 93, 255),
                0.02f,
                0.22f);

            _wood = CreateMaterial(
                new Color32(124, 81, 52, 255),
                0.03f,
                0.24f);

            _stone = CreateMaterial(
                new Color32(100, 114, 105, 255),
                0.0f,
                0.20f);

            _stoneDark = CreateMaterial(
                new Color32(59, 74, 68, 255),
                0.0f,
                0.14f);

            _glow = CreateMaterial(
                new Color32(104, 238, 167, 255),
                0.0f,
                0.18f,
                new Color32(86, 255, 166, 255));

            _hero = CreateMaterial(
                new Color32(59, 69, 65, 255),
                0.04f,
                0.34f);

            _soil = CreateMaterial(
                new Color32(74, 61, 47, 255),
                0.0f,
                0.12f);

            _water = CreateMaterial(
                new Color32(48, 106, 105, 255),
                0.08f,
                0.72f);
        }

        private void ConfigureAtmosphere()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color32(91, 123, 105, 255);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color32(29, 54, 47, 255);
            RenderSettings.fogDensity = 0.010f;
        }

        private void CreateIslandLayers()
        {
            CreatePrimitive(
                "HearthwildSoil",
                PrimitiveType.Cylinder,
                new Vector3(0f, -0.52f, 0f),
                new Vector3(8.0f, 0.46f, 8.0f),
                Vector3.zero,
                _soil);

            CreatePrimitive(
                "HearthwildIsland",
                PrimitiveType.Cylinder,
                new Vector3(0f, -0.24f, 0f),
                new Vector3(7.65f, 0.22f, 7.65f),
                Vector3.zero,
                _grass);

            CreatePrimitive(
                "RainPool",
                PrimitiveType.Cylinder,
                new Vector3(3.85f, -0.06f, 0.55f),
                new Vector3(1.15f, 0.035f, 1.15f),
                Vector3.zero,
                _water);

            for (var index = 0; index < 12; index++)
            {
                var angle = index * Mathf.PI * 2f / 12f;
                var radius = 6.9f + (index % 3) * 0.18f;

                CreatePrimitive(
                    "IslandEdgeStone_" + index,
                    PrimitiveType.Sphere,
                    new Vector3(
                        Mathf.Cos(angle) * radius,
                        -0.10f,
                        Mathf.Sin(angle) * radius),
                    new Vector3(
                        0.72f + (index % 2) * 0.16f,
                        0.36f,
                        0.58f),
                    new Vector3(
                        index * 3f,
                        -angle * Mathf.Rad2Deg,
                        index % 2 == 0 ? 7f : -5f),
                    _stoneDark);
            }
        }

        private void CreateMossPath()
        {
            var pathRoot = new GameObject("MossPath");
            pathRoot.transform.SetParent(transform, false);

            for (var index = 0; index < 9; index++)
            {
                var t = index / 8f;
                var x = Mathf.Sin(index * 1.27f) * 0.20f;
                var z = Mathf.Lerp(-3.65f, 1.40f, t);
                var scale = 0.72f + (index % 3) * 0.08f;

                CreatePrimitive(
                    "PathStone_" + index,
                    PrimitiveType.Cylinder,
                    new Vector3(x, 0.02f, z),
                    new Vector3(scale, 0.07f, scale * 0.82f),
                    new Vector3(0f, index * 13f, 0f),
                    index % 3 == 0 ? _grassLight : _stone,
                    pathRoot.transform);
            }
        }

        private void CreateHearth()
        {
            var hearthRoot = new GameObject("CentralHearth");
            hearthRoot.transform.SetParent(transform, false);
            hearthRoot.transform.localPosition = new Vector3(0f, 0f, 2.28f);

            for (var index = 0; index < 10; index++)
            {
                var angle = index * Mathf.PI * 2f / 10f;

                CreatePrimitive(
                    "HearthStone_" + index,
                    PrimitiveType.Sphere,
                    new Vector3(
                        Mathf.Cos(angle) * 1.12f,
                        0.16f,
                        Mathf.Sin(angle) * 1.12f),
                    new Vector3(0.54f, 0.26f, 0.40f),
                    new Vector3(0f, -angle * Mathf.Rad2Deg, 0f),
                    _stone,
                    hearthRoot.transform);
            }

            CreatePrimitive(
                "HearthBasin",
                PrimitiveType.Cylinder,
                new Vector3(0f, 0.10f, 0f),
                new Vector3(0.90f, 0.12f, 0.90f),
                Vector3.zero,
                _stoneDark,
                hearthRoot.transform);

            var bloomRoot = new GameObject("QuietBloom");
            bloomRoot.transform.SetParent(hearthRoot.transform, false);
            bloomRoot.transform.localPosition = new Vector3(0f, 0.62f, 0f);

            for (var index = 0; index < 5; index++)
            {
                var angle = index * 360f / 5f;

                CreatePrimitive(
                    "BloomPetal_" + index,
                    PrimitiveType.Sphere,
                    Quaternion.Euler(0f, angle, 0f) *
                        new Vector3(0f, 0f, 0.34f),
                    new Vector3(0.22f, 0.10f, 0.45f),
                    new Vector3(20f, angle, 0f),
                    _glow,
                    bloomRoot.transform);
            }

            CreatePrimitive(
                "BloomCore",
                PrimitiveType.Sphere,
                Vector3.zero,
                Vector3.one * 0.34f,
                Vector3.zero,
                _glow,
                bloomRoot.transform);

            var hearthLightObject = new GameObject("HearthLight");
            hearthLightObject.transform.SetParent(hearthRoot.transform, false);
            hearthLightObject.transform.localPosition = new Vector3(0f, 1.05f, 0f);

            _hearthLight = hearthLightObject.AddComponent<Light>();
            _hearthLight.type = LightType.Point;
            _hearthLight.color = new Color32(112, 255, 181, 255);
            _hearthLight.intensity = 2.2f;
            _hearthLight.range = 5.5f;
            _hearthLight.shadows = LightShadows.None;
        }

        private void CreateTrees()
        {
            var treeRoot = new GameObject("HearthwildGrove");
            treeRoot.transform.SetParent(transform, false);

            for (var index = 0; index < 12; index++)
            {
                var angle = index * Mathf.PI * 2f / 12f + 0.18f;
                var radius = 5.25f + (index % 2) * 0.82f;
                var height = 1.95f + (index % 4) * 0.22f;
                var position = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0f,
                    Mathf.Sin(angle) * radius);

                var tree = new GameObject("RenewalTree_" + index);
                tree.transform.SetParent(treeRoot.transform, false);
                tree.transform.localPosition = position;
                tree.transform.localRotation = Quaternion.Euler(
                    index % 3 == 0 ? 3f : -2f,
                    index * 7f,
                    index % 2 == 0 ? 2f : -3f);

                CreatePrimitive(
                    "Trunk",
                    PrimitiveType.Cylinder,
                    Vector3.up * height * 0.50f,
                    new Vector3(0.28f, height * 0.50f, 0.28f),
                    Vector3.zero,
                    _wood,
                    tree.transform);

                CreatePrimitive(
                    "CrownA",
                    PrimitiveType.Sphere,
                    new Vector3(-0.26f, height + 0.26f, 0f),
                    new Vector3(1.55f, 0.95f, 1.45f),
                    Vector3.zero,
                    index % 3 == 0 ? _grassLight : _grass,
                    tree.transform);

                CreatePrimitive(
                    "CrownB",
                    PrimitiveType.Sphere,
                    new Vector3(0.30f, height + 0.54f, 0.18f),
                    new Vector3(1.35f, 0.88f, 1.30f),
                    Vector3.zero,
                    _grass,
                    tree.transform);

                CreatePrimitive(
                    "CrownC",
                    PrimitiveType.Sphere,
                    new Vector3(0f, height + 0.82f, -0.12f),
                    new Vector3(1.12f, 0.72f, 1.10f),
                    Vector3.zero,
                    _grassLight,
                    tree.transform);
            }
        }

        private void CreateWaystones()
        {
            var waystoneRoot = new GameObject("Waystones");
            waystoneRoot.transform.SetParent(transform, false);

            for (var index = 0; index < 6; index++)
            {
                var angle = index * Mathf.PI * 2f / 6f + 0.40f;

                var stone = CreatePrimitive(
                    "Waystone_" + index,
                    PrimitiveType.Cube,
                    new Vector3(
                        Mathf.Cos(angle) * 3.95f,
                        0.55f,
                        Mathf.Sin(angle) * 3.95f),
                    new Vector3(0.46f, 1.14f, 0.32f),
                    new Vector3(
                        4f * index,
                        -angle * Mathf.Rad2Deg,
                        index % 2 == 0 ? 5f : -6f),
                    index == 0 ? _glow : _stoneDark,
                    waystoneRoot.transform);

                if (index > 0)
                {
                    CreatePrimitive(
                        "WaystoneRune_" + index,
                        PrimitiveType.Sphere,
                        new Vector3(0f, 0.12f, 0.55f),
                        new Vector3(0.10f, 0.16f, 0.06f),
                        Vector3.zero,
                        _glow,
                        stone.transform);
                }
            }
        }

        private void CreateDormantLandmarks()
        {
            var landmarks = new GameObject("DormantLandmarks");
            landmarks.transform.SetParent(transform, false);

            var workshop = new GameObject("DormantWorkshop");
            workshop.transform.SetParent(landmarks.transform, false);
            workshop.transform.localPosition = new Vector3(-4.15f, 0f, 1.25f);

            CreatePrimitive(
                "WorkshopFloor",
                PrimitiveType.Cylinder,
                new Vector3(0f, 0.04f, 0f),
                new Vector3(1.25f, 0.08f, 1.25f),
                Vector3.zero,
                _stoneDark,
                workshop.transform);

            CreatePrimitive(
                "WorkshopPostLeft",
                PrimitiveType.Cube,
                new Vector3(-0.72f, 0.86f, 0f),
                new Vector3(0.18f, 1.72f, 0.18f),
                new Vector3(0f, 0f, -4f),
                _wood,
                workshop.transform);

            CreatePrimitive(
                "WorkshopPostRight",
                PrimitiveType.Cube,
                new Vector3(0.72f, 0.86f, 0f),
                new Vector3(0.18f, 1.72f, 0.18f),
                new Vector3(0f, 0f, 4f),
                _wood,
                workshop.transform);

            CreatePrimitive(
                "WorkshopBeam",
                PrimitiveType.Cube,
                new Vector3(0f, 1.62f, 0f),
                new Vector3(1.70f, 0.18f, 0.22f),
                Vector3.zero,
                _wood,
                workshop.transform);

            var gatheringPlace = new GameObject("GatheringPlace");
            gatheringPlace.transform.SetParent(landmarks.transform, false);
            gatheringPlace.transform.localPosition = new Vector3(4.10f, 0f, 2.15f);

            for (var index = 0; index < 7; index++)
            {
                var angle = index * Mathf.PI * 2f / 7f;

                CreatePrimitive(
                    "GatheringStone_" + index,
                    PrimitiveType.Sphere,
                    new Vector3(
                        Mathf.Cos(angle) * 1.18f,
                        0.14f,
                        Mathf.Sin(angle) * 1.18f),
                    new Vector3(0.60f, 0.26f, 0.46f),
                    Vector3.zero,
                    _stoneDark,
                    gatheringPlace.transform);
            }

            var arch = new GameObject("GroveArch");
            arch.transform.SetParent(landmarks.transform, false);
            arch.transform.localPosition = new Vector3(0f, 0f, 5.25f);

            CreatePrimitive(
                "ArchLeft",
                PrimitiveType.Cube,
                new Vector3(-0.92f, 1.10f, 0f),
                new Vector3(0.32f, 2.20f, 0.42f),
                new Vector3(0f, 0f, -7f),
                _stoneDark,
                arch.transform);

            CreatePrimitive(
                "ArchRight",
                PrimitiveType.Cube,
                new Vector3(0.92f, 1.10f, 0f),
                new Vector3(0.32f, 2.20f, 0.42f),
                new Vector3(0f, 0f, 7f),
                _stoneDark,
                arch.transform);

            CreatePrimitive(
                "ArchCrown",
                PrimitiveType.Cube,
                new Vector3(0f, 2.15f, 0f),
                new Vector3(2.14f, 0.30f, 0.46f),
                Vector3.zero,
                _grass,
                arch.transform);
        }

        private void CreateAmbientWisps()
        {
            _wispRoot = new GameObject("BloomWisps");
            _wispRoot.transform.SetParent(transform, false);

            for (var index = 0; index < 8; index++)
            {
                var wisp = CreatePrimitive(
                    "BloomWisp_" + index,
                    PrimitiveType.Sphere,
                    Vector3.zero,
                    Vector3.one * (0.09f + (index % 3) * 0.025f),
                    Vector3.zero,
                    _glow,
                    _wispRoot.transform);

                var motion = wisp.AddComponent<PrototypeAmbientWisp>();
                motion.Configure(
                    new Vector3(
                        (index % 2 == 0 ? -1f : 1f) * (1.1f + index * 0.20f),
                        0.80f + (index % 4) * 0.32f,
                        1.2f + (index % 3) * 0.75f),
                    0.22f + (index % 3) * 0.08f,
                    22f + index * 4f,
                    index * 41f,
                    0.16f + (index % 2) * 0.08f);
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
            lightObject.transform.rotation = Quaternion.Euler(46f, -32f, 0f);

            _sunLight = lightObject.AddComponent<Light>();
            _sunLight.type = LightType.Directional;
            _sunLight.intensity = 1.30f;
            _sunLight.color = new Color32(255, 232, 199, 255);
            _sunLight.shadows = LightShadows.Hard;
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
            camera.backgroundColor = new Color32(24, 45, 39, 255);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 120f;
            camera.fieldOfView = 43f;

            return camera;
        }

        private GameObject CreatePrimitive(
            string name,
            PrimitiveType primitiveType,
            Vector3 localPosition,
            Vector3 localScale,
            Vector3 localEuler,
            Material material,
            Transform parentOverride = null)
        {
            var primitive = GameObject.CreatePrimitive(primitiveType);
            primitive.name = name;
            primitive.transform.SetParent(
                parentOverride != null ? parentOverride : transform,
                false);
            primitive.transform.localPosition = localPosition;
            primitive.transform.localScale = localScale;
            primitive.transform.localRotation = Quaternion.Euler(localEuler);
            primitive.GetComponent<Renderer>().sharedMaterial = material;

            var collider = primitive.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            return primitive;
        }

        private static Material CreateMaterial(
            Color color,
            float metallic,
            float smoothness,
            Color? emission = null)
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

            if (emission.HasValue && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", emission.Value * 1.8f);
            }

            return material;
        }
    }
}
