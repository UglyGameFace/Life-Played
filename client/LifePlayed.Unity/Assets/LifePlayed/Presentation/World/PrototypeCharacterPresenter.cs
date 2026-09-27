using UnityEngine;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class PrototypeCharacterPresenter : MonoBehaviour
    {
        public Transform Build(Material bodyMaterial, Material accentMaterial)
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "HeroBody";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            body.transform.localScale = new Vector3(0.72f, 0.92f, 0.72f);
            body.GetComponent<Renderer>().sharedMaterial = bodyMaterial;

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "HeroHead";
            head.transform.SetParent(transform, false);
            head.transform.localPosition = new Vector3(0f, 2.15f, 0f);
            head.transform.localScale = Vector3.one * 0.78f;
            head.GetComponent<Renderer>().sharedMaterial = accentMaterial;

            var pack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pack.name = "WaykeeperPack";
            pack.transform.SetParent(transform, false);
            pack.transform.localPosition = new Vector3(0f, 1.35f, -0.52f);
            pack.transform.localScale = new Vector3(0.56f, 0.72f, 0.28f);
            pack.GetComponent<Renderer>().sharedMaterial = accentMaterial;

            return transform;
        }
    }
}
