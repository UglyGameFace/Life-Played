using System;
using System.Text;
using LifePlayed.Client.Infrastructure;
using NUnit.Framework;

namespace LifePlayed.Client.Tests.EditMode
{
    public sealed class ContentIntegrityTests
    {
        [Test]
        public void ValidContentMatchesVerificationRecord()
        {
            var content =
                Encoding.UTF8.GetBytes(
                    "{\"release\":\"wild-renewal\"}");

            var hash =
                ContentIntegrityVerifier.ComputeSha256Hex(
                    content);

            var manifest =
                "{\"schemaVersion\":1," +
                "\"releaseId\":\"release.wild_renewal.v1.skeleton\"," +
                "\"releaseVersion\":\"1.0.0\"," +
                "\"minimumClientVersion\":\"0.1.0\"," +
                "\"publishedAt\":\"2026-09-27T15:00:00Z\"}";

            var verification =
                "{\"schemaVersion\":1," +
                "\"releaseId\":\"release.wild_renewal.v1.skeleton\"," +
                "\"releaseVersion\":\"1.0.0\"," +
                "\"contentSha256\":\"" +
                hash +
                "\"}";

            var parsed =
                ContentIntegrityVerifier.Validate(
                    manifest,
                    verification,
                    content);

            Assert.That(
                parsed.releaseVersion,
                Is.EqualTo("1.0.0"));
        }

        [Test]
        public void ContentRejectsIncompatibleClientVersion()
        {
            var manifest =
                new LifePlayed.Client.DomainBridge.ClientContentManifest
                {
                    schemaVersion = 1,
                    releaseId = "release.test",
                    releaseVersion = "1.0.0",
                    minimumClientVersion = "0.2.0",
                };

            Assert.Throws<InvalidOperationException>(
                () =>
                    ContentIntegrityVerifier.ValidateMinimumClientVersion(
                        manifest,
                        "0.1.0"));
        }

        [Test]
        public void CompatibleClientVersionIsAccepted()
        {
            var manifest =
                new LifePlayed.Client.DomainBridge.ClientContentManifest
                {
                    schemaVersion = 1,
                    releaseId = "release.test",
                    releaseVersion = "1.0.0",
                    minimumClientVersion = "0.1.0",
                };

            Assert.DoesNotThrow(
                () =>
                    ContentIntegrityVerifier.ValidateMinimumClientVersion(
                        manifest,
                        "0.1.0"));
        }

        [Test]
        public void TamperedContentIsRejected()
        {
            var original =
                Encoding.UTF8.GetBytes(
                    "{\"release\":\"wild-renewal\"}");

            var tampered =
                Encoding.UTF8.GetBytes(
                    "{\"release\":\"tampered\"}");

            var hash =
                ContentIntegrityVerifier.ComputeSha256Hex(
                    original);

            var manifest =
                "{\"schemaVersion\":1," +
                "\"releaseId\":\"release.wild_renewal.v1.skeleton\"," +
                "\"releaseVersion\":\"1.0.0\"}";

            var verification =
                "{\"schemaVersion\":1," +
                "\"releaseId\":\"release.wild_renewal.v1.skeleton\"," +
                "\"releaseVersion\":\"1.0.0\"," +
                "\"contentSha256\":\"" +
                hash +
                "\"}";

            Assert.Throws<InvalidOperationException>(
                () =>
                    ContentIntegrityVerifier.Validate(
                        manifest,
                        verification,
                        tampered));
        }
    }
}
