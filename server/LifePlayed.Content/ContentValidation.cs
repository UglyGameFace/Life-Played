using LifePlayed.Contracts.Content;

namespace LifePlayed.Content;

public static class ContentValidationCodes
{
    public const string DuplicateId = "duplicate_id";
    public const string MissingReference = "missing_reference";
    public const string MissingLocalizationKey = "missing_localization_key";
    public const string ChapterPrerequisiteCycle = "chapter_prerequisite_cycle";
    public const string ChapterNoCompletionPath = "chapter_no_completion_path";
    public const string ChapterMissingObjectiveHook = "chapter_missing_objective_hook";
    public const string PostV1FeatureRequired = "post_v1_feature_required";
    public const string ReleaseMismatch = "release_mismatch";
    public const string LoadFailed = "content_load_failed";
}

public sealed record ContentValidationIssue(
    string Code,
    string ContentId,
    string? ReferenceId,
    string Message);

public sealed record ContentValidationResult(
    IReadOnlyList<ContentValidationIssue> Issues)
{
    public bool IsValid => Issues.Count == 0;

    public static ContentValidationResult Valid { get; } =
        new(Array.Empty<ContentValidationIssue>());
}

public sealed record ContentValidationProfile(
    string Name,
    IReadOnlySet<string> AllowedFeatures);

public static class ContentValidationProfiles
{
    public static ContentValidationProfile V1 { get; } =
        new(
            "v1",
            new HashSet<string>(
                new[]
                {
                    ContentFeatureCatalog.Action,
                    ContentFeatureCatalog.Quest,
                    ContentFeatureCatalog.Campaign,
                    ContentFeatureCatalog.CampaignPhase,
                    ContentFeatureCatalog.Habit,
                    ContentFeatureCatalog.FocusSession,
                    ContentFeatureCatalog.RestPeriod,
                    ContentFeatureCatalog.Chronicle,
                    ContentFeatureCatalog.Companion,
                    ContentFeatureCatalog.WorldStructure,
                    ContentFeatureCatalog.LifeSkill,
                    ContentFeatureCatalog.NpcRelationship,
                    ContentFeatureCatalog.NextBestAction,
                },
                StringComparer.Ordinal));
}

public sealed class ContentValidator
{
    public ContentValidationResult Validate(
        ContentReleaseDefinition release,
        ContentValidationProfile profile)
    {
        ArgumentNullException.ThrowIfNull(release);
        ArgumentNullException.ThrowIfNull(profile);

        var issues = new List<ContentValidationIssue>();
        var ids = BuildIdIndex(release, issues);

        ValidateLocalizationKeys(release, issues);
        ValidateWorlds(release, ids, issues);
        ValidateSagas(release, ids, issues);
        ValidateChapters(release, ids, profile, issues);
        ValidateEntityWorldReferences(release, ids, issues);
        ValidateCampaignArchetypes(release, ids, issues);

        return new ContentValidationResult(
            issues
                .OrderBy(static issue => issue.Code, StringComparer.Ordinal)
                .ThenBy(static issue => issue.ContentId, StringComparer.Ordinal)
                .ThenBy(static issue => issue.ReferenceId, StringComparer.Ordinal)
                .ThenBy(static issue => issue.Message, StringComparer.Ordinal)
                .ToArray());
    }

    private static Dictionary<string, string> BuildIdIndex(
        ContentReleaseDefinition release,
        ICollection<ContentValidationIssue> issues)
    {
        var ids = new Dictionary<string, string>(StringComparer.Ordinal);

        void Add(string id, string type)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                issues.Add(new ContentValidationIssue(
                    ContentValidationCodes.DuplicateId,
                    type,
                    null,
                    "Content IDs must be non-empty."));
                return;
            }

