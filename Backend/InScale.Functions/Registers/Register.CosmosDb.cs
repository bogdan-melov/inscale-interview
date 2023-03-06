namespace InScale.Functions.Registers
{
    using InScale.Contracts.Settings;
    using InScale.Functions.Settings;
    using InScale.Persistance.Common.Contracts;
    using InScale.Persistance.Common.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Configuration;

    public static partial class Register
    {

        public static IServiceCollection RegisterCosmosDb(this IServiceCollection services, IConfiguration configuration) =>
             services.RegisterDatabaseContext(configuration);

        private static IServiceCollection RegisterDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            ICosmosDbSettings settings = new CosmosDbSettings(configuration);

            services.AddSingleton<ICosmosDbContext>(CosmosDbContext.CreateAsync(settings).GetAwaiter().GetResult());
            return services;
        }
    }
}
