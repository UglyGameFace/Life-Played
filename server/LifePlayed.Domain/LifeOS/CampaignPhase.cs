using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.LifeOS;

public enum CampaignPhaseStatus
{
    Draft = 0,
    Active = 1,
    Completed = 2,
    Archived = 3,
}

public sealed record CampaignPhase(
    EntityId CampaignPhaseId,
    EntityId CampaignId,
    string Title,
    int Position,
    CampaignPhaseStatus Status,
    EntityVersion Version);
