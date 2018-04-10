using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Kachuwa.Web.Security
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IHostingEnvironment _environment;
        private readonly IAntiforgery _xsrf;

        public TokenGenerator(IHostingEnvironment environment, IAntiforgery xsrf)
        {
            _environment = environment;
            _xsrf = xsrf;
        }

        public string Generate()
        {
            var httpContext = ContextResolver.Context;
            string refferrerUrl = httpContext.Request.Headers["Referer"].ToString();
            string domain = "";
            if (httpContext.Request.Scheme == "https")
            {
                domain = httpContext.Request.Host.Port == 443
                    ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Host}"
                    : $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Host}:{httpContext.Request.Host.Port}";
            }
            else
            {
                domain = httpContext.Request.Host.Port == 80
                    ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Host}"
                    : $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Host}:{httpContext.Request.Host.Port}";
            }

            var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "damienbodserver.pfx"), "");

            var credential = new SigningCredentials(new X509SecurityKey(cert), "RS256");


            var jwt = new JwtSecurityToken(
                domain,
                WebConstants.WebAudience,
                new List<Claim>
                {
                    new Claim("role", "dataEventRecords.admin"),
                    new Claim("role", "dataEventRecords.user"),
                    new Claim("scope", "dataEventRecords"),
                    new Claim("scope", "dataeventrecordsscope"),
                    new Claim("email", "guest@kf.com")
                },
                DateTime.UtcNow,
                DateTime.UtcNow.AddSeconds(3600),
                credential);

            // amr is an array - if there is only a single value turn it into an array
            if (jwt.Payload.ContainsKey("amr"))
            {
                var amrValue = jwt.Payload["amr"] as string;
                if (amrValue != null)
                {
                    jwt.Payload["amr"] = new string[] { amrValue };
                }

            }
            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(jwt);

            //httpContext.Request.Headers.Add("Authorization", "Bearer " + genToken);

        }

        public string RequestAntiforgeryToken()
        {
            return _xsrf.GetAndStoreTokens(ContextResolver.Context).RequestToken;
        }
        public async Task<bool> ValidateAntiforgeryToken(HttpContext context)
        {
            await _xsrf.ValidateRequestAsync(context);
            return true;
        }
    }
}