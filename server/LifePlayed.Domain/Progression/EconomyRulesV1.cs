namespace LifePlayed.Domain.Progression;

public static class EconomyRulesV1
{
    public const string RuleVersion = "economy-v1";

    public static int AccountXpToNextLevel(int currentLevel)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(currentLevel, 1);
        var xp = 100d + (35d * Math.Pow(currentLevel - 1d, 1.35d));
        return Round(xp);
    }

    public static int LifeSkillXpToNextLevel(int currentLevel)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(currentLevel, 1);
        var xp = 60d + (20d * Math.Pow(currentLevel - 1d, 1.25d));
        return Round(xp);
    }

    public static double DurationScore(double expectedMinutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(expectedMinutes);
        return 20d * Math.Log(1d + (expectedMinutes / 10d));
    }

    public static double EffortScore(
        double expectedMinutes,
        double complexityMultiplier = 1d,
        double campaignMultiplier = 1d,
        double repetitionMultiplier = 1d,
        double integrityMultiplier = 1d)
    {
        var duration = DurationScore(expectedMinutes);
        var complexity = Math.Clamp(complexityMultiplier, 0.80d, 1.30d);
        var campaign = Math.Clamp(campaignMultiplier, 1.00d, 1.20d);
        var repetition = Math.Clamp(repetitionMultiplier, 0.10d, 1.00d);
        var integrity = Math.Clamp(integrityMultiplier, 0d, 1d);

        return duration * complexity * campaign * repetition * integrity;
    }

    public static RewardEvaluation EvaluateOrdinaryAction(
        double expectedMinutes,
        double complexityMultiplier = 1d,
        double campaignMultiplier = 1d,
        double repetitionMultiplier = 1d,
        double integrityMultiplier = 1d)
    {
        var effort = EffortScore(
            expectedMinutes,
            complexityMultiplier,
            campaignMultiplier,
            repetitionMultiplier,
            integrityMultiplier);

        var accountXp = AccountXpFromEffort(effort);
        return new RewardEvaluation(
            RuleVersion,
            effort,
            accountXp,
            TotalSkillXp(accountXp));
    }

    public static int AccountXpFromEffort(double effortScore)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(effortScore);
        return Round(Math.Clamp(effortScore, 5d, 120d));
    }

    public static int TotalSkillXp(int accountXp)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(accountXp);
        return Round(accountXp * 0.75d);
    }

    public static double DuplicateMultiplier(int equivalentOccurrenceNumber) =>
        equivalentOccurrenceNumber switch
        {
            < 1 => throw new ArgumentOutOfRangeException(
                nameof(equivalentOccurrenceNumber),
                "Occurrence number must be at least 1."),
            1 => 1.00d,
            2 => 0.50d,
            3 => 0.25d,
            _ => 0.10d,
        };

    public static double EffectiveFocusMinutes(
        TimeSpan sessionDuration,
        TimeSpan alreadyQualifiedToday)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sessionDuration, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThan(alreadyQualifiedToday, TimeSpan.Zero);

        var sessionMinutes = sessionDuration.TotalMinutes;
        var priorMinutes = alreadyQualifiedToday.TotalMinutes;

        const double fullRateLimit = 240d;

        var fullRateRemaining = Math.Max(0d, fullRateLimit - priorMinutes);
        var fullRateMinutes = Math.Min(sessionMinutes, fullRateRemaining);

        var remainingSession = sessionMinutes - fullRateMinutes;
        var reducedAlreadyUsed = Math.Clamp(priorMinutes - fullRateLimit, 0d, fullRateLimit);
        var reducedRemaining = Math.Max(0d, fullRateLimit - reducedAlreadyUsed);
        var reducedRateMinutes = Math.Min(remainingSession, reducedRemaining);

        return fullRateMinutes + (reducedRateMinutes * 0.50d);
    }

    public static double ClampMomentum(double momentum) =>
        Math.Clamp(momentum, 0d, 100d);

    private static int Round(double value) =>
        checked((int)Math.Round(value, MidpointRounding.AwayFromZero));
}
