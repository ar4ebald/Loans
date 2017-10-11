using System.Security.Claims;

namespace Loans.Extensions
{
    public static class IdentityExtensions
    {
        public static int GetIdentifier(this ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                if (int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out int id))
                {
                    return id;
                }
            }

            return default;
        }
    }
}
