using System.Data;
using System.Security.Claims;

namespace BookLibrary.API.Extensions
{
    /// <summary>
    /// Provides extension methods for ClaimsPrincipal to easily retrieve user information.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Retrieves the user ID from the ClaimsPrincipal.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the current user.</param>
        /// <returns>The user ID as a long, or 0 if not found or invalid.</returns>
        public static long GetUserId(this ClaimsPrincipal user)
        {
            if (long.TryParse(user.Claims?.FirstOrDefault(c => c.Type == "id")?.Value, out long id))
            {
                return id;
            }
            return 0;
        }

        /// <summary>
        /// Retrieves the list of roles assigned to the user from the ClaimsPrincipal.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the current user.</param>
        /// <returns>A list of role names as strings.</returns>
        public static List<string> GetRoles(this ClaimsPrincipal user)
        {
            List<string> roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value.ToString()).ToList();
            var roles1 = user.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            return roles;
        }
    }
}