namespace LifePlayed.Domain.Progression;

public sealed record RewardEvaluation(
    string RuleVersion,
    double EffortScore,
    int AccountXp,
    int TotalSkillXp);
