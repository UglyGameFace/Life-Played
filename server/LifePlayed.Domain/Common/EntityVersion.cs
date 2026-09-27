namespace LifePlayed.Domain.Common;

public readonly record struct EntityVersion
{
    public EntityVersion(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        Value = value;
    }

    public long Value { get; }

    public EntityVersion Next() => new(checked(Value + 1));

    public static EntityVersion Initial => new(0);
}
