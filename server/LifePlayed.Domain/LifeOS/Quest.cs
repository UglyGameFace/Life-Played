using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.LifeOS;

public sealed record Quest(
    EntityId QuestId,
    EntityId AccountId,
    string Title,
    QuestStatus Status,
    DateTimeOffset? DueAt,
    EntityId? CampaignId,
    EntityVersion Version);
