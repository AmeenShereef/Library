using BookLibrary.Data.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibrary.Business.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessRules(this IServiceCollection services)
        {
            return services
                .BindClassesInAssemblyToImplementedInterfaces(typeof(ServiceCollectionExtensions).Assembly);
        }
    }
}
