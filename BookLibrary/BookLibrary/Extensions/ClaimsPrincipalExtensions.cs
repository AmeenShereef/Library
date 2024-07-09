using System.Data;
using System.Security.Claims;
using Microsoft.Data.SqlClient;

namespace BookLibrary.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            if (long.TryParse(user.Claims?.FirstOrDefault(c => c.Type == "id")?.Value, out long id))
            {
                return id;
            }

            return 0;
        }

        public static List<string> GetRoles(this ClaimsPrincipal user)
        {
            List<string> roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value.ToString()).ToList();
            var roles1 = user.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            return roles;
        }
    }
}