            if (!ids.TryAdd(id, type))
            {
                issues.Add(new ContentValidationIssue(
                    ContentValidationCodes.DuplicateId,
                    id,
                    id,
                    $"Content ID '{id}' is duplicated."));
            }
        }

        foreach (var world in release.Worlds)
        {
            Add(world.Id, "world");
        }

        foreach (var saga in release.Sagas)
        {
            Add(saga.Id, "saga");
            foreach (var chapter in saga.ChapterIds)
            {
                _ = chapter;
            }
        }

        foreach (var chapter in release.Sagas
                     .SelectMany(saga => saga.ChapterIds)
                     .Select(id => FindChapter(release, id))
                     .Where(static chapter => chapter is not null)
                     .Cast<ChapterDefinition>())
        {
            Add(chapter.Id, "chapter");
            foreach (var node in chapter.Nodes)
            {
                Add(node.Id, "story_node");
            }
        }

        foreach (var npc in release.Npcs)
        {
            Add(npc.Id, "npc");
        }

        foreach (var companion in release.Companions)
        {
            Add(companion.Id, "companion");
        }

        foreach (var structure in release.WorldStructures)
        {
            Add(structure.Id, "world_structure");
        }

        foreach (var trigger in release.ChronicleTriggers)
        {
            Add(trigger.Id, "chronicle_trigger");
        }

        foreach (var reward in release.Rewards)
        {
            Add(reward.Id, "reward");
        }

        foreach (var archetype in release.CampaignArchetypes)
        {
            Add(archetype.Id, "campaign_archetype");
        }

        return ids;
    }

    private static void ValidateLocalizationKeys(
        ContentReleaseDefinition release,
        ICollection<ContentValidationIssue> issues)
    {
        foreach (var world in release.Worlds)
        {
            RequireKey(world.Id, world.NameKey, "NameKey", issues);
            RequireKey(world.Id, world.SummaryKey, "SummaryKey", issues);
            RequireKey(world.Id, world.RoleKey, "RoleKey", issues);
            RequireKey(world.Id, world.HubNameKey, "HubNameKey", issues);
        }

        foreach (var saga in release.Sagas)
        {
            RequireKey(saga.Id, saga.TitleKey, "TitleKey", issues);
            RequireKey(saga.Id, saga.SummaryKey, "SummaryKey", issues);
        }

        foreach (var chapter in AllChapters(release))
        {
            RequireKey(chapter.Id, chapter.TitleKey, "TitleKey", issues);
            RequireKey(chapter.Id, chapter.SummaryKey, "SummaryKey", issues);

            foreach (var node in chapter.Nodes)
            {
                RequireKey(node.Id, node.LocalizationKey, "LocalizationKey", issues);
            }
        }

        foreach (var npc in release.Npcs)
        {
            RequireKey(npc.Id, npc.NameKey, "NameKey", issues);
            RequireKey(npc.Id, npc.RoleKey, "RoleKey", issues);
        }

        foreach (var companion in release.Companions)
        {
            RequireKey(companion.Id, companion.NameKey, "NameKey", issues);
            RequireKey(companion.Id, companion.FamilyKey, "FamilyKey", issues);
        }

        foreach (var structure in release.WorldStructures)
        {
            RequireKey(structure.Id, structure.NameKey, "NameKey", issues);
        }

        foreach (var trigger in release.ChronicleTriggers)
        {
            RequireKey(trigger.Id, trigger.TitleKey, "TitleKey", issues);
        }

        foreach (var archetype in release.CampaignArchetypes)
        {
            RequireKey(archetype.Id, archetype.TitleTemplateKey, "TitleTemplateKey", issues);
            RequireKey(archetype.Id, archetype.SummaryTemplateKey, "SummaryTemplateKey", issues);
        }
    }

    private static void ValidateWorlds(
        ContentReleaseDefinition release,
        IReadOnlyDictionary<string, string> ids,
        ICollection<ContentValidationIssue> issues)
    {
        foreach (var world in release.Worlds)
        {
            foreach (var sagaId in world.SagaIds)
            {
                RequireReference(world.Id, sagaId, "saga", ids, issues);
            }
        }
    }

    private static void ValidateSagas(
        ContentReleaseDefinition release,
        IReadOnlyDictionary<string, string> ids,
        ICollection<ContentValidationIssue> issues)
    {
        foreach (var saga in release.Sagas)
        {
            RequireReference(saga.Id, saga.WorldId, "world", ids, issues);

            foreach (var chapterId in saga.ChapterIds)
            {
                RequireReference(saga.Id, chapterId, "chapter", ids, issues);
            }

            ValidateChapterPrerequisiteCycle(release, saga, issues);
        }
    }

    private static void ValidateChapters(
        ContentReleaseDefinition release,
        IReadOnlyDictionary<string, string> ids,
        ContentValidationProfile profile,
        ICollection<ContentValidationIssue> issues)
    {
        foreach (var chapter in AllChapters(release))
        {
            RequireReference(chapter.Id, chapter.SagaId, "saga", ids, issues);

            if (chapter.ObjectiveHooks.Count == 0)
            {
                issues.Add(new ContentValidationIssue(
                    ContentValidationCodes.ChapterMissingObjectiveHook,
                    chapter.Id,
                    null,
                    "Canonical chapters must expose at least one real-life objective hook."));
            }

            foreach (var feature in chapter.RequiredFeatures)
            {
                if (!profile.AllowedFeatures.Contains(feature))
                {
                    issues.Add(new ContentValidationIssue(
                        ContentValidationCodes.PostV1FeatureRequired,
                        chapter.Id,
                        feature,
                        $"Feature '{feature}' is not allowed in validation profile '{profile.Name}'."));
                }
            }

            foreach (var prerequisite in chapter.PrerequisiteChapterIds)
            {
                RequireReference(chapter.Id, prerequisite, "chapter", ids, issues);
            }

            var nodeIds = chapter.Nodes
                .Select(static node => node.Id)
                .ToHashSet(StringComparer.Ordinal);

            RequireLocalNode(chapter, chapter.EntryNodeId, nodeIds, issues);
            RequireLocalNode(chapter, chapter.CompletionNodeId, nodeIds, issues);

            foreach (var node in chapter.Nodes)
            {
                foreach (var nextNodeId in node.NextNodeIds)
                {
                    RequireLocalNode(chapter, nextNodeId, nodeIds, issues);
                }

                foreach (var npcId in node.NpcIds)
                {
                    RequireReference(node.Id, npcId, "npc", ids, issues);
                }

                foreach (var companionId in node.CompanionIds)
                {
                    RequireReference(node.Id, companionId, "companion", ids, issues);
                }

                foreach (var structureId in node.WorldStructureIds)
                {
                    RequireReference(node.Id, structureId, "world_structure", ids, issues);
                }

                foreach (var rewardId in node.RewardIds)
                {
                    RequireReference(node.Id, rewardId, "reward", ids, issues);
                }

                foreach (var triggerId in node.ChronicleTriggerIds)
                {
                    RequireReference(node.Id, triggerId, "chronicle_trigger", ids, issues);
                }
            }

            if (!HasCompletionPath(chapter))
            {
                issues.Add(new ContentValidationIssue(
                    ContentValidationCodes.ChapterNoCompletionPath,
                    chapter.Id,
                    chapter.CompletionNodeId,
                    "The chapter entry node cannot reach the completion node."));
            }
        }
    }

    private static void ValidateEntityWorldReferences(
        ContentReleaseDefinition release,
        IReadOnlyDictionary<string, string> ids,
        ICollection<ContentValidationIssue> issues)
    {
        foreach (var npc in release.Npcs)
        {
            RequireReference(npc.Id, npc.WorldId, "world", ids, issues);
        }

        foreach (var companion in release.Companions)
        {
            RequireReference(companion.Id, companion.WorldId, "world", ids, issues);
        }

        foreach (var structure in release.WorldStructures)
        {
            RequireReference(structure.Id, structure.WorldId, "world", ids, issues);
        }

        foreach (var trigger in release.ChronicleTriggers)
        {
            RequireReference(trigger.Id, trigger.WorldId, "world", ids, issues);
        }
    }

    private static void ValidateCampaignArchetypes(
        ContentReleaseDefinition release,
        IReadOnlyDictionary<string, string> ids,
        ICollection<ContentValidationIssue> issues)
    {
        foreach (var archetype in release.CampaignArchetypes)
        {
            RequireReference(archetype.Id, archetype.WorldId, "world", ids, issues);
        }
    }

    private static void ValidateChapterPrerequisiteCycle(
        ContentReleaseDefinition release,
        SagaDefinition saga,
        ICollection<ContentValidationIssue> issues)
    {
        var chapters = saga.ChapterIds
            .Select(id => FindChapter(release, id))
            .Where(static chapter => chapter is not null)
            .Cast<ChapterDefinition>()
            .ToDictionary(static chapter => chapter.Id, StringComparer.Ordinal);

        var visiting = new HashSet<string>(StringComparer.Ordinal);
        var visited = new HashSet<string>(StringComparer.Ordinal);

        bool Visit(string chapterId)
        {
            if (visiting.Contains(chapterId))
            {
                return true;
            }

            if (!visited.Add(chapterId))
            {
                return false;
            }

            visiting.Add(chapterId);

            if (chapters.TryGetValue(chapterId, out var chapter))
            {
                foreach (var prerequisite in chapter.PrerequisiteChapterIds)
                {
                    if (chapters.ContainsKey(prerequisite) && Visit(prerequisite))
                    {
                        return true;
                    }
                }
            }

            visiting.Remove(chapterId);
            return false;
        }

        if (saga.ChapterIds.Any(Visit))
        {
            issues.Add(new ContentValidationIssue(
                ContentValidationCodes.ChapterPrerequisiteCycle,
                saga.Id,
                null,
                "The saga contains a cycle in chapter prerequisites."));
        }
    }

    private static bool HasCompletionPath(ChapterDefinition chapter)
    {
        var nodes = chapter.Nodes.ToDictionary(static node => node.Id, StringComparer.Ordinal);

        if (!nodes.ContainsKey(chapter.EntryNodeId) ||
            !nodes.ContainsKey(chapter.CompletionNodeId))
        {
            return false;
        }

        var pending = new Queue<string>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        pending.Enqueue(chapter.EntryNodeId);

        while (pending.TryDequeue(out var nodeId))
        {
            if (!visited.Add(nodeId))
            {
                continue;
            }

            if (nodeId == chapter.CompletionNodeId)
            {
                return true;
            }

            if (!nodes.TryGetValue(nodeId, out var node))
            {
                continue;
            }

            foreach (var next in node.NextNodeIds)
            {
                pending.Enqueue(next);
            }
        }

        return false;
    }

    private static IEnumerable<ChapterDefinition> AllChapters(
        ContentReleaseDefinition release) =>
        release.Sagas
            .SelectMany(static saga => saga.ChapterIds)
            .Distinct(StringComparer.Ordinal)
            .Select(id => FindChapter(release, id))
            .Where(static chapter => chapter is not null)
            .Cast<ChapterDefinition>();

    private static ChapterDefinition? FindChapter(
        ContentReleaseDefinition release,
        string chapterId)
    {
        foreach (var saga in release.Sagas)
        {
            var chapter = ChapterRegistry.Get(release, saga.Id, chapterId);
            if (chapter is not null)
            {
                return chapter;
            }
        }

        return null;
    }

    private static void RequireKey(
        string contentId,
        string value,
        string fieldName,
        ICollection<ContentValidationIssue> issues)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        issues.Add(new ContentValidationIssue(
            ContentValidationCodes.MissingLocalizationKey,
            contentId,
            fieldName,
            $"Localization key '{fieldName}' is required."));
    }

    private static void RequireReference(
        string contentId,
        string referenceId,
        string expectedType,
        IReadOnlyDictionary<string, string> ids,
        ICollection<ContentValidationIssue> issues)
    {
        if (ids.TryGetValue(referenceId, out var actualType) &&
            actualType == expectedType)
        {
            return;
        }

        issues.Add(new ContentValidationIssue(
            ContentValidationCodes.MissingReference,
            contentId,
            referenceId,
            $"Reference '{referenceId}' must resolve to content type '{expectedType}'."));
    }

    private static void RequireLocalNode(
        ChapterDefinition chapter,
        string nodeId,
        IReadOnlySet<string> nodeIds,
        ICollection<ContentValidationIssue> issues)
    {
        if (nodeIds.Contains(nodeId))
        {
            return;
        }

        issues.Add(new ContentValidationIssue(
            ContentValidationCodes.MissingReference,
            chapter.Id,
            nodeId,
            $"Story node '{nodeId}' must exist inside chapter '{chapter.Id}'."));
    }

    private static class ChapterRegistry
    {
        private static readonly Dictionary<ContentReleaseDefinition, IReadOnlyDictionary<string, ChapterDefinition>>
            Cache = new(ReferenceEqualityComparer.Instance);

        public static ChapterDefinition? Get(
            ContentReleaseDefinition release,
            string sagaId,
            string chapterId)
        {
            _ = sagaId;
            if (!Cache.TryGetValue(release, out var chapters))
            {
                chapters = Build(release);
                Cache[release] = chapters;
            }

            return chapters.TryGetValue(chapterId, out var chapter)
                ? chapter
                : null;
        }

        public static void Register(
            ContentReleaseDefinition release,
            IReadOnlyList<ChapterDefinition> chapters) =>
            Cache[release] = chapters.ToDictionary(static chapter => chapter.Id, StringComparer.Ordinal);

        private static IReadOnlyDictionary<string, ChapterDefinition> Build(
            ContentReleaseDefinition release) =>
            new Dictionary<string, ChapterDefinition>(StringComparer.Ordinal);
    }

    public static ContentReleaseDefinition WithChapters(
        ContentReleaseDefinition release,
        IReadOnlyList<ChapterDefinition> chapters)
    {
        ChapterRegistry.Register(release, chapters);
        return release;
    }
}
