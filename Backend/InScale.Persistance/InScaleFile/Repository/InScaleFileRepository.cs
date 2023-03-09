
namespace InScale.Persistance.InScaleFile.Repository
{
    using FluentResults;
    using InScale.Contracts.InScaleFile.Repositories;
    using InScale.Domain.InScaleFile.Entities;
    using InScale.Persistance.Common.Contracts;
    using InScale.Persistance.Common.Extensions;
    using InScale.Persistance.Common.Repository;
    using InScale.Persistance.InScaleFile.Factory;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InScaleFileRepository : Repository, IInScaleFileRepository
    {
        public InScaleFileRepository(ICosmosDbContext dbContext, ILogger<Repository> logger) : base(dbContext.FileContainer, logger)
        {
        }

        public async Task<Result<List<InScaleFile>>> GetInScaleFilesByFileId(string fileId)
        {
            IQueryable<Entities.InScaleFile> inScaleFilesQuery = GetPartitionedEntities<Entities.InScaleFile>(partitionUid: fileId);

            IReadOnlyList<Entities.InScaleFile> inScaleFileCosmosResponse = await inScaleFilesQuery.ExecuteQueryAsync();

            return inScaleFileCosmosResponse.ToList().ToInScaleFiles();
        }

        public async Task<Result> SaveInScaleFile(InScaleFile file)
        {
            Entities.InScaleFile dbInScaleFile = file.ToInScaleFileEntity();

            Result insertResult = await AddAsync(dbInScaleFile);
           
            if (insertResult.IsFailed)
            {
                return Result.Fail(insertResult.Errors);
            }

            return insertResult;
        }
    }
}
