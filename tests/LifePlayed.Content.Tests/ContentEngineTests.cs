using System.Text.Json;
using LifePlayed.Contracts.Content;

namespace LifePlayed.Content.Tests;

public sealed class ContentEngineTests
{
    private static readonly string ReleaseDirectory =
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "wild-renewal-v1");

    private static readonly string[] PostV1FeatureFixture =
        ["background_location_rewards"];

    [Fact]
    public async Task WildRenewalReleaseLoadsAndValidatesEndToEnd()
    {
        var loaded = await ContentReleaseLoader.LoadAsync(
            ReleaseDirectory,
            TestContext.Current.CancellationToken);

        var validation = ContentValidator.Validate(
            loaded.Content,
            ContentValidationProfiles.V1);

        Assert.True(validation.IsValid, Describe(validation));
        Assert.Equal("release.wild_renewal.v1.skeleton", loaded.Content.ReleaseId);
        Assert.Single(loaded.Content.Worlds);
        Assert.Single(loaded.Content.Sagas);
        Assert.Equal(8, loaded.Content.Chapters.Count);
        Assert.Equal(6, loaded.Content.Npcs.Count);
        Assert.Equal(4, loaded.Content.Companions.Count);
        Assert.Equal(8, loaded.Content.WorldStructures.Count);
        Assert.Equal(8, loaded.Content.ChronicleTriggers.Count);
        Assert.Equal(10, loaded.Content.CampaignArchetypes.Count);

        var expectedChapterIds = new[]
        {
            "chapter.wr.01.empty_hearth",
            "chapter.wr.02.footprints_moss",
            "chapter.wr.03.workshop_wakes",
            "chapter.wr.04.voices_return",
            "chapter.wr.05.faded_grove",
            "chapter.wr.06.long_rain",
            "chapter.wr.07.roots_remember",
            "chapter.wr.08.quiet_bloom",
        };

        Assert.Equal(expectedChapterIds, loaded.Content.Sagas[0].ChapterIds);
        Assert.Equal(
            "world.wild_renewal.state.quiet_bloom_complete",
            loaded.Content.Sagas[0].PostSagaStateKey);

        for (var index = 0; index < loaded.Content.Chapters.Count; index++)
        {
            var chapter = loaded.Content.Chapters[index];
            Assert.NotEmpty(chapter.ObjectiveHooks);
            Assert.NotEmpty(chapter.Nodes);
            Assert.False(string.IsNullOrWhiteSpace(chapter.TitleKey));
            Assert.False(string.IsNullOrWhiteSpace(chapter.SummaryKey));

            if (index == 0)
            {
                Assert.Empty(chapter.PrerequisiteChapterIds);
            }
            else
            {
                Assert.Equal(
                    loaded.Content.Chapters[index - 1].Id,
                    Assert.Single(chapter.PrerequisiteChapterIds));
            }
        }

        Assert.Contains(
            loaded.Content.Companions,
            static companion =>
                companion.Id == "companion.wr.grove_guardian" &&
                companion.Tier == "guardian");
    }

    [Fact]
    public async Task DuplicateIdsAreRejectedWithStableReasonCode()
    {
        var release = await LoadContentAsync();
        var duplicate = release.Npcs[0] with { RoleKey = "duplicate.role" };
        var broken = release with
        {
            Npcs = release.Npcs.Concat(new[] { duplicate }).ToArray(),
        };

        AssertIssue(broken, ContentValidationCodes.DuplicateId);
    }

    [Fact]
    public async Task MissingReferencesAreRejectedWithStableReasonCode()
    {
        var release = await LoadContentAsync();
        var chapter = release.Chapters[0];
        var node = chapter.Nodes[0] with
        {
            NpcIds = new[] { "npc.wr.does_not_exist" },
        };
        var broken = ReplaceChapter(
            release,
            chapter with { Nodes = ReplaceNode(chapter.Nodes, node) });

        AssertIssue(broken, ContentValidationCodes.MissingReference);
    }

    [Fact]
    public async Task ChapterPrerequisiteCyclesAreRejected()
    {
        var release = await LoadContentAsync();
        var first = release.Chapters[0] with
        {
            PrerequisiteChapterIds = new[] { release.Chapters[^1].Id },
        };
        var broken = ReplaceChapter(release, first);

        AssertIssue(broken, ContentValidationCodes.ChapterPrerequisiteCycle);
    }

    [Fact]
    public async Task CanonicalChapterWithoutCompletionPathIsRejected()
    {
        var release = await LoadContentAsync();
        var chapter = release.Chapters[0];
        var entry = chapter.Nodes.First(node => node.Id == chapter.EntryNodeId) with
        {
            NextNodeIds = Array.Empty<string>(),
        };
        var broken = ReplaceChapter(
            release,
            chapter with { Nodes = ReplaceNode(chapter.Nodes, entry) });

        AssertIssue(broken, ContentValidationCodes.ChapterNoCompletionPath);
    }

    [Fact]
    public async Task PostV1FeatureDependenciesAreRejectedFromV1Content()
    {
        var release = await LoadContentAsync();
        var chapter = release.Chapters[0] with
        {
            RequiredFeatures = release.Chapters[0].RequiredFeatures
                .Concat(PostV1FeatureFixture)
                .ToArray(),
        };
        var broken = ReplaceChapter(release, chapter);

        AssertIssue(broken, ContentValidationCodes.PostV1FeatureRequired);
    }

    [Fact]
    public async Task MissingLocalizationKeysAreRejected()
    {
        var release = await LoadContentAsync();
        var chapter = release.Chapters[0] with { TitleKey = string.Empty };
        var broken = ReplaceChapter(release, chapter);

        AssertIssue(broken, ContentValidationCodes.MissingLocalizationKey);
    }

    [Fact]
    public async Task FailedActivationKeepsLastKnownGoodRelease()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var manager = new ContentReleaseManager();

        var activated = await manager.TryActivateAsync(
            ReleaseDirectory,
            ContentValidationProfiles.V1,
            cancellationToken);

        Assert.True(activated.Activated);
        var active = Assert.IsType<ContentReleaseDefinition>(manager.ActiveRelease);

        var temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            $"life-played-content-{Guid.NewGuid():N}");
        Directory.CreateDirectory(temporaryDirectory);

        try
        {
            File.Copy(
                Path.Combine(ReleaseDirectory, "manifest.json"),
                Path.Combine(temporaryDirectory, "manifest.json"));
            File.Copy(
                Path.Combine(ReleaseDirectory, "content.json"),
                Path.Combine(temporaryDirectory, "content.json"));

            await File.AppendAllTextAsync(
                Path.Combine(temporaryDirectory, "content.json"),
                " ",
                cancellationToken);

            var rejected = await manager.TryActivateAsync(
                temporaryDirectory,
                ContentValidationProfiles.V1,
                cancellationToken);

            Assert.False(rejected.Activated);
            Assert.Same(active, manager.ActiveRelease);
            Assert.Contains(
                rejected.Validation.Issues,
                static issue => issue.Code == ContentValidationCodes.LoadFailed);
        }
        finally
        {
            Directory.Delete(temporaryDirectory, recursive: true);
        }
    }

    [Fact]
    public void ContentDefinitionJsonSchemaIsVersioned()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Fixtures",
            "content-definition.schema.json");

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;

        Assert.Equal(
            "urn:life-played:schema:content-definition:v1",
            root.GetProperty("$id").GetString());
        Assert.Equal(
            1,
            root.GetProperty("properties")
                .GetProperty("schemaVersion")
                .GetProperty("const")
                .GetInt32());
    }

    [Fact]
    public void ContentAssemblyHasNoUnityDependency()
    {
        var references = typeof(ContentValidator).Assembly
            .GetReferencedAssemblies()
            .Select(static assembly => assembly.Name ?? string.Empty)
            .ToArray();

        Assert.DoesNotContain(
            references,
            static name => name.StartsWith("Unity", StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<ContentReleaseDefinition> LoadContentAsync()
    {
        var loaded = await ContentReleaseLoader.LoadAsync(
            ReleaseDirectory,
            TestContext.Current.CancellationToken);
        return loaded.Content;
    }

    private static void AssertIssue(
        ContentReleaseDefinition release,
        string expectedCode)
    {
        var result = ContentValidator.Validate(
            release,
            ContentValidationProfiles.V1);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Issues,
            issue => issue.Code == expectedCode);
    }

    private static ContentReleaseDefinition ReplaceChapter(
        ContentReleaseDefinition release,
        ChapterDefinition replacement) =>
        release with
        {
            Chapters = release.Chapters
                .Select(chapter => chapter.Id == replacement.Id ? replacement : chapter)
                .ToArray(),
        };

    private static StoryNodeDefinition[] ReplaceNode(
        IReadOnlyList<StoryNodeDefinition> nodes,
        StoryNodeDefinition replacement) =>
        nodes
            .Select(node => node.Id == replacement.Id ? replacement : node)
            .ToArray();

    private static string Describe(ContentValidationResult result) =>
        string.Join(
            Environment.NewLine,
            result.Issues.Select(
                issue => $"{issue.Code}: {issue.ContentId} -> {issue.ReferenceId}: {issue.Message}"));
}
