namespace InScale.Queries.Common.Contracts
{
    using System.Linq.Expressions;
    using System.Linq;
    using System;

    public interface IReadOnlyCosmosDbContext
    {
        IQueryable<T> GetPartitionedEntities<T>(string partitionUid, Expression<Func<T, bool>>? predicate = default);
    }
}
