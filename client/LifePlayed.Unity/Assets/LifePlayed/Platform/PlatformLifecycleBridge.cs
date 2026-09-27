using System;
using LifePlayed.Client.Application;
using UnityEngine;

namespace LifePlayed.Client.Platform
{
    public sealed class PlatformLifecycleBridge : MonoBehaviour, IPlatformLifecycle
    {
        public bool IsPaused { get; private set; }

        public event Action<bool> PauseChanged = delegate { };

        private void OnApplicationPause(bool paused)
        {
            IsPaused = paused;
            PauseChanged(paused);
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused && !IsPaused)
            {
                IsPaused = true;
                PauseChanged(true);
            }
            else if (focused && IsPaused)
            {
                IsPaused = false;
                PauseChanged(false);
            }
        }
    }
}
