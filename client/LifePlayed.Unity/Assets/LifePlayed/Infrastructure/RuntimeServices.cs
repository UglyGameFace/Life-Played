using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LifePlayed.Client.Application;
using LifePlayed.Client.DomainBridge;
using UnityEngine;

namespace LifePlayed.Client.Infrastructure
{
    public sealed class RuntimeNavigator : IAppNavigator
    {
        private AppRoute _currentRoute = AppRoute.Home;

        public AppRoute CurrentRoute => _currentRoute;

        public event Action<AppRoute> RouteChanged = delegate { };

        public void Navigate(AppRoute route)
        {
            if (_currentRoute == route)
            {
                return;
            }

            _currentRoute = route;
            RouteChanged(route);
        }
    }

    public sealed class PlayerPrefsClientSettingsStore : IClientSettingsStore
    {
        public string Read(string key)
        {
            return PlayerPrefs.HasKey(key)
                ? PlayerPrefs.GetString(key)
                : null;
        }

        public void Write(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        public void Remove(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    public sealed class MemoryClientSettingsStore : IClientSettingsStore
    {
        private readonly Dictionary<string, string> _values =
            new Dictionary<string, string>(StringComparer.Ordinal);

        public string Read(string key)
        {
            string value;
            return _values.TryGetValue(key, out value)
                ? value
                : null;
        }

        public void Write(string key, string value)
        {
            _values[key] = value;
        }

        public void Remove(string key)
        {
            _values.Remove(key);
        }
    }

    [Serializable]
    public sealed class OfflineStoreDocument
    {
        public long syncCursor;
        public List<ClientSyncMutation> pendingMutations =
            new List<ClientSyncMutation>();
    }

    public sealed class JsonFileOfflineStore : IClientOfflineStore
    {
        private readonly object _gate = new object();
        private readonly string _path;
        private OfflineStoreDocument _document;
        private bool _loaded;

        public JsonFileOfflineStore()
            : this(Path.Combine(
                UnityEngine.Application.persistentDataPath,
                "lifeplayed",
                "offline-store.json"))
        {
        }

        public JsonFileOfflineStore(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(
                    "Offline store path is required.",
                    nameof(path));
            }

            _path = path;
        }

        public Task EnqueueMutationAsync(
            ClientSyncMutation mutation,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (mutation == null ||
                string.IsNullOrWhiteSpace(mutation.mutationId))
            {
                throw new ArgumentException(
                    "Mutation and mutationId are required.",
                    nameof(mutation));
            }

            lock (_gate)
            {
                EnsureLoaded();

                for (var index = 0;
                    index < _document.pendingMutations.Count;
                    index++)
                {
                    if (string.Equals(
                        _document.pendingMutations[index].mutationId,
                        mutation.mutationId,
                        StringComparison.Ordinal))
                    {
                        return Task.CompletedTask;
                    }
                }

                _document.pendingMutations.Add(mutation);
                Save();
            }

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<ClientSyncMutation>> ReadPendingMutationsAsync(
            int limit,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (limit <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(limit),
                    limit,
                    "Limit must be greater than zero.");
            }

            lock (_gate)
            {
                EnsureLoaded();

                var count = Math.Min(
                    limit,
                    _document.pendingMutations.Count);

                var result = new List<ClientSyncMutation>(count);
                for (var index = 0; index < count; index++)
                {
                    result.Add(_document.pendingMutations[index]);
                }

                return Task.FromResult<IReadOnlyList<ClientSyncMutation>>(
                    result);
            }
        }

        public Task AcknowledgeMutationAsync(
            string mutationId,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(mutationId))
            {
                throw new ArgumentException(
                    "MutationId is required.",
                    nameof(mutationId));
            }

            lock (_gate)
            {
                EnsureLoaded();

                var removed = _document.pendingMutations.RemoveAll(
                    mutation => string.Equals(
                        mutation.mutationId,
                        mutationId,
                        StringComparison.Ordinal));

                if (removed > 0)
                {
                    Save();
                }
            }

            return Task.CompletedTask;
        }

        public long ReadSyncCursor()
        {
            lock (_gate)
            {
                EnsureLoaded();
                return _document.syncCursor;
            }
        }

        public Task WriteSyncCursorAsync(
            long cursor,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (cursor < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cursor),
                    cursor,
                    "Sync cursor cannot be negative.");
            }

            lock (_gate)
            {
                EnsureLoaded();
                _document.syncCursor = cursor;
                Save();
            }

            return Task.CompletedTask;
        }

        private void EnsureLoaded()
        {
            if (_loaded)
            {
                return;
            }

            _loaded = true;

            if (!File.Exists(_path))
            {
                _document = new OfflineStoreDocument();
                return;
            }

            var json = File.ReadAllText(_path);
            _document = JsonUtility.FromJson<OfflineStoreDocument>(json);

            if (_document == null)
            {
                _document = new OfflineStoreDocument();
            }

            if (_document.pendingMutations == null)
            {
                _document.pendingMutations =
                    new List<ClientSyncMutation>();
            }
        }

        private void Save()
        {
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tempPath = _path + ".tmp";
            var json = JsonUtility.ToJson(_document, false);

            File.WriteAllText(tempPath, json);
            File.Copy(tempPath, _path, true);
            File.Delete(tempPath);
        }
    }

    public sealed class StaticFeatureFlagProvider : IFeatureFlagProvider
    {
        private readonly HashSet<string> _enabled;

        public StaticFeatureFlagProvider(IEnumerable<string> enabled)
        {
            _enabled = new HashSet<string>(
                enabled,
                StringComparer.Ordinal);
        }

        public bool IsEnabled(string featureKey) =>
            _enabled.Contains(featureKey);
    }

    public sealed class DeferredSyncGateway : IClientSyncGateway
    {
        public Task<IReadOnlyList<ClientSyncResult>> SyncAsync(
            ClientSyncBatch batch,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<ClientSyncResult> empty =
                Array.Empty<ClientSyncResult>();

            return Task.FromResult(empty);
        }
    }

    public sealed class EmbeddedContentGateway : IClientContentGateway
    {
        private readonly ClientContentManifest _manifest;

        public EmbeddedContentGateway(
            ClientContentManifest manifest)
        {
            _manifest = manifest;
        }

        public Task<ClientContentManifest> GetActiveManifestAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(_manifest);
        }
    }
}
