using LifePlayed.Client.Application;
using NUnit.Framework;

namespace LifePlayed.Client.Tests.EditMode
{
    public sealed class AppCoordinatorTests
    {
        [Test]
        public void OpenMovesThroughNavigatorWithoutOwningPresentation()
        {
            var navigator = new FakeNavigator();
            var lifecycle = new FakeLifecycle();
            var coordinator = new AppCoordinator(navigator, lifecycle);

            coordinator.Open(AppRoute.World);

            Assert.That(coordinator.CurrentRoute, Is.EqualTo(AppRoute.World));
            Assert.That(lifecycle.IsPaused, Is.False);
        }

        private sealed class FakeNavigator : IAppNavigator
        {
            public AppRoute CurrentRoute { get; private set; } = AppRoute.Home;

            public event System.Action<AppRoute> RouteChanged = delegate { };

            public void Navigate(AppRoute route)
            {
                CurrentRoute = route;
                RouteChanged(route);
            }
        }

        private sealed class FakeLifecycle : IPlatformLifecycle
        {
            public bool IsPaused { get; private set; }

            public event System.Action<bool> PauseChanged = delegate { };
        }
    }
}
