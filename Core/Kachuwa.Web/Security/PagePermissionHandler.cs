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
using Microsoft.AspNetCore.Routing;
using Kachuwa.Extensions;

namespace Kachuwa.Web.Security
{
    public class
        PagePermissionHandler : AuthorizationHandler<PagePermissionRequirement>
    {
        private string _cachingKey = "Kachuwa.Routes";
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

            var linkGenerator = ContextResolver.Context.RequestServices.GetService<LinkGenerator>();
            //  _urlHelper = new UrlHelper(actionAccesser.ActionContext);
            var routeData = httpContext.GetRouteData();
            var ax = routeData.Values["action"] != null ? routeData.Values["action"].ToString() : "";
            var ctrl = routeData.Values["controller"] != null ? routeData.Values["controller"].ToString() : "";
            var ara = routeData.Values["area"] != null ? routeData.Values["area"].ToString() : "";
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
                    var indexRootActionUrl = linkGenerator.GetPathByAction("Index", ctrl, new { area = ara }); //_urlHelper.Action("Index", ctrl, new { area = ara });
                    if (indexRootActionUrl.StartsWith("/"))
                    {
                        indexRootActionUrl = indexRootActionUrl.TrimStart('/');
                    }
                    //removing /page on routed value that used for pagination
                    if (indexRootActionUrl.Contains("/page"))
                    {
                        indexRootActionUrl = indexRootActionUrl.ReplaceLastOccurrence(@"/page", "");
                    }
                    var pagerootPermissions = permissions.Where(p =>
                    {
                        string _url = p.Url;
                        if (_url.StartsWith("/"))
                        {
                            _url = _url.TrimStart('/');
                        }

                        return indexRootActionUrl.Contains(_url, StringComparison.InvariantCultureIgnoreCase);
                    });
                    if (userRoles.Any())
                    {
                        var userrootPagePermision = pagerootPermissions.Where(z => userRoles.Contains(z.RoleName, StringComparer.CurrentCultureIgnoreCase));
                        foreach (var p in userrootPagePermision)
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

                        if (pagerootPermissions.Where(z => z.AllowAccessForAll || z.RoleId == 0).Any())
                        {
                            context.Succeed(requirement);
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

                    return templates.Contains(_url, StringComparer.CurrentCultureIgnoreCase);
                });
                if (userRoles.Any())
                {
                    var userPagePermision = pagePermissions.Where(z => userRoles.Contains(z.RoleName, StringComparer.CurrentCultureIgnoreCase));
                    if (userPagePermision.Any(p => p.AllowAccessForAll || p.AllowAccess))
                    {
                        context.Succeed(requirement);
                    }
                }
                else
                {
                    if (pagePermissions.Where(z => z.AllowAccessForAll || z.RoleId == 0).Any())
                    {
                        context.Succeed(requirement);
                    }
                }
                //var userPagePermision = pagePermissions.Where(z => userRoles.Contains(z.RoleName, StringComparer.CurrentCultureIgnoreCase));
                //if (userPagePermision.Any(p => p.AllowAccessForAll || p.AllowAccess))
                //{
                //    context.Succeed(requirement);
                //}
                //else
                //{
                //    context.Fail();
                //}


            }
            else
            {
                //if no custom routes are used in controller 
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

                if (userRoles.Any())
                {
                    var userrootPagePermision = pagerootPermissions.Where(z => userRoles.Contains(z.RoleName, StringComparer.CurrentCultureIgnoreCase));
                    if (userrootPagePermision.Any(p => p.AllowAccessForAll || p.AllowAccess))
                    {
                        context.Succeed(requirement);
                    }
                }
                else
                {
                    if (pagerootPermissions.Where(z => z.AllowAccessForAll || z.RoleId == 0).Any())
                    {
                        context.Succeed(requirement);
                    }
                }



            }


        }
    }
}
