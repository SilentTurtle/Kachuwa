using System.Collections.Generic;
using System.Reflection;
using Kachuwa.Web.Module;

namespace Kachuwa.HtmlContent
{
    public class HtmlModule : IModule
    {
        public string Name { get; set; } = "HtmlModule";
        public string Version { get; set; } = "1.0.0.0";
        public List<string> SupportedVersions { get; set; }=new List<string>(){"1.0.0"};
        public string Author { get; set; } = "Binod Tamang";
        public Assembly Assembly { get; set; } = typeof(HtmlModule).GetTypeInfo().Assembly;
        public bool IsInstalled { get; set; } = false;
    }
}