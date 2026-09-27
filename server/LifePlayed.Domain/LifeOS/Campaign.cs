using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.LifeOS;

public sealed record Campaign(
    EntityId CampaignId,
    EntityId AccountId,
    string Title,
    string? Description,
    CampaignStatus Status,
    DateTimeOffset? StartAt,
    DateTimeOffset? DueAt,
    DateTimeOffset? CompletedAt,
    EntityVersion Version);
