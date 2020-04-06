using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Extensions;
using Kachuwa.Identity.Extensions;
using Kachuwa.Web;
using Kachuwa.Web.Security;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class DashboardController : BaseController
    {
        private readonly IWidgetService _widgetService;
        private readonly IDashboardWidgetManager _dashboardWidgetManager;

        public DashboardController(IWidgetService widgetService, IDashboardWidgetManager dashboardWidgetManager)
        {
            _widgetService = widgetService;
            _dashboardWidgetManager = dashboardWidgetManager;
        }

        public async Task<IActionResult> Index()
        {
            var widgets = await _widgetService.GetAllWidgets();
            var roles = User.Identity.GetRoles();
            var dashboardwidgets = await _dashboardWidgetManager.GetDashboardWidgetConfigs(roles.FirstOrDefault());
            var dashboardWidgetViewModel = new List<DashboardWidgetConfigViewModel>();
            foreach (var dash_widget in dashboardwidgets)
            {
                var widget = widgets.FirstOrDefault(x => x.SystemName == dash_widget.WidgetSystemName);
                if (widget != null)
                {
                    var dashWidget = dash_widget.To<DashboardWidgetConfigViewModel>();
                    dashWidget.Widget = widget;
                    dashboardWidgetViewModel.Add(dashWidget);
                }
            }

            ViewData["Widgets"] = widgets;
            return View(dashboardWidgetViewModel);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("admin/dashboard/widget/save")]
        public async Task<JsonResult> SaveConfig(List<DashboardWidgetConfig> configs)
        {
            var getRoleName = User.Identity.GetRoles();
            var status = await _dashboardWidgetManager.SaveDashboardWidgets(getRoleName.FirstOrDefault(), configs);
            return Json(status);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("admin/dashboard/widget/reset")]
        public async Task<JsonResult> Reset()
        {
            var getRoleName = User.Identity.GetRoles();
            var status = await _dashboardWidgetManager.ResetDashboardWidgets(getRoleName.FirstOrDefault());
            return Json(status);
        }


    }

}