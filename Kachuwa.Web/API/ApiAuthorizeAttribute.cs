using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web.API
{

    /// <summary>
    /// this api authorize attribute to only post responses for invalid token 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {

        public ApiAuthorizeAttribute()
        {
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var skipAuthorization =
                context.ActionDescriptor.FilterDescriptors.FirstOrDefault(x =>
                    x.Filter.GetType() == typeof(AllowAnonymousAttribute) || x.Filter.GetType() == typeof(AllowAnonymousFilter));

            if (skipAuthorization != null)
            {
                return;
            }
            var authenticationService = context.HttpContext.RequestServices.GetService<IAuthenticationService>();
            var result = await authenticationService.AuthenticateAsync(context.HttpContext, JwtBearerDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                context.Result = new JsonResult(new { Code = 401, Message = "Unauthorized Request" });
                return;
            }


        }
    }
}