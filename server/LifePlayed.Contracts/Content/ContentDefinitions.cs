namespace LifePlayed.Contracts.Content;

public sealed record ContentReleaseDefinition(
    int SchemaVersion,
    string ReleaseId,
    string ReleaseVersion,
    IReadOnlyList<IdentityWorldDefinition> Worlds,
    IReadOnlyList<SagaDefinition> Sagas,
    IReadOnlyList<NpcDefinition> Npcs,
    IReadOnlyList<CompanionDefinition> Companions,
    IReadOnlyList<WorldStructureDefinition> WorldStructures,
    IReadOnlyList<ChronicleTriggerDefinition> ChronicleTriggers,
    IReadOnlyList<ContentRewardDefinition> Rewards,
    IReadOnlyList<CampaignArchetypeFramingDefinition> CampaignArchetypes);

public sealed record IdentityWorldDefinition(
    string Id,
    string NameKey,
    string SummaryKey,
    string RoleKey,
    string HubNameKey,
    IReadOnlyList<string> SagaIds);

public sealed record SagaDefinition(
    string Id,
    string WorldId,
    string TitleKey,
    string SummaryKey,
    IReadOnlyList<string> ChapterIds,
    string PostSagaStateKey);

public sealed record ChapterDefinition(
    string Id,
    string SagaId,
    string TitleKey,
    string SummaryKey,
    IReadOnlyList<string> PrerequisiteChapterIds,
    string EntryNodeId,
    string CompletionNodeId,
    IReadOnlyList<string> ObjectiveHooks,
    IReadOnlyList<string> RequiredFeatures,
    IReadOnlyList<StoryNodeDefinition> Nodes);

public sealed record StoryNodeDefinition(
    string Id,
    string Kind,
    string LocalizationKey,
    IReadOnlyList<string> NextNodeIds,
    IReadOnlyList<string> NpcIds,
    IReadOnlyList<string> CompanionIds,
    IReadOnlyList<string> WorldStructureIds,
    IReadOnlyList<string> RewardIds,
    IReadOnlyList<string> ChronicleTriggerIds);

public sealed record NpcDefinition(
    string Id,
    string WorldId,
    string NameKey,
    string RoleKey);

public sealed record CompanionDefinition(
    string Id,
    string WorldId,
    string NameKey,
    string FamilyKey,
    string Tier);

public sealed record WorldStructureDefinition(
    string Id,
    string WorldId,
    string NameKey,
    string StructureType);

public sealed record ChronicleTriggerDefinition(
    string Id,
    string WorldId,
    string EntryType,
    string TitleKey);

public sealed record ContentRewardDefinition(
    string Id,
    string RuleKey);

public sealed record CampaignArchetypeFramingDefinition(
    string Id,
    string WorldId,
    string Archetype,
    string TitleTemplateKey,
    string SummaryTemplateKey);
