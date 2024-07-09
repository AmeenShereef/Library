using BookLibrary.Data;
using BookLibrary.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace BookLibrary.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection BindClassesInAssemblyToImplementedInterfaces(this IServiceCollection services, Assembly assembly)
        {
            foreach (var implementationType in assembly.DefinedTypes.Where(IsConcreteClass))
            {
                foreach (var implementedInterface in implementationType.ImplementedInterfaces)
                {
                    if (assembly == implementedInterface.Assembly)
                    {
                        switch (GetObjectLifeTime(implementationType))
                        {
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(implementedInterface, implementationType);
                                break;

                            case ServiceLifetime.Scoped:
                                services.AddScoped(implementedInterface, implementationType);
                                break;

                            case ServiceLifetime.Transient:
                                services.AddTransient(implementedInterface, implementationType);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            return services;
        }

        private static ServiceLifetime GetObjectLifeTime(TypeInfo implementation)
        {
            var lifeTime = implementation.GetCustomAttribute(typeof(ServiceLifeTimeAttribute)) as ServiceLifeTimeAttribute;
            if (lifeTime == null)
                return ServiceLifetime.Transient;
            else
                return lifeTime.LifeTime;
        }

        private static readonly Func<TypeInfo, bool> IsConcreteClass = _ => !_.IsAbstract && !_.IsInterface;

        public static ModelBuilder LoadConfigurationsFromAssembly(this ModelBuilder modelBuilder)
        {
            var implementedConfigTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(ImplementsIEntityTypeConfiguration);

            foreach (Type configType in implementedConfigTypes)
            {
                dynamic? config = Activator.CreateInstance(configType);
                modelBuilder.ApplyConfiguration(config);
            }

            return modelBuilder;
        }

        private static bool ImplementsIEntityTypeConfiguration(Type t)
        {
            return !t.IsAbstract && !t.IsGenericTypeDefinition &&
                   t.GetTypeInfo().ImplementedInterfaces.Any(IsConfigurationInterface);
        }

        private static Func<Type, bool> IsConfigurationInterface => i => i.GetTypeInfo().IsGenericType &&
                                                                         i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>);

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        { 
            return services.AddDbContext<APIContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });
        }

    }
}
