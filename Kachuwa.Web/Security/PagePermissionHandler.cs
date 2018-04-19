using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Kachuwa.Caching;
using Kachuwa.Identity.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Kachuwa.Web.Security
{
    public class PagePermissionHandler : AuthorizationHandler<PagePermissionRequirement>
    {
        private string _cachingKey = "Kachuwa.Routes";
        private IUrlHelper _urlHelper;
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PagePermissionRequirement requirement)
        {
            var httpContext = ContextResolver.Context;
            var pageService = httpContext.RequestServices.GetService<IPageService>();
            var actionProvider = httpContext.RequestServices.GetService<IActionDescriptorCollectionProvider>();
            var permissions = await pageService.GetPermissionsFromCache();
            var cacheService = httpContext.RequestServices.GetService<ICacheService>();
            var routes = await cacheService.GetAsync<IEnumerable<RouteCollectionViewModel>>(_cachingKey, async () =>
              {
                  return actionProvider.ActionDescriptors.Items.Where(z => z.AttributeRouteInfo != null).Select(y => new
              RouteCollectionViewModel
                  {
                      Action = y.RouteValues["Action"],
                      Controller = y.RouteValues["Controller"],
                      Area = y.RouteValues["Area"] == null ? "" : y.RouteValues["Area"],
                      Name = y.AttributeRouteInfo.Name,
                      Template = y.AttributeRouteInfo.Template

                  }).ToList();
              });

            var actionAccesser = ContextResolver.Context.RequestServices.GetService<IActionContextAccessor>();
            _urlHelper = new UrlHelper(actionAccesser.ActionContext);
            var ax = actionAccesser.ActionContext.ActionDescriptor.RouteValues["action"];
            var ctrl = actionAccesser.ActionContext.ActionDescriptor.RouteValues["controller"];
            var ara = actionAccesser.ActionContext.ActionDescriptor.RouteValues["area"];
            //route collection will have only route attribute used
            var currentActionDetails = routes.Where(c => c.Action.ToLower() == ax.ToLower() && c.Controller.ToLower() == ctrl.ToLower() && c.Area.ToLower() == ara.ToLower());
            if (currentActionDetails.Any())
            {
                var templates = currentActionDetails.Select(x => x.Template).ToArray();
                var userRoles = context.User.Identity.GetRoles();
                //checking controller other actions which are sub activities of a page
                //ie if index has permission then allowing other actions as well
                if (ax.ToLower() != "index")
                {
                    var indexRootActionUrl = _urlHelper.Action("Index", ctrl, new { area = ara });
                    if (indexRootActionUrl.StartsWith("/"))
                    {
                        indexRootActionUrl = indexRootActionUrl.TrimStart('/');
                    }
                    var pagerootPermissions = permissions.Where(p =>
                    {
                        string _url = p.Url;
                        if (_url.StartsWith("/"))
                        {
                            _url = _url.TrimStart('/');
                        }

                        return indexRootActionUrl.Contains(_url);
                    });

                    var userrootPagePermision = pagerootPermissions.Where(z => userRoles.Contains(z.RoleName));
                    foreach (var p in userrootPagePermision)
                    {
                        if (p.AllowAccessForAll || p.AllowAccess)
                        {
                            context.Succeed(requirement);
                            break;
                        }
                    }

                }

                var pagePermissions = permissions.Where(p =>
                {
                    string _url = p.Url;
                    if (_url.StartsWith("/"))
                    {
                        _url = _url.TrimStart('/');
                    }

                    return templates.Contains(_url);
                });

                var userPagePermision = pagePermissions.Where(z => userRoles.Contains(z.RoleName));
                if (userPagePermision.Any(p => p.AllowAccessForAll || p.AllowAccess))
                {
                    context.Succeed(requirement);
                }


            }
            else
            {//if no custom routes are used in controller 
                //ie:checking default or index route only
                var userRoles = context.User.Identity.GetRoles();
                //no custom routes inside controllers

                var indexRootActionUrl = $"{ara}/{ctrl}";//_urlHelper.Action("Index", ctrl, new { area = ara });
                if (indexRootActionUrl.StartsWith("/"))
                {
                    indexRootActionUrl = indexRootActionUrl.TrimStart('/');
                }

                var pagerootPermissions = permissions.Where(p =>
                    p.Url.ToLower() == indexRootActionUrl.ToLower());

                var userrootPagePermision = pagerootPermissions.Where(z => userRoles.Contains(z.RoleName));
                if (userrootPagePermision.Any(p => p.AllowAccessForAll || p.AllowAccess))
                {
                    context.Succeed(requirement);
                }


            }

        }
    }
}
