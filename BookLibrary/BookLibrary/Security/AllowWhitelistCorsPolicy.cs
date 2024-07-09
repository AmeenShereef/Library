using Microsoft.AspNetCore.Cors.Infrastructure;

namespace BookLibrary.API.Security
{
    /// <summary>
    /// Defines a CORS policy that allows requests from whitelisted origins.
    /// </summary>
    public class AllowWhitelistCorsPolicy
    {
        /// <summary>
        /// The name of the CORS policy.
        /// </summary>
        public const string Name = "AllowWhitelist";

        /// <summary>
        /// Creates a CORS policy that allows any origin, method, and header.
        /// </summary>
        /// <param name="whitelist">An array of whitelisted origins (not currently used).</param>
        /// <returns>A CorsPolicy object representing the CORS policy.</returns>
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