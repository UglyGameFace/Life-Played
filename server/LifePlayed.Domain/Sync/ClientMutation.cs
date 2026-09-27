using LifePlayed.Domain.Common;

namespace LifePlayed.Domain.Sync;

public readonly record struct MutationId(Guid Value)
{
    public static MutationId New() => new(Guid.CreateVersion7());

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString("D");
}

public sealed record ClientMutation(
    MutationId MutationId,
    EntityId AccountId,
    EntityId DeviceId,
    EntityId EntityId,
    string MutationType,
    EntityVersion BaseVersion,
    DateTimeOffset ClientTimestamp);
