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
    public void AccountXpCurveMatchesVersionedFoundationFormula(int level, int expected)
    {
        Assert.Equal(expected, EconomyRulesV1.AccountXpToNextLevel(level));
    }

    [Theory]
    [InlineData(1, 60)]
    [InlineData(5, 173)]
    [InlineData(10, 372)]
    [InlineData(25, 1122)]
    [InlineData(50, 2653)]
    [InlineData(100, 6306)]
    public void LifeSkillXpCurveMatchesVersionedFoundationFormula(int level, int expected)
    {
        Assert.Equal(expected, EconomyRulesV1.LifeSkillXpToNextLevel(level));
    }

    [Fact]
    public void DurationScoreUsesLogarithmicGrowth()
    {
        var fiveMinutes = EconomyRulesV1.DurationScore(5);
        var sixtyMinutes = EconomyRulesV1.DurationScore(60);
        var fourHours = EconomyRulesV1.DurationScore(240);

        Assert.InRange(fiveMinutes, 8.10d, 8.12d);
        Assert.InRange(sixtyMinutes, 38.91d, 38.93d);
        Assert.InRange(fourHours, 64.37d, 64.39d);
    }

    [Fact]
    public void RewardEvaluationIsDeterministicAndVersioned()
    {
        var first = EconomyRulesV1.EvaluateOrdinaryAction(60, 1.2d, 1.1d, 1d, 1d);
        var second = EconomyRulesV1.EvaluateOrdinaryAction(60, 1.2d, 1.1d, 1d, 1d);

        Assert.Equal(first, second);
        Assert.Equal(EconomyRulesV1.RuleVersion, first.RuleVersion);
        Assert.Equal(EconomyRulesV1.TotalSkillXp(first.AccountXp), first.TotalSkillXp);
    }

    [Theory]
    [InlineData(1, 1.00)]
    [InlineData(2, 0.50)]
    [InlineData(3, 0.25)]
    [InlineData(4, 0.10)]
    [InlineData(50, 0.10)]
    public void DuplicateMultiplierDiminishesEquivalentUnscheduledWork(int occurrence, double expected)
    {
        Assert.Equal(expected, EconomyRulesV1.DuplicateMultiplier(occurrence), 2);
    }

    [Fact]
    public void FocusRewardUsesFullRateForFirstFourHours()
    {
        var effective = EconomyRulesV1.EffectiveFocusMinutes(
            TimeSpan.FromMinutes(60),
            TimeSpan.FromHours(2));

        Assert.Equal(60d, effective);
    }

    [Fact]
    public void FocusRewardUsesHalfRateBetweenFourAndEightHours()
    {
        var effective = EconomyRulesV1.EffectiveFocusMinutes(
            TimeSpan.FromMinutes(60),
            TimeSpan.FromHours(5));

        Assert.Equal(30d, effective);
    }

    [Fact]
    public void FocusRewardSplitsSessionAcrossTaperBoundary()
    {
        var effective = EconomyRulesV1.EffectiveFocusMinutes(
            TimeSpan.FromMinutes(120),
            TimeSpan.FromHours(3.5));

        Assert.Equal(75d, effective);
    }

    [Fact]
    public void FocusRewardStopsIncrementalTimeRewardAfterEightHours()
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
    public void MomentumIsBounded(double value, double expected)
    {
        Assert.Equal(expected, EconomyRulesV1.ClampMomentum(value));
    }

    [Fact]
    public void EffortMultipliersAreClampedToDocumentedBounds()
    {
        var low = EconomyRulesV1.EffortScore(
            60,
            complexityMultiplier: -5,
            campaignMultiplier: -5,
            repetitionMultiplier: -5,
            integrityMultiplier: -5);

        var expectedLow = EconomyRulesV1.DurationScore(60) * 0.80d * 1.00d * 0.10d * 0d;
        Assert.Equal(expectedLow, low, 8);

        var high = EconomyRulesV1.EffortScore(
            60,
            complexityMultiplier: 99,
            campaignMultiplier: 99,
            repetitionMultiplier: 99,
            integrityMultiplier: 99);

        var expectedHigh = EconomyRulesV1.DurationScore(60) * 1.30d * 1.20d * 1.00d * 1.00d;
        Assert.Equal(expectedHigh, high, 8);
    }

    [Fact]
    public void InvalidEconomyInputsAreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => EconomyRulesV1.AccountXpToNextLevel(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => EconomyRulesV1.LifeSkillXpToNextLevel(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => EconomyRulesV1.DurationScore(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => EconomyRulesV1.AccountXpFromEffort(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => EconomyRulesV1.TotalSkillXp(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => EconomyRulesV1.DuplicateMultiplier(0));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            EconomyRulesV1.EffectiveFocusMinutes(TimeSpan.FromMinutes(-1), TimeSpan.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            EconomyRulesV1.EffectiveFocusMinutes(TimeSpan.Zero, TimeSpan.FromMinutes(-1)));
    }

    [Fact]
    public void AccountXpIsClampedToOrdinaryActionBand()
    {
        Assert.Equal(5, EconomyRulesV1.AccountXpFromEffort(0));
        Assert.Equal(120, EconomyRulesV1.AccountXpFromEffort(9999));
    }

    [Fact]
    public void SkillXpIsSeventyFivePercentOfAccountXp()
    {
        Assert.Equal(45, EconomyRulesV1.TotalSkillXp(60));
    }
}
