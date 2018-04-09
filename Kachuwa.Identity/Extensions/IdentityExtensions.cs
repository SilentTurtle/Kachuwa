using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Kachuwa.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public static IEnumerable<string> GetRoles(this IIdentity identity)
        {
            var roles = ((ClaimsIdentity)identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);
            return roles;
        }

        public static long GetAppUserUserId(this IIdentity identity)
        {
            //TODO: AppUser ID
            var claim = ((ClaimsIdentity)identity).FindFirst("IdUid");//((ClaimsIdentity)identity).FindFirst("appuserid");
            // Test for null to avoid issues during local testing
            return (claim != null) ? Convert.ToInt64(claim.Value) : 0;
        }
        public static string GetUserPlatform(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("plat");
            // Test for null to avoid issues during local testing
            if ((claim == null))
            {
                var clientPlat = ((ClaimsIdentity)identity).FindFirst("client_plat");
                return (clientPlat != null) ? clientPlat.Value : "Web";
            }
            else
            {
                return claim.Value;
            }
          
        }
        

    }

    public static class UserHelper
    {
        

        public static string UserName
        {
            get
            {
                string name = GetClaimsValue(ClaimTypes.Name);
                if (string.IsNullOrEmpty(name))
                    return "guestuser";
                return name;
            }
        }

        public static bool IsAdmin =>  ContextResolver.Context.User.IsInRole("Admin");

        public static string GetClaimsValue(string key)
        {
            try
            {
               var _context = ContextResolver.Context;
                if (_context.User.Identity.IsAuthenticated)
                {
                    var identity = (ClaimsIdentity)_context.User.Identity;
                    IEnumerable<Claim> claims = identity.Claims;
                    foreach (var claim in claims)
                    {
                        if (claim.Type == key)
                            return claim.Value;
                    }

                }
                return "";
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static string GetClaimsValue(Claim customClaim)
        {
            try
            {
                var _context = ContextResolver.Context;
                if (_context.User.Identity.IsAuthenticated)
                {
                    var identity = (ClaimsIdentity)_context.User.Identity;
                    IEnumerable<Claim> claims = identity.Claims;
                    foreach (var claim in claims)
                    {
                        if (claim.Type == customClaim.Type)
                            return claim.Value;
                    }

                }
                return "";
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public static IEnumerable<Claim> GetClaims()
        {
            try
            {
                var _context = ContextResolver.Context;
                if (_context.User.Identity.IsAuthenticated)
                {
                    var identity = (ClaimsIdentity)_context.User.Identity;
                    return identity.Claims;

                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public static string GetSessionId()
        {
            try
            {
                var _context = ContextResolver.Context;
                if (_context.User.Identity.IsAuthenticated)
                {
                    var identity = (ClaimsIdentity)_context.User.Identity;
                    IEnumerable<Claim> claims = identity.Claims;
                    foreach (var claim in claims)
                    {
                        if (claim.Type == ApplicationClaim.SessionCode)
                            return claim.Value;
                    }

                }
                else
                {
                    //var cookie = _context.Request.Cookies.Get(ApplicationClaim.Anonymous);
                    //if (cookie != null)
                    //{
                    //    return cookie.Value;

                    //}
                    //else
                    //{
                    //    var userId = Guid.NewGuid().ToString();
                    //    HttpCookie gcookie = new HttpCookie(ApplicationClaim.Anonymous);
                    //    gcookie.HttpOnly = true;
                    //    gcookie.Expires = DateTime.Now.AddMinutes(30);
                    //    gcookie.Value = userId;
                    //    _context.Response.Cookies.Add(gcookie);
                    //    return userId;
                    //}
                }
                return "";
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private static Claim FindClaim(string value)
        {
            try
            {
                var _context = ContextResolver.Context;
                var user = _context.User as ClaimsPrincipal;
                var claim = (from c in user.Claims
                             where c.Value == value
                             select c).Single();
                return claim;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        public static void AddRole(string role)
        {
            if (FindClaim(role) != null)
                return;
            var _context = ContextResolver.Context;
            var user = _context.User as ClaimsPrincipal;
            if (user == null) return;

            var claimId = new ClaimsIdentity();
            claimId.AddClaim(new Claim(ClaimTypes.Role, role));
            user.AddIdentity(claimId);
        }

        public static void AddClaim(string claimtype, string value)
        {
            var _context = ContextResolver.Context;
            var user = _context.User as ClaimsPrincipal;
            if (user == null) return;
            var claimId = new ClaimsIdentity();
            claimId.AddClaim(new Claim(claimtype, value));
            user.AddIdentity(claimId);
        }

        public static void RemoveClaim(string value)
        {
            var _context = ContextResolver.Context;
            var user = _context.User as ClaimsPrincipal;
            var identity = user.Identity as ClaimsIdentity;
            var claim = (from c in user.Claims
                         where c.Value == value
                         select c).Single();
            identity.RemoveClaim(claim);
        }

        public static int GetCustomerId()
        {
            try
            {
                var _context = ContextResolver.Context;
                if (_context.User.Identity.IsAuthenticated)
                {
                    var identity = (ClaimsIdentity)_context.User.Identity;
                    IEnumerable<Claim> claims = identity.Claims;
                    foreach (var claim in claims)
                    {
                        if (claim.Type == ApplicationClaim.CustomerId)
                            return Int32.Parse(claim.Value);
                    }

                }
                return 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
    public class ApplicationClaim
    {
        public static readonly string CustomerId = "_customerId";
        public static readonly string SessionCode = "_sessionCode";
        public static readonly string Anonymous = "__anonymous";
        public static readonly string OnlineId = "_onlineId";
    }
}