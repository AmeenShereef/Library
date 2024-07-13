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
        // This method automatically binds classes to their implemented interfaces
        public static IServiceCollection BindClassesInAssemblyToImplementedInterfaces(this IServiceCollection services, Assembly assembly)
        {
            // Iterate through all concrete classes in the assembly
            foreach (var implementationType in assembly.DefinedTypes.Where(IsConcreteClass))
            {
                // For each interface implemented by the class
                foreach (var implementedInterface in implementationType.ImplementedInterfaces)
                {
                    // If the interface is from the same assembly
                    if (assembly == implementedInterface.Assembly)
                    {
                        // Determine the lifetime of the service and register it accordingly
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

        // Helper method to get the service lifetime from a custom attribute
        private static ServiceLifetime GetObjectLifeTime(TypeInfo implementation)
        {
            var lifeTime = implementation.GetCustomAttribute(typeof(ServiceLifeTimeAttribute)) as ServiceLifeTimeAttribute;
            return lifeTime?.LifeTime ?? ServiceLifetime.Transient;
        }

        // Lambda function to check if a type is a concrete class
        private static readonly Func<TypeInfo, bool> IsConcreteClass = t => !t.IsAbstract && !t.IsInterface;

        // This method loads entity configurations from the assembly
        public static ModelBuilder LoadConfigurationsFromAssembly(this ModelBuilder modelBuilder)
        {
            // Find all types that implement IEntityTypeConfiguration
            var implementedConfigTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(ImplementsIEntityTypeConfiguration);

            // Apply each configuration to the model builder
            foreach (Type configType in implementedConfigTypes)
            {
                dynamic? config = Activator.CreateInstance(configType);
                modelBuilder.ApplyConfiguration(config);
            }
            return modelBuilder;
        }

        // Helper method to check if a type implements IEntityTypeConfiguration
        private static bool ImplementsIEntityTypeConfiguration(Type t)
        {
            return !t.IsAbstract && !t.IsGenericTypeDefinition &&
                   t.GetTypeInfo().ImplementedInterfaces.Any(IsConfigurationInterface);
        }

        // Lambda function to check if a type is IEntityTypeConfiguration<>
        private static Func<Type, bool> IsConfigurationInterface => i => i.GetTypeInfo().IsGenericType &&
                                                                         i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>);

        // Extension method to add DbContext to the service collection
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<APIContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });
        }
    }
}