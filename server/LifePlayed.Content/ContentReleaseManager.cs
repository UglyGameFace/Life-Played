using LifePlayed.Contracts.Content;

namespace LifePlayed.Content;

public sealed record ContentActivationResult(
    bool Activated,
    ContentReleaseDefinition? ActiveRelease,
    ContentValidationResult Validation);

public sealed class ContentReleaseManager
{
    private readonly ContentReleaseLoader _loader;
    private readonly ContentValidator _validator;

    public ContentReleaseManager(
        ContentReleaseLoader loader,
        ContentValidator validator)
    {
        _loader = loader;
        _validator = validator;
    }

    public ContentReleaseDefinition? ActiveRelease { get; private set; }

    public async Task<ContentActivationResult> TryActivateAsync(
        string directory,
        ContentValidationProfile profile,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var loaded = await _loader.LoadAsync(directory, cancellationToken);
            var validation = _validator.Validate(loaded.Content, profile);

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
