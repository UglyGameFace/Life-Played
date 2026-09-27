namespace LifePlayed.Contracts.Content;

public sealed record ContentReleaseManifest(
    int SchemaVersion,
    string ReleaseId,
    string ReleaseVersion,
    string MinimumClientVersion,
    DateTimeOffset PublishedAt,
    IReadOnlyDictionary<string, string> FileHashes);
