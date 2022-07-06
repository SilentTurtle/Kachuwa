using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace Kachuwa.IdentityServer.Config
{
    public class Resources
    {
        public static IEnumerable<ApiScope> Apis =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My Api")
            };
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
                // some standard scopes from the OIDC spec
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                

                // custom identity resource with some consolidated claims
                new IdentityResource("custom.profile", new[] { JwtClaimTypes.Name, JwtClaimTypes.Email, "location" })
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {

                new ApiResource
                {
                    Name = "KachuwaApi",
                    ApiSecrets = { new Secret("supersecret".Sha256()) },
                    Scopes =
                    { "rd",
                       "wt",
                       "rw",
                       "api1",
                    },
                    //Scopes =
                    //{ //scopes must be uniqu try ok.api1 
                    //    new Scope("rd"),
                    //    new Scope("wt"),
                    //    new Scope("rw"),
                    //    new Scope("api1"),
                    //},
                },
                // simple version with ctor
                //new ApiResource("api1", "Some API 1")
                //{
                //    // this is needed for introspection when using reference tokens
                //    ApiSecrets = { new Secret("secret".Sha256()) }
                //},
                
                //// expanded version if more control is needed
                //new ApiResource
                //{
                //    Name = "api2",

                //    ApiSecrets =
                //    {
                //        new Secret("secret".Sha256())
                //    },

                //    UserClaims =
                //    {
                //        JwtClaimTypes.Name,
                //        JwtClaimTypes.Email
                //    },

                //    Scopes =
                //    {
                //        new Scope()
                //        {
                //            Name = "api2.full_access",
                //            DisplayName = "Full access to API 2"
                //        },
                //        new Scope
                //        {
                //            Name = "api2.read_only",
                //            DisplayName = "Read only access to API 2"
                //        }
                //    }
                //}
            };
        }
    }
}
