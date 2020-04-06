using System;
using System.Collections.Generic;
using System.Reflection;
using Kachuwa.Web.Module;

namespace Kachuwa.Web
{
    public class KachuwaGridModule : IModule
    {
        public string Name { get; set; } = "KachuwaGrid";
        public string Version { get; set; } = "1.0.0.0";
        public List<string> SupportedVersions { get; set; } = new List<String>() {"beta", "1.0.0.0" };
        public string Author { get; set; } = "Binod Tamang";
        public Assembly Assembly { get; set; } = typeof(KachuwaGridModule).GetTypeInfo().Assembly;
        public bool IsInstalled { get; set; } = true;
        public bool RequireSettingComponent { get; set; } = false;
        public string ModuleSettingComponent { get; set; }
    }
}