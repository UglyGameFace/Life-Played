using System.Text.Json;
using LifePlayed.Contracts.Content;

namespace LifePlayed.Content;

public sealed record ContentActivationResult(
    bool Activated,
    ContentReleaseDefinition? ActiveRelease,
    ContentValidationResult Validation);

public sealed class ContentReleaseManager
{
    public ContentReleaseDefinition? ActiveRelease { get; private set; }

    public async Task<ContentActivationResult> TryActivateAsync(
        string directory,
        ContentValidationProfile profile,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var loaded = await ContentReleaseLoader.LoadAsync(directory, cancellationToken);
            var validation = ContentValidator.Validate(loaded.Content, profile);

            if (!validation.IsValid)
            {
                return new ContentActivationResult(
                    false,
                    ActiveRelease,
                    validation);
            }

            ActiveRelease = loaded.Content;
            return new ContentActivationResult(
                true,
                ActiveRelease,
                validation);
        }
        catch (InvalidDataException exception)
        {
            var validation = new ContentValidationResult(
                new[]
                {
                    new ContentValidationIssue(
                        ContentValidationCodes.LoadFailed,
                        Path.GetFileName(directory),
                        null,
                        exception.Message),
                });

            return new ContentActivationResult(
                false,
                ActiveRelease,
                validation);
        }
        catch (JsonException exception)
        {
            var validation = new ContentValidationResult(
                new[]
                {
                    new ContentValidationIssue(
                        ContentValidationCodes.LoadFailed,
                        Path.GetFileName(directory),
                        null,
                        exception.Message),
                });

            return new ContentActivationResult(
                false,
                ActiveRelease,
                validation);
        }
    }
}
