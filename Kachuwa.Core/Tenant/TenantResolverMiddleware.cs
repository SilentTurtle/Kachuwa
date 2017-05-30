using System.Threading.Tasks;
using Kachuwa.Log;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Tenant
{
    public class TenantResolverMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITenantResolver _resolver;
        private readonly ILogger _logger;

        public TenantResolverMiddleware(RequestDelegate next, ITenantResolver resolver, ILogProvider logProvider)
        {
            _next = next;
            _resolver = resolver;
            _logger = logProvider.GetLogger("default");
        }

        public async Task Invoke(HttpContext context)
        {

            var tenant = await _resolver.ResolveAsync(context);

            _logger.Log(LogType.Trace, () => string.Format("Resolved tenant. Current tenant: {0}", tenant.Name));

            var currentTenant = new CurrentTenant(tenant);
            context.Items[TenantConstant.TenantContextKey] = currentTenant;

            await _next(context);
        }
    }
}