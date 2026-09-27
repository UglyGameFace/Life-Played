using System;
using System.IO;
using UnityEngine;

namespace LifePlayed.Client.Infrastructure
{
    public sealed class DevelopmentLogRecorder : IDisposable
    {
        private const long MaximumPreviousLogBytes =
            2L * 1024L * 1024L;

        private readonly object _gate =
            new object();

        private readonly string _path;
        private bool _started;

        public DevelopmentLogRecorder(
            string persistentDataPath)
        {
            if (string.IsNullOrWhiteSpace(
                persistentDataPath))
            {
                throw new ArgumentException(
                    "Persistent data path is required.",
                    nameof(persistentDataPath));
            }

            var directory = Path.Combine(
                persistentDataPath,
                "lifeplayed",
                "diagnostics");

            Directory.CreateDirectory(
                directory);

            _path = Path.Combine(
                directory,
                "development.log");

            RotateOversizedPreviousLog();
        }

        public string Path => _path;

        public void Start(string buildHeader)
        {
            if (_started)
            {
                return;
            }

            lock (_gate)
            {
                File.AppendAllText(
                    _path,
                    Environment.NewLine +
                    "=== Life Played session ===" +
                    Environment.NewLine +
                    buildHeader +
                    Environment.NewLine);
            }

            Application.logMessageReceivedThreaded +=
                OnLogMessage;

            _started = true;
        }

        public void Dispose()
        {
            if (!_started)
            {
                return;
            }

            Application.logMessageReceivedThreaded -=
                OnLogMessage;

            _started = false;
        }

        private void OnLogMessage(
            string condition,
            string stackTrace,
            LogType type)
        {
            try
            {
                var line =
                    DateTimeOffset.UtcNow.ToString("O") +
                    " [" +
                    type +
                    "] " +
                    condition;

                if (!string.IsNullOrWhiteSpace(
                    stackTrace))
                {
                    line +=
                        Environment.NewLine +
                        stackTrace;
                }

                lock (_gate)
                {
                    File.AppendAllText(
                        _path,
                        line +
                        Environment.NewLine);
                }
            }
            catch
            {
                // Never recurse into Unity logging from the log callback.
            }
        }

        private void RotateOversizedPreviousLog()
        {
            if (!File.Exists(_path))
            {
                return;
            }

            var info =
                new FileInfo(_path);

            if (info.Length <=
                MaximumPreviousLogBytes)
            {
                return;
            }

            var previousPath =
                _path + ".previous";

            File.Copy(
                _path,
                previousPath,
                true);

            File.WriteAllText(
                _path,
                string.Empty);
        }
    }
}
