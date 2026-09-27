using System;
using LifePlayed.Client.Application;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LifePlayed.Client.Presentation.World
{
    [DisallowMultipleComponent]
    public sealed class GraphicsQualityController :
        MonoBehaviour,
        IGraphicsQualityController
    {
        [SerializeField]
        private GraphicsTier initialTier = GraphicsTier.Standard;

        public GraphicsTier CurrentTier { get; private set; }

        public event Action<GraphicsTier> TierChanged = delegate { };

        private void Awake()
        {
            Apply(initialTier);
        }

        public void Apply(GraphicsTier tier)
        {
            CurrentTier = tier;

            switch (tier)
            {
                case GraphicsTier.Reduced:
                    Application.targetFrameRate = 30;
                    QualitySettings.shadows = ShadowQuality.Disable;
                    QualitySettings.shadowDistance = 0f;
                    SetRenderScale(0.72f);
                    break;

                case GraphicsTier.Standard:
                    Application.targetFrameRate = 30;
                    QualitySettings.shadows = ShadowQuality.HardOnly;
                    QualitySettings.shadowDistance = 28f;
                    SetRenderScale(0.90f);
                    break;

                case GraphicsTier.High:
                    Application.targetFrameRate = 60;
                    QualitySettings.shadows = ShadowQuality.All;
                    QualitySettings.shadowDistance = 45f;
                    SetRenderScale(1.0f);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(tier),
                        tier,
                        "Unknown graphics tier.");
            }

            TierChanged(tier);
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
