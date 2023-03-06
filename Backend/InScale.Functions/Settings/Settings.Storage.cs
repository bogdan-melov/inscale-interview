namespace InScale.Functions.Settings
{
    using InScale.Contracts.Settings;
    using Microsoft.Extensions.Configuration;

    public class StorageSettings : IStorageSettings
    {
        private readonly IConfiguration _configuration;

        public StorageSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString => _configuration.GetValue<string>("azureStorage:connectionString");
    }
}
