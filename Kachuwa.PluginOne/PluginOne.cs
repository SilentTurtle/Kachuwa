using System;
using System.Collections.Generic;
using System.Composition;
using System.Reflection;
using System.Text;
using Kachuwa.Plugin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Semantics;

namespace Kachuwa.PluginOne
{
    [Export(typeof(IPlugin))]
    public class PluginOne: IPlugin
    {
        public string SystemName { get; } = "PluginOne";
        public bool Install()
        {
            return true;
        }

        public bool UnInstall()
        {
            return true;
        }

        public PluginConfig Configuration { get; set; }=new PluginConfig()
        {
            Author = "Binod Tamang",
            EditPermisions =new List<string>() { "admin"},
            Name = "Plugin One",
            SystemName = "PluginOne",
            Assembly =typeof(PluginOne).GetTypeInfo().Assembly,
            PluginType = PluginType.Normal,
            Version = "1.0.0.1",
            ViewPermisions = new List<string>() { "*"}
            
            
        };
    }
    public class PluginOneViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int number)
        {
            return View(number + 1);
        }
    }
    public class SimpleViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(int number)
        {
            return View(number + 1);
        }

        public IViewComponentResult Test()
        {
            return View("Default2", 5);
        }

    }
}
