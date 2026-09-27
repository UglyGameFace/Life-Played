using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using LifePlayed.Client.Application;
using LifePlayed.Client.DomainBridge;
using UnityEngine;
using UnityEngine.Networking;

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

    public static class ContentIntegrityVerifier
    {
        public static ClientContentManifest Validate(
            string manifestJson,
            string verificationJson,
            byte[] contentBytes)
        {
            if (string.IsNullOrWhiteSpace(manifestJson))
            {
                throw new InvalidOperationException(
                    "Content manifest is empty.");
            }

            if (string.IsNullOrWhiteSpace(verificationJson))
            {
                throw new InvalidOperationException(
                    "Content verification record is empty.");
            }

            if (contentBytes == null ||
                contentBytes.Length == 0)
            {
                throw new InvalidOperationException(
                    "Content payload is empty.");
            }

            var manifest =
                JsonUtility.FromJson<ClientContentManifest>(
                    manifestJson);

            var verification =
                JsonUtility.FromJson<ClientContentVerification>(
                    verificationJson);

            if (manifest == null ||
                manifest.schemaVersion <= 0 ||
                string.IsNullOrWhiteSpace(manifest.releaseId) ||
                string.IsNullOrWhiteSpace(manifest.releaseVersion))
            {
                throw new InvalidOperationException(
                    "Content manifest is missing required fields.");
            }

            if (verification == null ||
                verification.schemaVersion != manifest.schemaVersion ||
                !string.Equals(
                    verification.releaseId,
                    manifest.releaseId,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    verification.releaseVersion,
                    manifest.releaseVersion,
                    StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(
                    verification.contentSha256))
            {
                throw new InvalidOperationException(
                    "Content verification metadata does not match the manifest.");
            }

            var actualHash =
                ComputeSha256Hex(
                    contentBytes);

            if (!string.Equals(
                actualHash,
                verification.contentSha256,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Content payload failed SHA-256 verification.");
            }

            return manifest;
        }

        public static void ValidateMinimumClientVersion(
            ClientContentManifest manifest,
            string currentClientVersion)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException(
                    nameof(manifest));
            }

            Version current;
            Version minimum;

            if (!Version.TryParse(
                    currentClientVersion,
                    out current) ||
                !Version.TryParse(
                    manifest.minimumClientVersion,
                    out minimum))
            {
                throw new InvalidOperationException(
                    "Client/content version metadata is invalid.");
            }

            if (current.CompareTo(minimum) < 0)
            {
                throw new InvalidOperationException(
                    "Content release requires client " +
                    minimum +
                    " or newer; current client is " +
                    current +
                    ".");
            }
        }

        public static string ComputeSha256Hex(
            byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(bytes);
                return BitConverter
                    .ToString(hash)
                    .Replace("-", string.Empty);
            }
        }
    }

    public sealed class StreamingAssetsContentGateway : IClientContentGateway
    {
        private const string ReleaseRelativeDirectory =
            "LifePlayed/Content/wild-renewal-v1";

        private const string ManifestFileName =
            "manifest.json";

        private const string ContentFileName =
            "content.json";

        private const string VerificationFileName =
            "verification.json";

        private readonly string _cacheDirectory;

        public StreamingAssetsContentGateway()
            : this(Path.Combine(
                UnityEngine.Application.persistentDataPath,
                "lifeplayed",
                "content",
                "wild-renewal-v1"))
        {
        }

        public StreamingAssetsContentGateway(
            string cacheDirectory)
        {
            if (string.IsNullOrWhiteSpace(cacheDirectory))
            {
                throw new ArgumentException(
                    "Content cache directory is required.",
                    nameof(cacheDirectory));
            }

            _cacheDirectory = cacheDirectory;
        }

        public string LastLoadSource { get; private set; } =
            "none";

        public async Task<ClientContentManifest> GetActiveManifestAsync(
            CancellationToken cancellationToken)
        {
            Exception bundledFailure = null;

            try
            {
                var bundled =
                    await LoadBundledReleaseAsync(
                        cancellationToken);

                var manifest =
                    ContentIntegrityVerifier.Validate(
                        bundled.manifestJson,
                        bundled.verificationJson,
                        bundled.contentBytes);

                ContentIntegrityVerifier.ValidateMinimumClientVersion(
                    manifest,
                    UnityEngine.Application.version);

                SaveLastKnownGood(
                    bundled.manifestJson,
                    bundled.verificationJson,
                    bundled.contentBytes);

                LastLoadSource = "bundle";
                return manifest;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                bundledFailure = exception;
            }

            try
            {
                var cached =
                    LoadLastKnownGood();

                var manifest =
                    ContentIntegrityVerifier.Validate(
                        cached.manifestJson,
                        cached.verificationJson,
                        cached.contentBytes);

                ContentIntegrityVerifier.ValidateMinimumClientVersion(
                    manifest,
                    UnityEngine.Application.version);

                LastLoadSource = "cache";
                return manifest;
            }
            catch (Exception cacheFailure)
            {
                throw new InvalidOperationException(
                    "Bundled and cached content releases both failed validation.",
                    new AggregateException(
                        bundledFailure,
                        cacheFailure));
            }
        }

        private async Task<ContentReleaseBytes> LoadBundledReleaseAsync(
            CancellationToken cancellationToken)
        {
            var basePath =
                UnityEngine.Application.streamingAssetsPath.TrimEnd('/') +
                "/" +
                ReleaseRelativeDirectory +
                "/";

            var manifestBytes =
                await ReadStreamingAssetAsync(
                    basePath + ManifestFileName,
                    cancellationToken);

            var verificationBytes =
                await ReadStreamingAssetAsync(
                    basePath + VerificationFileName,
                    cancellationToken);

            var contentBytes =
                await ReadStreamingAssetAsync(
                    basePath + ContentFileName,
                    cancellationToken);

            return new ContentReleaseBytes(
                System.Text.Encoding.UTF8.GetString(
                    manifestBytes),
                System.Text.Encoding.UTF8.GetString(
                    verificationBytes),
                contentBytes);
        }

        private static async Task<byte[]> ReadStreamingAssetAsync(
            string uri,
            CancellationToken cancellationToken)
        {
            using (var request = UnityWebRequest.Get(uri))
            {
                var completion =
                    new TaskCompletionSource<bool>(
                        TaskCreationOptions.RunContinuationsAsynchronously);

                var registration =
                    cancellationToken.Register(
                        request.Abort);

                try
                {
                    var operation =
                        request.SendWebRequest();

                    operation.completed += _ =>
                        completion.TrySetResult(true);

                    await completion.Task;
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.result !=
                        UnityWebRequest.Result.Success)
                    {
                        throw new InvalidOperationException(
                            "Streaming content load failed: " +
                            request.error +
                            " | " +
                            uri);
                    }

                    return request.downloadHandler.data;
                }
                finally
                {
                    registration.Dispose();
                }
            }
        }

        private void SaveLastKnownGood(
            string manifestJson,
            string verificationJson,
            byte[] contentBytes)
        {
            Directory.CreateDirectory(
                _cacheDirectory);

            WriteAtomic(
                Path.Combine(
                    _cacheDirectory,
                    ManifestFileName),
                System.Text.Encoding.UTF8.GetBytes(
                    manifestJson));

            WriteAtomic(
                Path.Combine(
                    _cacheDirectory,
                    VerificationFileName),
                System.Text.Encoding.UTF8.GetBytes(
                    verificationJson));

            WriteAtomic(
                Path.Combine(
                    _cacheDirectory,
                    ContentFileName),
                contentBytes);
        }

        private ContentReleaseBytes LoadLastKnownGood()
        {
            return new ContentReleaseBytes(
                File.ReadAllText(
                    Path.Combine(
                        _cacheDirectory,
                        ManifestFileName)),
                File.ReadAllText(
                    Path.Combine(
                        _cacheDirectory,
                        VerificationFileName)),
                File.ReadAllBytes(
                    Path.Combine(
                        _cacheDirectory,
                        ContentFileName)));
        }

        private static void WriteAtomic(
            string path,
            byte[] bytes)
        {
            var temporaryPath = path + ".tmp";
            File.WriteAllBytes(
                temporaryPath,
                bytes);

            File.Copy(
                temporaryPath,
                path,
                true);

            File.Delete(
                temporaryPath);
        }

        private readonly struct ContentReleaseBytes
        {
            public ContentReleaseBytes(
                string manifestJson,
                string verificationJson,
                byte[] contentBytes)
            {
                this.manifestJson = manifestJson;
                this.verificationJson = verificationJson;
                this.contentBytes = contentBytes;
            }

            public readonly string manifestJson;
            public readonly string verificationJson;
            public readonly byte[] contentBytes;
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
