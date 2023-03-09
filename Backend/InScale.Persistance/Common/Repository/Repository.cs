namespace InScale.Persistance.Common.Repository
{
    using FluentResults;
    using InScale.Common.Common.Result;
    using InScale.Contracts.Exceptions;
    using InScale.Domain.Common;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using Result = FluentResults.Result;

    public abstract class Repository
    {
        private const int AUTO_DECIDE_ITEMS_BUFFER = -1;
        private const int AUTO_DECIDE_CONCURRENCY = -1;
        private const int RETURN_MAX_ITEMS = -1;

        protected readonly Container _container;
        protected readonly ILogger<Repository> _logger;

        protected Repository(Container container, ILogger<Repository> logger)
        {
            _container = container;
            _logger = logger;
        }

        protected async Task<Result> AddAsync<T>(T entity)
        {
            try
            {
                await _container.CreateItemAsync(entity);

                return Result.Ok();
            }
            catch (NoSqlException ex)
            {
                return HandleNoSqlException(ex);
            }
        }

        protected IQueryable<T> GetPartitionedEntities<T>(string partitionUid, Expression<Func<T, bool>>? predicate = default)
        {
            QueryRequestOptions requestOptions = GetPagedQuerySettings();
            requestOptions.PartitionKey = new PartitionKey(partitionUid);

            IQueryable<T> result =
                   _container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true, requestOptions: requestOptions);

            if (predicate != default)
            {
                result = result.Where(predicate);
            }

            return result;
        }

        private Result<F> HandleNoSqlException<F>(NoSqlException ex) where F : Entity
        {
            switch (ex.HttpStatusCode)
            {
                case HttpStatusCode.NotFound:
                    return Result.Fail<F>(ResultErrorCodes.NotFound);
                case HttpStatusCode.Conflict:
                    return Result.Fail<F>(ResultErrorCodes.Conflicted);
                default:
                    _logger.Log(LogLevel.Error, nameof(HandleNoSqlException), ex);
                    return Result.Fail<F>(ResultErrorCodes.InternalServerError);
            }
        }

        private Result HandleNoSqlException(NoSqlException ex)
        {
            switch (ex.HttpStatusCode)
            {
                case HttpStatusCode.NotFound:
                    return Result.Fail(ResultErrorCodes.NotFound);
                case HttpStatusCode.Conflict:
                    return Result.Fail(ResultErrorCodes.Conflicted);
                default:
                    _logger.Log(LogLevel.Error, nameof(HandleNoSqlException), ex);
                    return Result.Fail(ResultErrorCodes.InternalServerError);
            }
        }

        private QueryRequestOptions GetPagedQuerySettings() =>
          new QueryRequestOptions
          {
              MaxBufferedItemCount = AUTO_DECIDE_ITEMS_BUFFER,
              MaxConcurrency = AUTO_DECIDE_CONCURRENCY,
              MaxItemCount = RETURN_MAX_ITEMS
          };
    }
}