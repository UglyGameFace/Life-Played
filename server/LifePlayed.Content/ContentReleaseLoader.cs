using System.Security.Cryptography;
using System.Text.Json;
using LifePlayed.Contracts.Content;

namespace LifePlayed.Content;

public sealed record LoadedContentRelease(
    ContentReleaseManifest Manifest,
    ContentReleaseDefinition Content);

public sealed class ContentReleaseLoader
{
    private const string ManifestFileName = "manifest.json";
    private const string ContentFileName = "content.json";

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task<LoadedContentRelease> LoadAsync(
        string directory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);

        var root = Path.GetFullPath(directory);
        var manifestPath = Path.Combine(root, ManifestFileName);
        var contentPath = Path.Combine(root, ContentFileName);

        var manifest = await ReadJsonAsync<ContentReleaseManifest>(
            manifestPath,
            cancellationToken);

        if (manifest.SchemaVersion != 1)
        {
            throw new InvalidDataException(
                $"Unsupported content manifest schema version '{manifest.SchemaVersion}'.");
        }

        if (!manifest.FileHashes.ContainsKey(ContentFileName))
        {
            throw new InvalidDataException(
                $"The manifest must contain a SHA-256 hash for '{ContentFileName}'.");
        }

        foreach (var pair in manifest.FileHashes.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            var filePath = ResolveSafePath(root, pair.Key);
            var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var hash = Convert.ToHexString(SHA256.HashData(bytes));

            if (!hash.Equals(pair.Value, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    $"Content hash mismatch for '{pair.Key}'.");
            }
        }

        var content = await ReadJsonAsync<ContentReleaseDefinition>(
            contentPath,
            cancellationToken);

        if (content.SchemaVersion != manifest.SchemaVersion ||
            !content.ReleaseId.Equals(manifest.ReleaseId, StringComparison.Ordinal) ||
            !content.ReleaseVersion.Equals(manifest.ReleaseVersion, StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                "Content release metadata does not match its manifest.");
        }

        return new LoadedContentRelease(manifest, content);
    }

    private static async Task<T> ReadJsonAsync<T>(
        string path,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(
                   stream,
                   JsonOptions,
                   cancellationToken)
               ?? throw new InvalidDataException(
                   $"Content file '{Path.GetFileName(path)}' is empty or invalid.");
    }

    private static string ResolveSafePath(
        string root,
        string relativePath)
    {
        if (Path.IsPathRooted(relativePath))
        {
            throw new InvalidDataException(
                "Content manifest paths must be relative.");
        }

        var candidate = Path.GetFullPath(Path.Combine(root, relativePath));
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        var rootWithSeparator = root.EndsWith(Path.DirectorySeparatorChar)
            ? root
            : root + Path.DirectorySeparatorChar;

        if (!candidate.StartsWith(rootWithSeparator, comparison))
        {
            throw new InvalidDataException(
                $"Content manifest path '{relativePath}' escapes the release directory.");
        }

        return candidate;
    }
}
