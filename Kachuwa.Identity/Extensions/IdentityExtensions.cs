using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using IdentityModel;
using Kachuwa.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Identity.Extensions
{

    public static class IdentityExtensions
    {
        public static long GetIdentityUserId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ApplicationClaim.IdentityUserId);
            return (claim != null) ? Convert.ToInt64(claim.Value) : 0;
        }
        public static string GetUserName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(JwtClaimTypes.Name);
            return (claim != null) ? claim.Value : "";
        }
        public static IEnumerable<string> GetRoles(this IIdentity identity)
        {
            var roles = ((ClaimsIdentity)identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            return roles;
        }

        public static long GetAppUserUserId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(ApplicationClaim.AppUserId);
            return (claim != null) ? Convert.ToInt64(claim.Value) : 0;
        }

        public static bool IsAdmin(this IIdentity identity)
        {
            var roles = ((ClaimsIdentity)identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            return roles.Contains("Admin");
        }
        public static string GetClaimsValue(this IIdentity identity, string claimName)
        {
           

                IEnumerable<Claim> claims = ((ClaimsIdentity)identity).Claims;
                foreach (var claim in claims)
                {
                    if (claim.Type == claimName)
                        return claim.Value;
                }
            return "";

        }

        public static string GetClaimsValue(this IIdentity identity, Claim customClaim)
        {


            IEnumerable<Claim> claims = ((ClaimsIdentity) identity).Claims;
            foreach (var claim in claims)
            {
                if (claim.Type == customClaim.Type)
                    return claim.Value;
            }
            return "";

        }
        public static Claim FindClaim(this IIdentity identity, string claimValue)
        {
            
                IEnumerable<Claim> claims = ((ClaimsIdentity)identity).Claims;
                var claim = (from c in claims
                             where c.Value == claimValue
                             select c).Single();
                return claim;
          
        }
        public static IEnumerable<Claim> GetAllClaims(this IIdentity identity)
        {
            IEnumerable<Claim> claims = ((ClaimsIdentity)identity).Claims;
            return claims;
        }

    }
}