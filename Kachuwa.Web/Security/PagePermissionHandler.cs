﻿using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Kachuwa.Caching;
using Kachuwa.Identity.Extensions;

namespace Kachuwa.Web.Security
{
    public class PagePermissionHandler : AuthorizationHandler<PagePermissionRequirement>
    {
        private string _cachingKey = "Kachuwa.Routes";
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PagePermissionRequirement requirement)
        {
            var httpContext = ContextResolver.Context;
            var menuService = httpContext.RequestServices.GetService<IMenuService>();
            var actionProvider = httpContext.RequestServices.GetService<IActionDescriptorCollectionProvider>();
            var permissions = await menuService.GetPermissionsFromCache();
            var cacheService = httpContext.RequestServices.GetService<ICacheService>();
            var routes = await cacheService.GetAsync<IEnumerable<RouteCollectionViewModel>>(_cachingKey,async() =>
             {
                 return actionProvider.ActionDescriptors.Items.Where(z => z.AttributeRouteInfo != null).Select(y => new
             RouteCollectionViewModel
                 {
                     Action = y.RouteValues["Action"],
                     Controller = y.RouteValues["Controller"],
                     Area = y.RouteValues["Area"]==null?"":y.RouteValues["Area"],
                     Name = y.AttributeRouteInfo.Name,
                     Template = y.AttributeRouteInfo.Template

                 }).ToList();
             });

            var actionAccesser = ContextResolver.Context.RequestServices.GetService<IActionContextAccessor>();
            var ax = actionAccesser.ActionContext.ActionDescriptor.RouteValues["action"];
            var ctrl = actionAccesser.ActionContext.ActionDescriptor.RouteValues["controller"];
            var ara = actionAccesser.ActionContext.ActionDescriptor.RouteValues["area"];
            //route collection will have only route attribute used
            var currentActionDetails = routes.Where(c => c.Action.ToLower() == ax.ToLower() && c.Controller.ToLower() == ctrl.ToLower() && c.Area.ToLower() == ara.ToLower());
            if (currentActionDetails.Any())
            {   
                var templates = currentActionDetails.Select(x => x.Template).ToArray();
                var userRoles = context.User.Identity.GetRoles();
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
                foreach (var p in userPagePermision)
                {
                    if (p.AllowAccessForAll || p.AllowAccess)
                    {
                        context.Succeed(requirement);
                        break;
                    }
                }


            }
            else
            {
                //no custom routes inside controllers
                string url;
                if (ax.ToLower() == "index") {
                    url = $"/{ara}/{ctrl}";
                }
                else
                {
                    url = $"/{ara}/{ctrl}/{ax}";
                }
                var userRoles = context.User.Identity.GetRoles();
                var pagePermissions = permissions.Where(p => p.Url.ToLower()== url.ToLower());

                var userPagePermision = pagePermissions.Where(z => userRoles.Contains(z.RoleName));
                foreach (var p in userPagePermision)
                {
                    if (p.AllowAccessForAll || p.AllowAccess)
                    {
                        context.Succeed(requirement);
                        break;
                    }
                }

            }
          
        }
    }
}