using System.Threading.Tasks;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Middleware
{
    public class ForceHttpsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISettingService _settingService;

        public ForceHttpsMiddleware(RequestDelegate next,ISettingService settingService)
        {
            _next = next;
            _settingService = settingService;
        }

        public async Task Invoke(HttpContext context)
        {
            var setting =await _settingService.GetSetting();
            HttpRequest req = context.Request;
            if (req.IsHttps == false)
            {
                if (setting.UseHttps)
                {
                    string url = "https://" + req.Host + req.Path + req.QueryString;
                    context.Response.Redirect(url, permanent: true);
                }
              
            }
            else
            {
                await _next(context);
            }
        }
    }
}