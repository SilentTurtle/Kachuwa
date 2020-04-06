using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Widgets
{
    
    public class WeatherViewComponent : KachuwaWidgetViewComponent<WeatherWidget>
    {
        private readonly ILogger _logger;
        private readonly IDashboardWidgetService _dashboardWidgetService;

        public WeatherViewComponent(ILogger logger,IDashboardWidgetService dashboardWidgetService)
        {
            _logger = logger;
            _dashboardWidgetService = dashboardWidgetService;
        }
        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<WidgetSetting> settings=null)
        {
            try
            {
                return View() ;
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => "Weather View Component loading error.", e);
                throw e;
            }

        }

        public override string DisplayName { get; } = "Weather";
    }
}
