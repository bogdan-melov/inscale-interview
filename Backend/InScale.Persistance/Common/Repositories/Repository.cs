﻿namespace InScale.Persistance.Common.Repositories
{
    using FluentResults;
    using InScale.Contracts.Exceptions;
    using InScale.Persistance.Common.Entities;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using Result = FluentResults.Result;

    public abstract class Repository<T> where T : Entity<T>
    {
        private const int AUTO_DECIDE_ITEMS_BUFFER = -1;
        private const int AUTO_DECIDE_CONCURRENCY = -1;
        private const int RETURN_MAX_ITEMS = -1;

        private readonly string _entityName = typeof(T).Name.ToUpperInvariant();

        protected readonly Container _container;
        protected readonly ILogger<Repository<T>> _logger;

        protected Repository(Container container, ILogger<Repository<T>> logger)
        {
            _container = container;
            _logger = logger;
        }

        protected async Task<Result> AddAsync(T entity)
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

        protected async Task<Result> UpsertBulkAsync(IEnumerable<T> entities)
        {
            try
            {
                var concurrentTasks = new List<Task>();

                foreach (var entity in entities)
                {
                    concurrentTasks.Add(_container.UpsertItemAsync(entity));
                }

                await Task.WhenAll(concurrentTasks);

                return Result.Ok();
            }
            catch (NoSqlException ex)
            {
                return HandleNoSqlException(ex);
            }
        }

        protected async Task<Result> UpdateAsync<F>(F entity) where F : Entity<F>
        {
            try
            {
                await _container.UpsertItemAsync(entity);

                return Result.Ok();
            }
            catch (NoSqlException ex)
            {
                return HandleNoSqlException(ex);
            }
        }

        protected async Task<Result> DeleteAsync(string partitionId, Guid entityUid)
        {
            try
            {
                await _container.DeleteItemAsync<T>(entityUid.ToString(), new PartitionKey(partitionId));

                return Result.Ok();
            }
            catch (NoSqlException ex)
            {
                return HandleNoSqlException(ex);
            }
        }

        protected IQueryable<T> GetCrossPartitionEntities(Expression<Func<T, bool>> predicate) =>
            _container.GetItemLinqQueryable<T>(requestOptions: GetPagedQuerySettings())
              .Where(e => _entityName.Equals(e.EntityName, StringComparison.InvariantCultureIgnoreCase))
              .Where(predicate);

        protected IQueryable<T> GetPartitionedEntities(string partitionUid, Expression<Func<T, bool>>? predicate = default)
        {
            QueryRequestOptions requestOptions = GetPagedQuerySettings();
            requestOptions.PartitionKey = new PartitionKey(partitionUid);

            IQueryable<T> result =
                   _container.GetItemLinqQueryable<T>(allowSynchronousQueryExecution: true, requestOptions: requestOptions)
                             .Where(e => _entityName.Equals(e.EntityName, StringComparison.InvariantCultureIgnoreCase));

            if (predicate != default)
            {
                result = result.Where(predicate);
            }

            return result;
        }

        private Result<F> HandleNoSqlException<F>(NoSqlException ex) where F : Entity<F>
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