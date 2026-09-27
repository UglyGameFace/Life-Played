using LifePlayed.Domain.Accounts;
using LifePlayed.Domain.Common;
using LifePlayed.Domain.LifeOS;
using LifePlayed.Domain.Progression;
using LifePlayed.Domain.Sync;

namespace LifePlayed.Domain.Tests;

public sealed class StatePrimitiveTests
{
    [Fact]
    public void EntityIdsAreNonEmptyAndUnique()
    {
        var first = EntityId.New();
        var second = EntityId.New();

        Assert.False(first.IsEmpty);
        Assert.False(second.IsEmpty);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void EntityVersionIncrementsMonotonically()
    {
        var initial = EntityVersion.Initial;
        var next = initial.Next();

        Assert.Equal(0, initial.Value);
        Assert.Equal(1, next.Value);
    }

    [Fact]
    public void EntityVersionRejectsNegativeValues()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EntityVersion(-1));
    }

    [Fact]
    public void MinimalDomainRecordsCarryStableIdentityAndVersion()
    {
        var accountId = EntityId.New();
        var campaignId = EntityId.New();

        var account = new Account(
            accountId,
            AccountStatus.Active,
            "en-US",
            "America/New_York",
            DateTimeOffset.UtcNow,
            EntityVersion.Initial);

        var action = new LifeAction(
            EntityId.New(),
            accountId,
            "Write tests",
            ActionStatus.Active,
            TimeSpan.FromMinutes(30),
            null,
            null,
            EntityVersion.Initial);

        var quest = new Quest(
            EntityId.New(),
            accountId,
            "Ship foundation",
            QuestStatus.Active,
            null,
            campaignId,
            EntityVersion.Initial);

        var campaign = new Campaign(
            campaignId,
            accountId,
            "Launch app",
            null,
            CampaignStatus.Active,
            DateTimeOffset.UtcNow,
            null,
            null,
            EntityVersion.Initial);

        var phase = new CampaignPhase(
            EntityId.New(),
            campaignId,
            "Foundation",
            0,
            CampaignPhaseStatus.Active,
            EntityVersion.Initial);

        Assert.Equal(accountId, account.AccountId);
        Assert.Equal(accountId, action.AccountId);
        Assert.Equal(campaignId, quest.CampaignId);
        Assert.Equal(campaignId, campaign.CampaignId);
        Assert.Equal(campaignId, phase.CampaignId);
    }

    [Fact]
    public void DraftWorkMustBecomeActiveBeforeCompletion()
    {
        Assert.False(WorkStateTransitions.CanTransition(ActionStatus.Draft, ActionStatus.Completed));
        Assert.True(WorkStateTransitions.CanTransition(ActionStatus.Draft, ActionStatus.Active));
    }

    [Fact]
    public void PausedWorkCanResumeWithoutPunishment()
    {
        Assert.True(WorkStateTransitions.CanTransition(QuestStatus.Paused, QuestStatus.Active));
        Assert.True(WorkStateTransitions.CanTransition(CampaignStatus.Paused, CampaignStatus.Active));
    }

    [Fact]
    public void CompletedWorkCanBeReopenedOrArchived()
    {
        Assert.True(WorkStateTransitions.CanTransition(ActionStatus.Completed, ActionStatus.Active));
        Assert.True(WorkStateTransitions.CanTransition(ActionStatus.Completed, ActionStatus.Archived));
    }

    [Fact]
    public void CampaignPhaseStateTransitionsAreExplicit()
    {
        Assert.False(WorkStateTransitions.CanTransition(CampaignPhaseStatus.Draft, CampaignPhaseStatus.Completed));
        Assert.True(WorkStateTransitions.CanTransition(CampaignPhaseStatus.Draft, CampaignPhaseStatus.Active));
        Assert.True(WorkStateTransitions.CanTransition(CampaignPhaseStatus.Completed, CampaignPhaseStatus.Archived));
    }

    [Fact]
    public void RewardGrantCarriesIdempotencyAndRuleVersion()
    {
        var grant = new RewardGrant(
            EntityId.New(),
            EntityId.New(),
            "action",
            EntityId.New(),
            EconomyRulesV1.RuleVersion,
            "mutation:123",
            RewardGrantState.Pending,
            DateTimeOffset.UtcNow);

        Assert.Equal("mutation:123", grant.IdempotencyKey);
        Assert.Equal(EconomyRulesV1.RuleVersion, grant.RuleVersion);
        Assert.Equal(RewardGrantState.Pending, grant.State);
    }

    [Fact]
    public void DomainAssemblyHasNoFrameworkOrProviderDependencies()
    {
        var referenced = typeof(EntityId).Assembly
            .GetReferencedAssemblies()
            .Select(static assembly => assembly.Name ?? string.Empty)
            .ToArray();

        Assert.DoesNotContain(referenced, static name => name.StartsWith("Unity", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(referenced, static name => name.StartsWith("Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(referenced, static name => name.Contains("EntityFrameworkCore", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(referenced, static name => name.StartsWith("Npgsql", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ClientMutationHasOfflineSafeIdentityAndBaseVersion()
    {
        var mutation = new ClientMutation(
            MutationId.New(),
            EntityId.New(),
            EntityId.New(),
            EntityId.New(),
            "action.complete",
            EntityVersion.Initial,
            DateTimeOffset.UtcNow);

        Assert.False(mutation.MutationId.IsEmpty);
        Assert.Equal(0, mutation.BaseVersion.Value);
        Assert.Equal("action.complete", mutation.MutationType);
    }
}
