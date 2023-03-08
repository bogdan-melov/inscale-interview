[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup(typeof(InScale.Functions.Startup))]
namespace InScale.Functions
{
    using InScale.Commands.InScaleFile.Commands;
    using InScale.Contracts.InScaleFile.Repositories;
    using InScale.Contracts.Settings;
    using InScale.Functions.Registers;
    using InScale.Functions.Settings;
    using InScale.Persistance.InScaleFile.Repository;
    using MediatR;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.IO;
    using System.Reflection;

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.Services.RegisterCosmosDb(context.Configuration)
                            .AddSingleton<IStorageSettings, StorageSettings>()
                            .AddScoped<IInScaleFileRepository, InScaleFileRepository>()
                            .RegisterStorage(context.Configuration)
                            .AddCQRS();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);

            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddEnvironmentVariables()
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: false, reloadOnChange: true);
        }
    }
}
