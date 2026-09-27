using System;
using LifePlayed.Client.Application;
using UnityEngine;

namespace LifePlayed.Client.Platform
{
    public sealed class PlatformLifecycleBridge :
        MonoBehaviour,
        IPlatformLifecycle
    {
        public bool IsPaused { get; private set; }

        public event Action<bool> PauseChanged =
            delegate { };

        public event Action LowMemory =
            delegate { };

        private void OnEnable()
        {
            Application.lowMemory +=
                OnLowMemory;
        }

        private void OnDisable()
        {
            Application.lowMemory -=
                OnLowMemory;
        }

        private void OnApplicationPause(
            bool paused)
        {
            SetPaused(paused);
        }

        private void OnApplicationFocus(
            bool focused)
        {
            SetPaused(!focused);
        }

        private void SetPaused(
            bool paused)
        {
            if (IsPaused == paused)
            {
                return;
            }

            IsPaused = paused;
            PauseChanged(paused);
        }

        private void OnLowMemory()
        {
            LowMemory();
        }
    }
}
