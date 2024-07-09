using Microsoft.AspNetCore.Cors.Infrastructure;

namespace BookLibrary.API.Security
{
    public class AllowWhitelistCorsPolicy
    {
        public const string Name = "AllowWhitelist";

        public static CorsPolicy Get(string[] whitelist)
        {
            return new CorsPolicyBuilder()
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("*")
                .Build();
        }
    }
}
