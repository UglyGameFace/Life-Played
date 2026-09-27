namespace LifePlayed.Domain.LifeOS;

public static class WorkStateTransitions
{
    public static bool CanTransition(ActionStatus from, ActionStatus to) =>
        CanTransitionCore((int)from, (int)to);

    public static bool CanTransition(QuestStatus from, QuestStatus to) =>
        CanTransitionCore((int)from, (int)to);

    public static bool CanTransition(CampaignStatus from, CampaignStatus to) =>
        CanTransitionCore((int)from, (int)to);

    private static bool CanTransitionCore(int from, int to)
    {
        if (from == to)
        {
            return true;
        }

        return from switch
        {
            0 => to is 1 or 5,
            1 => to is 2 or 3 or 4 or 5,
            2 => to is 1 or 4 or 5,
            3 => to is 1 or 5,
            4 => to is 1 or 5,
            5 => to is 1,
            _ => false,
        };
    }
}
