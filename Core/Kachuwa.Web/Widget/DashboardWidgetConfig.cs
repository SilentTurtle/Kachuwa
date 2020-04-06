using System.Collections;
using System.Collections.Generic;

namespace Kachuwa.Web
{
    public class DashboardWidgetConfig
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string WidgetSystemName { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<WidgetSetting> Settings { get; set; }

    }
    public class DashboardWidgetConfigViewModel
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string WidgetSystemName { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<WidgetSetting> Settings { get; set; }
        public IWidget Widget { get; set; }

    }
}