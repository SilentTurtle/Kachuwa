using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web
{
    public class ContextResolver
    {
        public ContextResolver(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        private static IHttpContextAccessor _contextAccessor;
        public static HttpContext CurrentContext { get { return _contextAccessor.HttpContext; } }
    }
}