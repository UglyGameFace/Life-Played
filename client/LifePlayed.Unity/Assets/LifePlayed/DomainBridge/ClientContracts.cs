using System;
using System.Collections.Generic;

namespace LifePlayed.Client.DomainBridge
{
    [Serializable]
    public sealed class ClientSyncMutation
    {
        public string mutationId = string.Empty;
        public string entityId = string.Empty;
        public string type = string.Empty;
        public long baseVersion;
        public string clientTimestamp = string.Empty;
        public string payloadJson = "{}";
    }

    [Serializable]
    public sealed class ClientSyncBatch
    {
        public string accountId = string.Empty;
        public string deviceId = string.Empty;
        public long sinceCursor;
        public List<ClientSyncMutation> mutations = new List<ClientSyncMutation>();
    }

    [Serializable]
    public sealed class ClientSyncResult
    {
        public string mutationId = string.Empty;
        public string entityId = string.Empty;
        public string status = string.Empty;
        public bool replayed;
        public long canonicalVersion;
        public string errorCode = string.Empty;
        public string errorMessage = string.Empty;
        public string canonicalDataJson = "{}";
    }

    [Serializable]
    public sealed class ClientContentManifest
    {
        public int schemaVersion;
        public string releaseId = string.Empty;
        public string releaseVersion = string.Empty;
        public string minimumClientVersion = string.Empty;
        public string publishedAt = string.Empty;
    }
}
