using System;
using System.Collections.Generic;
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

    public sealed class PlayerPrefsLocalStateStore : ILocalStateStore
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

    public sealed class MemoryLocalStateStore : ILocalStateStore
    {
        private readonly Dictionary<string, string> _values =
            new Dictionary<string, string>(StringComparer.Ordinal);

        public string Read(string key)
        {
            string value;
            return _values.TryGetValue(key, out value) ? value : null;
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

    public sealed class StaticFeatureFlagProvider : IFeatureFlagProvider
    {
        private readonly HashSet<string> _enabled;

        public StaticFeatureFlagProvider(IEnumerable<string> enabled)
        {
            _enabled = new HashSet<string>(enabled, StringComparer.Ordinal);
        }

        public bool IsEnabled(string featureKey) => _enabled.Contains(featureKey);
    }

    public sealed class DeferredSyncGateway : IClientSyncGateway
    {
        public Task<IReadOnlyList<ClientSyncResult>> SyncAsync(
            ClientSyncBatch batch,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<ClientSyncResult> empty = Array.Empty<ClientSyncResult>();
            return Task.FromResult(empty);
        }
    }

    public sealed class EmbeddedContentGateway : IClientContentGateway
    {
        private readonly ClientContentManifest _manifest;

        public EmbeddedContentGateway(ClientContentManifest manifest)
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
