using System;
using System.Collections.Generic;
using Kachuwa.Web;

namespace Kachuwa.Widgets
{
    public class WeatherWidget : IWidget
    {
        public string SystemName { get; } = "WeatherWidget";
        public string Description { get; set; }
        public string DisplayName { get; } = "Weather";
        public string Author { get; set; } = "Binod Tamang";

        public IEnumerable<WidgetSetting> Settings { get; set; } = new List<WidgetSetting>()
        {
            new WidgetSetting(){Key="",Value = ""}
        };

        public Type WidgetViewComponent { get; set; } = typeof(WeatherViewComponent);
    }
}