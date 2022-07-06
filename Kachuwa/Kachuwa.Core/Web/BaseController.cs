using System;
using System.Text;
using Kachuwa.Configuration;
using Kachuwa.Core;
using Kachuwa.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Kachuwa.Web
{
    [LogError]
    public class BaseController : Controller
    {
        public readonly ModelService ModelService = new ModelService();
        public readonly KachuwaAppConfig KachuwaAppConfig;
        public BaseController()
        {
            var kachuwaconfig = ContextResolver.Context.RequestServices.GetService<IOptionsSnapshot<KachuwaAppConfig>>();
            KachuwaAppConfig = kachuwaconfig.Value;

        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var _context = HttpContext;
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                if (!HttpContext.Request.Cookies.ContainsKey(ApplicationClaim.SessionCode))
                {

                    var guid = Guid.NewGuid().ToString();

                    CookieOptions option = new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.Now.AddMinutes(20)
                    };


                    _context.Response.Cookies.Append(ApplicationClaim.SessionCode, guid, option);
                }
            }
            base.OnActionExecuting(filterContext);

            if (!KachuwaAppConfig.IsInstalled)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Installer",
                    action = "Index"
                }));
            }
        }
        public RedirectResult RedirectToAnother(string url)
        {
            return base.Redirect(url);
        }
        public int GetCompanyId()

        {
            var claim = ((ClaimsIdentity)User.Identity).FindFirst("CompanyId");
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }
        public int GetBranchId()
        {
            var claim = ((ClaimsIdentity)User.Identity).FindFirst("BranchId");
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }
    }





}
