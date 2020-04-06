using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using Kachuwa.Configuration;
using Kachuwa.Core;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Kachuwa.Extensions;
using Kachuwa.Identity;
using Kachuwa.Identity.Extensions;
using Kachuwa.Log;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kachuwa.Web.Security
{
   
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IHostingEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IAntiforgery _xsrf;
        private readonly IConfiguration _configuration;
        private readonly KachuwaAppConfig _kachuwaConfig;

        public TokenGenerator(IHostingEnvironment environment, ILogger logger, IAntiforgery xsrf, IOptionsSnapshot<KachuwaAppConfig> optionsSnapshot, IConfiguration configuration)
        {
            _environment = environment;
            _logger = logger;
            _xsrf = xsrf;
            _configuration = configuration;
            _kachuwaConfig = optionsSnapshot.Value;
        }

        public static RsaSecurityKey CreateRsaSecurityKey(RSAParameters parameters, string id)
        {
            var key = new RsaSecurityKey(parameters)
            {
                KeyId = id
            };

            return key;
        }
        private class TemporaryRsaKey
        {
            public string KeyId { get; set; }
            public RSAParameters Parameters { get; set; }
        }
        private class RsaKeyContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                property.Ignored = false;

                return property;
            }
        }
        //public string Generate()
        //{

        //    var httpContext = ContextResolver.Context;

        //    var claims = new List<Claim>
        //    {

        //    };
        //    if (httpContext.User.IsAuthenticated())
        //    {

        //        claims.Add(new Claim("sub", httpContext.User.Identity.GetIdentityUserId().ToString()));
        //        claims.Add(new Claim("IdUid", httpContext.User.Identity.GetIdentityUserId().ToString()));
        //        claims.Add(new Claim("email", httpContext.User.Identity.GetUserName()));
        //        var roles = httpContext.User.Identity.GetRoles();
        //        foreach (var role in roles)
        //        {
        //            claims.Add(new Claim("role", role));
        //        }
        //        if (httpContext.Request.Cookies.ContainsKey(ApplicationClaim.SessionCode))
        //        {
        //            string cookie;
        //            httpContext.Request.Cookies.TryGetValue(ApplicationClaim.SessionCode, out cookie);


        //            claims.Add(new Claim(ApplicationClaim.SessionCode, cookie));
        //        }
        //    }
        //    else
        //    {
        //        claims.Add(new Claim("sub", "0"));
        //        claims.Add(new Claim("IdUid", "0"));
        //        claims.Add(new Claim("email", ""));
        //        claims.Add(new Claim("role", KachuwaRoleNames.Guest));
        //        if (httpContext.Request.Cookies.ContainsKey(ApplicationClaim.SessionCode))
        //        {
        //            string cookie;
        //            httpContext.Request.Cookies.TryGetValue(ApplicationClaim.SessionCode, out cookie);


        //            claims.Add(new Claim(ApplicationClaim.SessionCode, cookie));
        //        }
        //    }
        //    claims.Add(new Claim("plat", "web"));

        //    //var creds = new SigningCredentials(
        //    //    new RsaSecurityKey(new RSACryptoServiceProvider(2048).ExportParameters(true)),
        //    //    SecurityAlgorithms.RsaSha256);
        //    //borrowed from identity server 4 develper keys
        //    //need to apply same identity server has applied
        //    string filename = "";

        //    filename = Path.Combine(_environment.ContentRootPath, "tempkey.rsa");
        //    if (File.Exists(filename))
        //    {
        //        var keyFile = File.ReadAllText(filename);
        //        var tempKey = JsonConvert.DeserializeObject<TemporaryRsaKey>(keyFile, new JsonSerializerSettings { ContractResolver = new RsaKeyContractResolver() });

        //        var rsaKey = CreateRsaSecurityKey(tempKey.Parameters, tempKey.KeyId);

        //        if (!rsaKey.HasPrivateKey)
        //        {
        //            throw new InvalidOperationException("RSA key does not have a private key.");
        //        }

        //        var credential = new SigningCredentials(rsaKey, "RS256");
        //        var token = new JwtSecurityToken(
        //            _kachuwaConfig.SiteUrl,
        //            _kachuwaConfig.AppName,
        //            claims,
        //            DateTime.UtcNow,
        //            DateTime.UtcNow.AddSeconds(3600),
        //            credential);
        //        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        //        return jwt;
        //    }
        //    else
        //    {
        //        _logger.Log(LogType.Info, () => "Unable to generate token");
        //        return "";
        //    }



        //    //SigningCredentials credential = new SigningCredentials((SecurityKey)rsaKey, "RS256");


        //    //// amr is an array - if there is only a single value turn it into an array
        //    //if (token.Payload.ContainsKey("amr"))
        //    //{
        //    //    var amrValue = token.Payload["amr"] as string;
        //    //    if (amrValue != null)
        //    //    {
        //    //        token.Payload["amr"] = new string[] { amrValue };
        //    //    }

        //    //}

        //}

        public async Task<object> Generate()
        {
            var httpContext = ContextResolver.Context;
            if (httpContext.User.Identity.IsAuthenticated)
            {

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["KachuwaAppConfig:ApiTokenEndPoint"]);

                var keyValues = new List<KeyValuePair<string, string>>();
                keyValues.Add(new KeyValuePair<string, string>("client_id", "ro.ok.kachuwa.internal"));
                // keyValues.Add(new KeyValuePair<string, string>("client_secret", "password"));
                keyValues.Add(new KeyValuePair<string, string>("grant_type", OidcConstants.GrantTypes.Password));
                keyValues.Add(new KeyValuePair<string, string>("username", httpContext.User.Identity.GetUserName()));
                keyValues.Add(new KeyValuePair<string, string>("password", "dummy"));
                keyValues.Add(new KeyValuePair<string, string>("originrequest", "internal"));

                request.Content = new FormUrlEncodedContent(keyValues);
                var response = await client.SendAsync(request);

                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject(data);
            }
            else
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["KachuwaAppConfig:ApiTokenEndPoint"]);

                var keyValues = new List<KeyValuePair<string, string>>();
                keyValues.Add(new KeyValuePair<string, string>("client_id", "client"));
                keyValues.Add(new KeyValuePair<string, string>("client_secret", "secret"));
                keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                request.Content = new FormUrlEncodedContent(keyValues);
                var response = await client.SendAsync(request);

                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject(data);

            }

        

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