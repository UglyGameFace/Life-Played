using LifePlayed.Domain.Common;
using LifePlayed.Domain.LifeOS;
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
