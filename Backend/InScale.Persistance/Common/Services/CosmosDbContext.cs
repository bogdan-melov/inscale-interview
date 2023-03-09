namespace InScale.Persistance.Common.Services
{
    using InScale.Persistance.Common.Contracts;
    using Microsoft.Azure.Cosmos;

    public class CosmosDbContext : ICosmosDbContext
    {
        public Container FileContainer { get; }

        public CosmosDbContext(Container fileContainer)
        {
            FileContainer = fileContainer;
        }
    }
}
