using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.Accounts;

public enum AccountStatus
{
    Active = 0,
    DeletionRequested = 1,
    Deleted = 2,
}

public sealed record Account(
    EntityId AccountId,
    AccountStatus Status,
    string Locale,
    string TimeZone,
    DateTimeOffset CreatedAt,
    EntityVersion Version);
