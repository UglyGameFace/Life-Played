namespace LifePlayed.Domain.Common;

public readonly record struct EntityId(Guid Value)
{
    public static EntityId New() => new(Guid.CreateVersion7());

    public static EntityId Parse(string value) => new(Guid.Parse(value));

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString("D");
}
