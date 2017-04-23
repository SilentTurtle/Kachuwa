using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Kachuwa.Plugin
{
    public class PluginConfig
    {

        public virtual string Name { get; set; }
        public virtual string Version { get; set; }
        public virtual List<string> SupportedVersions { get; set; }
        public virtual string Author { get; set; }
        public virtual bool IsInstalled { get; set; }
        public virtual List<string> ViewPermisions { get; set; }
        public virtual List<string> EditPermisions { get; set; }
        public virtual Type ClassType { get; set; }
        public virtual PluginType PluginType { get; set; }
        public virtual FileInfo OriginalAssemblyFile { get; set; }
        public virtual Assembly Assembly { get; set; }
        public string SystemName { get; set; }
    }
}