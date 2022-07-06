using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Web.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Middleware
{
    public class WebTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        private readonly ICacheService _cache;
        private readonly ITokenGenerator _tokenGenerator;

        public WebTokenMiddleware(RequestDelegate next,
            IWebHostEnvironment environment,
            ICacheService cache,ITokenGenerator tokenGenerator)
        {
            _next = next;
            _environment = environment;
            _cache = cache;
            _tokenGenerator = tokenGenerator;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //TODO:: generate only if request is from web app
           // string token= await _tokenGenerator.Generate().ToString();

           // httpContext.Request.Headers.Add("Authorization", "Bearer " + token);

            await _next(httpContext);
        }
    }
}