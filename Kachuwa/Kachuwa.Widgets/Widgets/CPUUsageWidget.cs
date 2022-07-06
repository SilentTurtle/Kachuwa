
//using System;
//using System.Collections.Generic;
//using Kachuwa.Web;

//namespace Tixalaya.Widgets
//{
//    public class CPUUsageWidget : IWidget
//    {
//        public string SystemName { get; } = "CPUUsageWidget";
//        public string Description { get; set; }
//        public string DisplayName { get; } = "CPU Usage";
//        public string Author { get; set; } = "Binod Tamang";

//        public IEnumerable<WidgetSetting> Settings { get; set; } = new List<WidgetSetting>()
//        {
//            new WidgetSetting(){Key="",Value = ""}
//        };

//        public Type WidgetViewComponent { get; set; } = typeof(CPUUsageViewComponent);
//    }
//}