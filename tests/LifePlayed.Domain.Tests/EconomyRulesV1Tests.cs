using LifePlayed.Domain.Progression;

namespace LifePlayed.Domain.Tests;

public sealed class EconomyRulesV1Tests
{
    [Theory]
    [InlineData(1, 100)]
    [InlineData(5, 327)]
    [InlineData(10, 780)]
    [InlineData(25, 2655)]
    [InlineData(50, 6796)]
    [InlineData(100, 17405)]
    public void AccountXpCurve_MatchesVersionedFoundationFormula(int level, int expected)
    {
        Assert.Equal(expected, EconomyRulesV1.AccountXpToNextLevel(level));
    }

    [Fact]
    public void DurationScore_UsesLogarithmicGrowth()
    {
        var fiveMinutes = EconomyRulesV1.DurationScore(5);
        var sixtyMinutes = EconomyRulesV1.DurationScore(60);
        var fourHours = EconomyRulesV1.DurationScore(240);

        Assert.InRange(fiveMinutes, 8.10d, 8.12d);
        Assert.InRange(sixtyMinutes, 38.91d, 38.93d);
        Assert.InRange(fourHours, 64.37d, 64.39d);
    }

    [Theory]
    [InlineData(1, 1.00)]
    [InlineData(2, 0.50)]
    [InlineData(3, 0.25)]
    [InlineData(4, 0.10)]
    [InlineData(50, 0.10)]
    public void DuplicateMultiplier_DiminishesEquivalentUnscheduledWork(int occurrence, double expected)
    {
        Assert.Equal(expected, EconomyRulesV1.DuplicateMultiplier(occurrence), 2);
    }

    [Fact]
    public void FocusReward_FullRateForFirstFourHours()
    {
        var effective = EconomyRulesV1.EffectiveFocusMinutes(
            TimeSpan.FromMinutes(60),
            TimeSpan.FromHours(2));

        Assert.Equal(60d, effective);
    }

    [Fact]
    public void FocusReward_HalfRateBetweenFourAndEightHours()
    {
        var effective = EconomyRulesV1.EffectiveFocusMinutes(
            TimeSpan.FromMinutes(60),
            TimeSpan.FromHours(5));

        Assert.Equal(30d, effective);
    }

    [Fact]
    public void FocusReward_SplitsSessionAcrossTaperBoundary()
    {
        var effective = EconomyRulesV1.EffectiveFocusMinutes(
            TimeSpan.FromMinutes(120),
            TimeSpan.FromHours(3.5));

        Assert.Equal(75d, effective);
    }

    [Fact]
    public void FocusReward_StopsIncrementalTimeRewardAfterEightHours()
    {
        var effective = EconomyRulesV1.EffectiveFocusMinutes(
            TimeSpan.FromMinutes(60),
            TimeSpan.FromHours(8));

        Assert.Equal(0d, effective);
    }

    [Theory]
    [InlineData(-10, 0)]
    [InlineData(0, 0)]
    [InlineData(50, 50)]
    [InlineData(100, 100)]
    [InlineData(125, 100)]
    public void Momentum_IsBounded(double value, double expected)
    {
        Assert.Equal(expected, EconomyRulesV1.ClampMomentum(value));
    }

    [Fact]
    public void AccountXp_IsClampedToOrdinaryActionBand()
    {
        Assert.Equal(5, EconomyRulesV1.AccountXpFromEffort(0));
        Assert.Equal(120, EconomyRulesV1.AccountXpFromEffort(9999));
    }

    [Fact]
    public void SkillXp_IsSeventyFivePercentOfAccountXp()
    {
        Assert.Equal(45, EconomyRulesV1.TotalSkillXp(60));
    }
}
