using BookLibrary.Data.Extensions;
using Microsoft.Extensions.Configuration;

namespace BookLibrary.Repositories.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddRepositories(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddDbContext(configuration)
                .BindClassesInAssemblyToImplementedInterfaces(typeof(ServiceCollectionExtensions).Assembly);
        }
    }
}
