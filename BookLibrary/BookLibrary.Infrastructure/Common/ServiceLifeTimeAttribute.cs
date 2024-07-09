using Microsoft.Extensions.DependencyInjection;

namespace BookLibrary.Infrastructure.Common
{
    public class ServiceLifeTimeAttribute : Attribute
    {
        public ServiceLifetime LifeTime { get; set; }
        public ServiceLifeTimeAttribute(ServiceLifetime lifeTime)
        {
            LifeTime = lifeTime;
        }
    }
}
