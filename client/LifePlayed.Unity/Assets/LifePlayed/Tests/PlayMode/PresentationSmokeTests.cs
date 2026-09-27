using System.Collections;
using LifePlayed.Client.Application;
using LifePlayed.Client.Presentation.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LifePlayed.Client.Tests.PlayMode
{
    public sealed class PresentationSmokeTests
    {
        [UnityTest]
        public IEnumerator WildRenewalPrototypeBuildsMeaningfulFirstWorld()
        {
            var root = new GameObject("WildRenewalTest");
            root.AddComponent<PrototypeWildRenewalHub>();

            yield return null;

            Assert.That(GameObject.Find("Waykeeper"), Is.Not.Null);
            Assert.That(GameObject.Find("LeafglowFox"), Is.Not.Null);
            Assert.That(GameObject.Find("CentralHearth"), Is.Not.Null);
            Assert.That(GameObject.Find("QuietBloom"), Is.Not.Null);
            Assert.That(GameObject.Find("MossPath"), Is.Not.Null);
            Assert.That(GameObject.Find("DormantWorkshop"), Is.Not.Null);
            Assert.That(GameObject.Find("GatheringPlace"), Is.Not.Null);
            Assert.That(GameObject.Find("GroveArch"), Is.Not.Null);
            Assert.That(GameObject.Find("BloomWisps"), Is.Not.Null);

            Object.Destroy(root);
        }

        [UnityTest]
        public IEnumerator GraphicsTiersCanSwitchAtRuntime()
        {
            var root = new GameObject("QualityTest");
            var quality = root.AddComponent<GraphicsQualityController>();

            yield return null;

            quality.Apply(GraphicsTier.Reduced);
            Assert.That(
                quality.CurrentTier,
                Is.EqualTo(GraphicsTier.Reduced));

            quality.Apply(GraphicsTier.Standard);
            Assert.That(
                quality.CurrentTier,
                Is.EqualTo(GraphicsTier.Standard));

            quality.Apply(GraphicsTier.High);
            Assert.That(
                quality.CurrentTier,
                Is.EqualTo(GraphicsTier.High));

            Object.Destroy(root);
        }
    }
}
