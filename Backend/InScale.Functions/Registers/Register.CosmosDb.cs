namespace InScale.Functions.Registers
{
    using InScale.Contracts.Settings;
    using InScale.Functions.Settings;
    using InScale.Persistance.Common.Contracts;
    using InScale.Persistance.Common.Services;
    using InScale.Queries.Common.Contracts;
    using InScale.Queries.Common.Services;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class Register
    {
        public static IServiceCollection RegisterCosmosDb(this IServiceCollection services, IConfiguration configuration) =>
             services.RegisterDatabaseContext(configuration);

        private static IServiceCollection RegisterDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            ICosmosDbSettings settings = new CosmosDbSettings(configuration);

            CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },
                AllowBulkExecution = true,
                ConnectionMode = ConnectionMode.Direct
            };

            var cosmosClient = new CosmosClient(settings.ConnectionString, cosmosClientOptions);

            Database database = cosmosClient.CreateDatabaseIfNotExistsAsync(settings.DatabaseName).Result;

            var containerOptions = new ContainerProperties
            {
                Id = settings.FileContainer.Name,
                PartitionKeyPath = settings.FileContainer.PartitionKey,
                PartitionKeyDefinitionVersion = PartitionKeyDefinitionVersion.V2
            };

            Container fileContainer = database.CreateContainerIfNotExistsAsync(containerOptions).Result;

            services.AddSingleton<ICosmosDbContext>(provider =>
            {
                return new CosmosDbContext(fileContainer);
            });

            services.AddSingleton<IInScaleFileReadOnlyDbContext>(provider =>
            {
                return new InScaleFileReadOnlyDbContext(fileContainer);
            });

            return services;
        }
    }
}
