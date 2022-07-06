using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace Kachuwa.Web.Module
{
    public interface IModule
    {
        string Name { get; set; }
        string Version { get; set; }
        List<string> SupportedVersions { get; set; }
        string Author { get; set; }
        Assembly Assembly { get; set; }
        bool IsInstalled { get; set; }
        bool RequireSettingComponent { get; set; }
        string ModuleSettingComponent { get; set; } 
    }
}