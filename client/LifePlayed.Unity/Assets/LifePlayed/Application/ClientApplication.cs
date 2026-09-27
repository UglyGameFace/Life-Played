using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LifePlayed.Client.DomainBridge;

namespace LifePlayed.Client.Application
{
    public enum AppRoute
    {
        Home = 0,
        Quests = 1,
        World = 2,
        Hero = 3,
        More = 4,
    }

    public enum GraphicsTier
    {
        Reduced = 0,
        Standard = 1,
        High = 2,
    }

    public interface IClientSyncGateway
    {
        Task<IReadOnlyList<ClientSyncResult>> SyncAsync(
            ClientSyncBatch batch,
            CancellationToken cancellationToken);
    }

    public interface IClientContentGateway
    {
        Task<ClientContentManifest> GetActiveManifestAsync(
            CancellationToken cancellationToken);
    }

    public interface ILocalStateStore
    {
        string Read(string key);

        void Write(string key, string value);

        void Remove(string key);
    }

    public interface IFeatureFlagProvider
    {
        bool IsEnabled(string featureKey);
    }

    public interface IPlatformLifecycle
    {
        bool IsPaused { get; }

        event Action<bool> PauseChanged;
    }

    public interface IGraphicsQualityController
    {
        GraphicsTier CurrentTier { get; }

        event Action<GraphicsTier> TierChanged;

        void Apply(GraphicsTier tier);
    }

    public interface IAppNavigator
    {
        AppRoute CurrentRoute { get; }

        event Action<AppRoute> RouteChanged;

        void Navigate(AppRoute route);
    }

    public sealed class AppCoordinator
    {
        private readonly IAppNavigator _navigator;
        private readonly IPlatformLifecycle _lifecycle;

        public AppCoordinator(
            IAppNavigator navigator,
            IPlatformLifecycle lifecycle)
        {
            _navigator = navigator;
            _lifecycle = lifecycle;
        }

        public AppRoute CurrentRoute => _navigator.CurrentRoute;

        public bool IsPaused => _lifecycle.IsPaused;

        public event Action<AppRoute> RouteChanged
        {
            add => _navigator.RouteChanged += value;
            remove => _navigator.RouteChanged -= value;
        }

        public void Open(AppRoute route) => _navigator.Navigate(route);
    }
}
