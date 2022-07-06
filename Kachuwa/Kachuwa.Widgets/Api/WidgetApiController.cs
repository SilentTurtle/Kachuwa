using System;
using Kachuwa.Log;
using Kachuwa.Web.API;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Widgets.Api
{
    [Route("api/v1/widget")]
    public class WidgetApiController:BaseApiController
    {
        private readonly ILogger _logger;
        private readonly IDashboardWidgetService _dashboardWidgetService;
        private readonly ISettingService _settingService;


        public WidgetApiController(ILogger logger,IDashboardWidgetService dashboardWidgetService,ISettingService settingService)
        {
            _logger = logger;
            _dashboardWidgetService = dashboardWidgetService;
            _settingService = settingService;
        }
        //[Route("routename")]
        //[AllowAnonymous]
        //public async Task<dynamic> GetHourlyReport()
        //{
          
        //    return HttpResponse(200, "Succsess", new{});

        //}

    }
}
