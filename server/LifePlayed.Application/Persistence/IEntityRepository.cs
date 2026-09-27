using LifePlayed.Domain.Common;

namespace LifePlayed.Application.Persistence;

public interface IEntityRepository<TEntity>
    where TEntity : class
{
    ValueTask<TEntity?> GetAsync(
        EntityId entityId,
        CancellationToken cancellationToken = default);

    ValueTask UpsertAsync(
        TEntity entity,
        EntityVersion expectedVersion,
        CancellationToken cancellationToken = default);
}
