using System.Collections;
using LifePlayed.Client.Presentation.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LifePlayed.Client.Tests.PlayMode
{
    public sealed class PresentationSmokeTests
    {
        [UnityTest]
        public IEnumerator WildRenewalPrototypeBuildsCharacterAndCompanion()
        {
            var root = new GameObject("WildRenewalTest");
            root.AddComponent<PrototypeWildRenewalHub>();

            yield return null;

            Assert.That(GameObject.Find("Waykeeper"), Is.Not.Null);
            Assert.That(GameObject.Find("StarterCompanion"), Is.Not.Null);

            Object.Destroy(root);
        }
    }
}
