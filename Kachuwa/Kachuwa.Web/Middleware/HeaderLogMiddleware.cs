using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Log;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Kachuwa.Web.Middleware
{
    public class HeaderLogMiddleware
    {
        const string MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        

        readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public HeaderLogMiddleware(RequestDelegate next, ILogger logger)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var sw = Stopwatch.StartNew();
            try
            {
                await _next(httpContext);
                sw.Stop();

                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 499 ? LogType.Error : LogType.Info;

                //var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : Log;
                var msg = $"HTTP {httpContext.Request.Method} { httpContext.Request.Path} responded {statusCode} in {sw.Elapsed.TotalMilliseconds:0.0000} ms";
                var dict = httpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                _logger.Log(level, () => JsonConvert.SerializeObject(dict));
            }
            // Never caught, because `LogException()` returns false.
            catch (Exception ex) when (LogException(httpContext, sw, ex)) { }
        }

        bool LogException(HttpContext httpContext, Stopwatch sw, Exception ex)
        {
            sw.Stop();
            var statusCode = httpContext.Response?.StatusCode;
            var level = statusCode > 499 ? LogType.Error : LogType.Info;
            var msg = $"HTTP {httpContext.Request.Method} { httpContext.Request.Path} responded {statusCode} in {sw.Elapsed.TotalMilliseconds:0.0000} ms";
            _logger.Log(level, () => msg, ex);
            //LogForErrorContext(httpContext)
            //    .Error(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, sw.Elapsed.TotalMilliseconds);

            return false;
        }

        // ILogger LogForErrorContext(HttpContext httpContext)
        //{
        //    var request = httpContext.Request;

        //    var result = logger.ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
        //        .ForContext("RequestHost", request.Host)
        //        .ForContext("RequestProtocol", request.Protocol);

        //    if (request.HasFormContentType)
        //        result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

        //    return result;
        //}
    }
}