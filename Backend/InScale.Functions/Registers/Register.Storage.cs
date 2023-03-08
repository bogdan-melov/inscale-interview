namespace InScale.Functions.Registers
{
    using Azure.Storage.Blobs;
    using InScale.Contracts.Settings;
    using InScale.Contracts.Storage;
    using InScale.Functions.Settings;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static partial class Register
    {
        public static IServiceCollection RegisterStorage(this IServiceCollection services, IConfiguration configuration)
        {
            IStorageSettings settings = new StorageSettings(configuration);

            return services.AddSingleton<IStorageService>(provider =>
            {
                return new AzureStorageService(
                    client: new BlobServiceClient(
                        connectionString: settings.ConnectionString));
            });

        }
    }
}
