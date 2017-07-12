using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Kachuwa.Identity.Extensions
{
    public static class IdentityExtensions
    {
        public static long GetIdentityUserId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IdUid");
            // Test for null to avoid issues during local testing
            return (claim != null) ? Convert.ToInt64(claim.Value) : 0;
        }
    }
}