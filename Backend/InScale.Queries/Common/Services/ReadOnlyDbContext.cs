namespace InScale.Queries.Common.Services
{
    using Microsoft.Azure.Cosmos;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class ReadOnlyDbContext
    {
        private const int AUTO_DECIDE_ITEMS_BUFFER = -1;
        private const int AUTO_DECIDE_CONCURRENCY = -1;
        private const int RETURN_MAX_ITEMS = -1;

        protected readonly Container _container;

        protected ReadOnlyDbContext(Container container)
        {
            _container = container;
        }

        public IQueryable<T> GetPartitionedEntities<T>(string partitionUid, Expression<Func<T, bool>>? predicate = default)
        {
            QueryRequestOptions requestOptions = new QueryRequestOptions
            {
                MaxBufferedItemCount = AUTO_DECIDE_ITEMS_BUFFER,
                MaxConcurrency = AUTO_DECIDE_CONCURRENCY,
                MaxItemCount = RETURN_MAX_ITEMS
            };

            requestOptions.PartitionKey = new PartitionKey(partitionUid);

            IQueryable<T> result =
                   _container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true, requestOptions: requestOptions);

            if (predicate != default)
            {
                result = result.Where(predicate);
            }

            return result;
        }
    }
}

