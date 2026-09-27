using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LifePlayed.Client.DomainBridge;
using LifePlayed.Client.Infrastructure;
using NUnit.Framework;

namespace LifePlayed.Client.Tests.EditMode
{
    public sealed class OfflineStoreTests
    {
        [Test]
        public async Task MutationAndCursorSurviveStoreRecreation()
        {
            var directory = Path.Combine(
                Path.GetTempPath(),
                "lifeplayed-tests",
                Guid.NewGuid().ToString("N"));

            var path = Path.Combine(
                directory,
                "offline-store.json");

            try
            {
                var first = new JsonFileOfflineStore(path);
                var mutation = new ClientSyncMutation
                {
                    mutationId = Guid.NewGuid().ToString("D"),
                    entityId = Guid.NewGuid().ToString("D"),
                    type = "action.create",
                    baseVersion = 0,
                    clientTimestamp =
                        DateTimeOffset.UtcNow.ToString("O"),
                    payloadJson = "{\"title\":\"Prototype\"}",
                };

                await first.EnqueueMutationAsync(
                    mutation,
                    CancellationToken.None);

                await first.WriteSyncCursorAsync(
                    42,
                    CancellationToken.None);

                var second = new JsonFileOfflineStore(path);
                var pending =
                    await second.ReadPendingMutationsAsync(
                        10,
                        CancellationToken.None);

                Assert.That(pending.Count, Is.EqualTo(1));
                Assert.That(
                    pending[0].mutationId,
                    Is.EqualTo(mutation.mutationId));
                Assert.That(
                    second.ReadSyncCursor(),
                    Is.EqualTo(42));

                await second.EnqueueMutationAsync(
                    mutation,
                    CancellationToken.None);

                var deduplicated =
                    await second.ReadPendingMutationsAsync(
                        10,
                        CancellationToken.None);

                Assert.That(
                    deduplicated.Count,
                    Is.EqualTo(1));

                await second.AcknowledgeMutationAsync(
                    mutation.mutationId,
                    CancellationToken.None);

                var third = new JsonFileOfflineStore(path);
                var empty =
                    await third.ReadPendingMutationsAsync(
                        10,
                        CancellationToken.None);

                Assert.That(empty, Is.Empty);
                Assert.That(
                    third.ReadSyncCursor(),
                    Is.EqualTo(42));
            }
            finally
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }
    }
}
