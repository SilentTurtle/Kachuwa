using System;
using System.Diagnostics;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Kachuwa.Identity.Extensions;
using Kachuwa.Web.Model;
using Kachuwa.Web.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Kachuwa.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AuditAttribute : Attribute, IAsyncActionFilter
    {


        public AuditAttribute()
        {
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                //To do : before the action executes
                var key = "";
                if (context.ActionDescriptor.AttributeRouteInfo == null)
                {
                    key = $"{context.ActionDescriptor.RouteValues["controller"]}/{context.ActionDescriptor.RouteValues["action"]}";
                }
                else
                {
                    key = context.ActionDescriptor.AttributeRouteInfo.Template;
                }
                context.HttpContext.Items.Add(key, Stopwatch.StartNew());
                await next();
                var stopWatch = (Stopwatch)context.HttpContext.Items[key];
                stopWatch.Stop();
                var elapsedtime = $"{stopWatch.Elapsed}";
                var httpContext = context.HttpContext;
                var auditService = httpContext.RequestServices.GetService<IAuditService>();
                var audit = new Audit
                {
                    UserName = httpContext.User.Identity.GetUserName()
                    ,
                    Action = key,
                    Duration = (int)stopWatch.Elapsed.TotalMilliseconds,
                    IpAddress = httpContext.Connection.RemoteIpAddress.ToString(),
                    RequestObject = JsonConvert.SerializeObject(context.ActionArguments),
                    Url = key,
                    Role = string.Join(',', httpContext.User.Identity.GetRoles()),
                    UserAgent = httpContext.Request.Headers["User-Agent"].ToString()
                };
                await auditService.CrudService.InsertAsync(audit);
            }
            catch (Exception e)
            {
              
                throw e;
            }
        }
    }
}