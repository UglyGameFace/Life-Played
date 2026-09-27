using LifePlayed.Domain.Common;
using LifePlayed.Domain.LifeOS;
using LifePlayed.Domain.Sync;

namespace LifePlayed.Domain.Tests;

public sealed class StatePrimitiveTests
{
    [Fact]
    public void EntityIds_AreNonEmptyAndUnique()
    {
        var first = EntityId.New();
        var second = EntityId.New();

        Assert.False(first.IsEmpty);
        Assert.False(second.IsEmpty);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void EntityVersion_IncrementsMonotonically()
    {
        var initial = EntityVersion.Initial;
        var next = initial.Next();

        Assert.Equal(0, initial.Value);
        Assert.Equal(1, next.Value);
    }

    [Fact]
    public void EntityVersion_RejectsNegativeValues()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new EntityVersion(-1));
    }

    [Fact]
    public void DraftWork_MustBecomeActiveBeforeCompletion()
    {
        Assert.False(WorkStateTransitions.CanTransition(ActionStatus.Draft, ActionStatus.Completed));
        Assert.True(WorkStateTransitions.CanTransition(ActionStatus.Draft, ActionStatus.Active));
    }

    [Fact]
    public void PausedWork_CanResumeWithoutPunishment()
    {
        Assert.True(WorkStateTransitions.CanTransition(QuestStatus.Paused, QuestStatus.Active));
        Assert.True(WorkStateTransitions.CanTransition(CampaignStatus.Paused, CampaignStatus.Active));
    }

    [Fact]
    public void CompletedWork_CanBeReopenedOrArchived()
    {
        Assert.True(WorkStateTransitions.CanTransition(ActionStatus.Completed, ActionStatus.Active));
        Assert.True(WorkStateTransitions.CanTransition(ActionStatus.Completed, ActionStatus.Archived));
    }

    [Fact]
    public void ClientMutation_HasOfflineSafeIdentityAndBaseVersion()
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
