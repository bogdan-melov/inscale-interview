
namespace InScale.Persistance.InScaleFile.Repositories
{
    using InScale.Contracts.InScaleFile.Repositories;
    using InScale.Persistance.Common.Contracts;
    using InScale.Persistance.Common.Repositories;
    using Microsoft.Extensions.Logging;

    public class InScaleFileRepository : Repository<Entities.InScaleFile>, IInScaleFileRepository
    {
        public InScaleFileRepository(ICosmosDbContext dbContext, ILogger<Repository<Entities.InScaleFile>> logger) : base(dbContext.FileContainer, logger)
        {
        }
    }
}
