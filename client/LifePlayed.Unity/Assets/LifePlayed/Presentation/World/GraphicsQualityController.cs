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
                    ConfigureUrp(
                        0.72f,
                        0f,
                        1,
                        1);
                    break;

                case GraphicsTier.Standard:
                    Application.targetFrameRate = 30;
                    QualitySettings.shadows = ShadowQuality.HardOnly;
                    ConfigureUrp(
                        0.90f,
                        28f,
                        2,
                        2);
                    break;

                case GraphicsTier.High:
                    Application.targetFrameRate = 60;
                    QualitySettings.shadows = ShadowQuality.All;
                    ConfigureUrp(
                        1.0f,
                        45f,
                        2,
                        4);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(tier),
                        tier,
                        "Unknown graphics tier.");
            }

            TierChanged(tier);
        }

        private static void ConfigureUrp(
            float renderScale,
            float shadowDistance,
            int shadowCascadeCount,
            int msaaSampleCount)
        {
            var pipeline = UniversalRenderPipeline.asset;
            if (pipeline == null)
            {
                return;
            }

            pipeline.renderScale = renderScale;
            pipeline.shadowDistance = shadowDistance;
            pipeline.shadowCascadeCount = shadowCascadeCount;
            pipeline.msaaSampleCount = msaaSampleCount;
        }
    }
}
