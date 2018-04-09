using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Security
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SecurityHeadersPolicy _policy;
        private readonly ICspNonceService _cspNonceService;

        public SecurityHeadersMiddleware(RequestDelegate next, SecurityHeadersPolicy policy, ICspNonceService cspNonceService)
        {
            _next = next;
            _policy = policy;
            _cspNonceService = cspNonceService;
        }

        public async Task Invoke(HttpContext context)
        {
            IHeaderDictionary headers = context.Response.Headers;

            if (_policy.AddNonce)
            {
                context.Response.Headers["Content-Security-Policy"] = _policy.CspBuiilder.Build(_cspNonceService);
            }
            else
            {
                context.Response.Headers["Content-Security-Policy"] = _policy.CspBuiilder.Build();
            }
            //context.Response.Headers["Content-Security-Policy"] = "default-src *;" +
            //                                                "script-src 'self' 'http://localhost:11258' 'nonce-" + _cspNonceService.GetNonce() + "';" +
            //                                                "object-src 'self';" +
            //                                                "style-src 'self' 'unsafe-inline' 'unsafe-eval';" +
            //                                                "img-src 'self';" +
            //                                                // "data:assets-cdn.github.com identicons.github.com www.google-analytics.com ...;" +
            //                                                "media-src 'none';" +
            //                                                "child-src 'self';" +
            //                                                //"render.githubusercontent.com ;" +
            //                                                // "font-src assets-cdn.github.com;" +
            //                                                "connect-src 'self';" +
            //                                                "base-uri 'self';" +
            //                                                "form-action 'self';";
            foreach (var headerValuePair in _policy.SetHeaders)
            {
                headers[headerValuePair.Key] = headerValuePair.Value;
            }

            foreach (var header in _policy.RemoveHeaders)
            {
                headers.Remove(header);
            }

            await _next(context);
        }
    }
}