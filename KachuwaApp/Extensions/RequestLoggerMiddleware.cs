using Microsoft.AspNetCore.Http;

using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace Host.Logging
{

    public static class ServiceExtensions
    {
        public static IServiceCollection AddExternalIdentityProviders(this IServiceCollection services)
        {
            // configures the OpenIdConnect handlers to persist the state parameter into the server-side IDistributedCache.
            // services.AddOidcStateDataFormatterCache("aad", "demoidsrv");

            services.AddAuthentication()
                .AddOpenIdConnect("Google", "Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.Authority = "https://accounts.google.com/";
                    options.ClientId = "369145466632-i3pie9n8ubormq5v5d372jrj9kbgapgv.apps.googleusercontent.com";
                    options.ClientSecret = "Lsd0w6JCU-LgMKu-EsE9jcLp";
                    options.SaveTokens = true;
                    options.Events = new OpenIdConnectEvents
                    {

                    };
                    //Lsd0w6JCU-LgMKu-EsE9jcLp 

                    options.CallbackPath = "/signin-google";
                    options.Scope.Add("email");
                });
                //.AddFacebook(facebookOptions =>
                //{
                //    facebookOptions.AppId = "2304357913112946";
                //    facebookOptions.AppSecret = "7d429126ddf6c704a98ef30a07441bf0";
                //    facebookOptions.CallbackPath= "/signin-facebook";
                //    facebookOptions.Scope.Add("email");
                //    facebookOptions.SaveTokens = true;
                //    facebookOptions.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                   
                //});
            //.AddOpenIdConnect("demoidsrv", "IdentityServer", options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

            //    options.Authority = "https://demo.identityserver.io/";
            //    options.ClientId = "implicit";
            //    options.ResponseType = "id_token";
            //    options.SaveTokens = true;
            //    options.CallbackPath = "/signin-idsrv";
            //    options.SignedOutCallbackPath = "/signout-callback-idsrv";
            //    options.RemoteSignOutPath = "/signout-idsrv";

            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        NameClaimType = "name",
            //        RoleClaimType = "role"
            //    };
            //})
            //.AddOpenIdConnect("aad", "Azure AD", options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

            //    options.Authority = "https://login.windows.net/4ca9cb4c-5e5f-4be9-b700-c532992a3705";
            //    options.ClientId = "96e3c53e-01cb-4244-b658-a42164cb67a9";
            //    options.ResponseType = "id_token";
            //    options.CallbackPath = "/signin-aad";
            //    options.SignedOutCallbackPath = "/signout-callback-aad";
            //    options.RemoteSignOutPath = "/signout-aad";
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        NameClaimType = "name",
            //        RoleClaimType = "role"
            //    };
            //})
            //.AddOpenIdConnect("adfs", "ADFS", options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

            //    options.Authority = "https://adfs.leastprivilege.vm/adfs";
            //    options.ClientId = "c0ea8d99-f1e7-43b0-a100-7dee3f2e5c3c";
            //    options.ResponseType = "id_token";

            //    options.CallbackPath = "/signin-adfs";
            //    options.SignedOutCallbackPath = "/signout-callback-adfs";
            //    options.RemoteSignOutPath = "/signout-adfs";
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        NameClaimType = "name",
            //        RoleClaimType = "role"
            //    };
            //})
            //.AddWsFederation("adfs-wsfed", "ADFS with WS-Fed", options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

            //    options.MetadataAddress = "https://adfs4.local/federationmetadata/2007-06/federationmetadata.xml";
            //    options.Wtrealm = "urn:test";

            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        NameClaimType = "name",
            //        RoleClaimType = "role"
            //    };
            //});

            return services;
        }
    }
    //internal class RequestLoggerMiddleware
    //{
    //    const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    //    static readonly ILogger Log = Serilog.Log.ForContext<RequestLoggerMiddleware>();

    //    readonly RequestDelegate _next;

    //    public RequestLoggerMiddleware(RequestDelegate next)
    //    {
    //        _next = next ?? throw new ArgumentNullException(nameof(next));
    //    }

    //    public async Task Invoke(HttpContext httpContext)
    //    {
    //        if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

    //        var start = Stopwatch.GetTimestamp();
    //        try
    //        {
    //            await _next(httpContext);
    //            var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

    //            var statusCode = httpContext.Response?.StatusCode;
    //            var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

    //            var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : Log;
    //            log.Write(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, statusCode, elapsedMs);
    //        }
    //        // Never caught, because `LogException()` returns false.
    //        catch (Exception ex) when (LogException(httpContext, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex)) { }
    //    }

    //    static bool LogException(HttpContext httpContext, double elapsedMs, Exception ex)
    //    {
    //        LogForErrorContext(httpContext)
    //            .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, elapsedMs);

    //        return false;
    //    }

    //    static ILogger LogForErrorContext(HttpContext httpContext)
    //    {
    //        var request = httpContext.Request;

    //        var result = Log
    //            .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
    //            .ForContext("RequestHost", request.Host)
    //            .ForContext("RequestProtocol", request.Protocol);

    //        if (request.HasFormContentType)
    //            result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

    //        return result;
    //    }

    //    static double GetElapsedMilliseconds(long start, long stop)
    //    {
    //        return (stop - start) * 1000 / (double)Stopwatch.Frequency;
    //    }
    //}
}