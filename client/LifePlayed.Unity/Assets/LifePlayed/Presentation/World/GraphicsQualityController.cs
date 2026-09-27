using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LifePlayed.Client.Presentation.World
{
    public enum LifePlayedGraphicsTier
    {
        Reduced = 0,
        Standard = 1,
        High = 2,
    }

    [DisallowMultipleComponent]
    public sealed class GraphicsQualityController : MonoBehaviour
    {
        [SerializeField]
        private LifePlayedGraphicsTier initialTier = LifePlayedGraphicsTier.Standard;

        public LifePlayedGraphicsTier CurrentTier { get; private set; }

        private void Awake()
        {
            Apply(initialTier);
        }

        public void Apply(LifePlayedGraphicsTier tier)
        {
            CurrentTier = tier;

            switch (tier)
            {
                case LifePlayedGraphicsTier.Reduced:
                    Application.targetFrameRate = 30;
                    QualitySettings.shadows = ShadowQuality.Disable;
                    SetRenderScale(0.72f);
                    break;

                case LifePlayedGraphicsTier.Standard:
                    Application.targetFrameRate = 30;
                    QualitySettings.shadows = ShadowQuality.HardOnly;
                    SetRenderScale(0.90f);
                    break;

                case LifePlayedGraphicsTier.High:
                    Application.targetFrameRate = 60;
                    QualitySettings.shadows = ShadowQuality.All;
                    SetRenderScale(1.0f);
                    break;
            }
        }

        private static void SetRenderScale(float value)
        {
            var pipeline = UniversalRenderPipeline.asset;
            if (pipeline != null)
            {
                pipeline.renderScale = value;
            }
        }
    }
}
