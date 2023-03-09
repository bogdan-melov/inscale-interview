namespace InScale.Functions.Registers
{
    using InScale.Commands.Common.Commands;
    using InScale.Queries.Common.Queries;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class Register
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CommandsAssemblyPlaceholder>());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<QueriesAssemblyPlaceholder>());

            return services;
        }
    }
}
