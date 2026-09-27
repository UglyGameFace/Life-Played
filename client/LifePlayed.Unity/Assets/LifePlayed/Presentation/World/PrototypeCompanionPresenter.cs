using UnityEngine;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class PrototypeCompanionPresenter : MonoBehaviour
    {
        private Vector3 _origin;

        public void Build(Material bodyMaterial, Material glowMaterial)
        {
            _origin = transform.position;

            var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "CompanionBody";
            body.transform.SetParent(transform, false);
            body.transform.localScale = new Vector3(0.85f, 0.62f, 1.05f);
            body.GetComponent<Renderer>().sharedMaterial = bodyMaterial;

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "CompanionHead";
            head.transform.SetParent(transform, false);
            head.transform.localPosition = new Vector3(0f, 0.30f, 0.58f);
            head.transform.localScale = Vector3.one * 0.58f;
            head.GetComponent<Renderer>().sharedMaterial = bodyMaterial;

            CreateEar("LeftEar", new Vector3(-0.23f, 0.65f, 0.60f), -18f, glowMaterial);
            CreateEar("RightEar", new Vector3(0.23f, 0.65f, 0.60f), 18f, glowMaterial);

            var charm = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            charm.name = "BloomCharm";
            charm.transform.SetParent(transform, false);
            charm.transform.localPosition = new Vector3(0f, 0.04f, 0.78f);
            charm.transform.localScale = Vector3.one * 0.18f;
            charm.GetComponent<Renderer>().sharedMaterial = glowMaterial;
        }

        private void Update()
        {
            var bob = Mathf.Sin(Time.time * 2.1f) * 0.12f;
            transform.position = _origin + Vector3.up * bob;
            transform.Rotate(Vector3.up, 8f * Time.deltaTime, Space.World);
        }

        private void CreateEar(
            string name,
            Vector3 localPosition,
            float zRotation,
            Material material)
        {
            var ear = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ear.name = name;
            ear.transform.SetParent(transform, false);
            ear.transform.localPosition = localPosition;
            ear.transform.localScale = new Vector3(0.12f, 0.28f, 0.12f);
            ear.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
            ear.GetComponent<Renderer>().sharedMaterial = material;
        }
    }
}
