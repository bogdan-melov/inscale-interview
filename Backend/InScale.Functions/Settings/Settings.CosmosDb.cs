namespace InScale.Functions.Settings
{
    using InScale.Contracts.Settings;
    using Microsoft.Extensions.Configuration;

    public class CosmosDbSettings : ICosmosDbSettings
    {
        private readonly IConfiguration _configuration;

        public CosmosDbSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString => _configuration.GetValue<string>("cosmosDbSettings:connectionString");

        public string DatabaseName => _configuration.GetValue<string>("cosmosDbSettings:databaseName");

        public FileDbContainer FileContainer =>
            new FileDbContainer(_configuration.GetValue<string>("cosmosDbSettings:containers:fileContainer:name"),
                                _configuration.GetValue<string>("cosmosDbSettings:containers:fileContainer:partitionKey"));
    }
}
