namespace InScale.Persistance.Common.Services
{
    using InScale.Contracts.Settings;
    using InScale.Persistance.Common.Contracts;
    using Microsoft.Azure.Cosmos;
    using System.Threading.Tasks;

    public class CosmosDbContext : ICosmosDbContext
    {
        public Container FileContainer { get; }

        public CosmosDbContext(Container fileContainer)
        {
            FileContainer = fileContainer;
        }

        public static async Task<CosmosDbContext> CreateAsync(ICosmosDbSettings settings)
        {
            CosmosClientOptions cosmosClientOptions = GetCosmosClientOptions();

            var cosmosClient = new CosmosClient(settings.ConnectionString, cosmosClientOptions);

            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(settings.DatabaseName);
            Container fileContainer = await CreateContainerIfNotExistsAsync(database, settings.FileContainer);

            return new CosmosDbContext(fileContainer);
        }

        private static async Task<Container> CreateContainerIfNotExistsAsync<T>(Database database, T container) where T : IDbContainer
        {
            var containerOptions = new ContainerProperties
            {
                Id = container.Name,
                PartitionKeyPath = container.PartitionKey,
                PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2
            };

            Container containerResponse =
                await database.CreateContainerIfNotExistsAsync(containerOptions);

            return containerResponse;
        }

        private static CosmosClientOptions GetCosmosClientOptions()
        {
            var cosmosClientOptions = new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },
                AllowBulkExecution = true
            };

            cosmosClientOptions.ConnectionMode = ConnectionMode.Direct;

            return cosmosClientOptions;
        }
    }
}
