namespace InScale.Functions.Registers
{
    using InScale.Commands.Common.Commands;
    using InScale.Commands.InScaleFile.Commands;
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    public static partial class Register
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services)
        {

            return services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CommandsAssemblyPlaceholder>());
        }
    }
}
