using System.Collections.Generic;

namespace Kachuwa.Web.Module
{
    public class ModuleContainer
    {
        public ModuleContainer(IEnumerable<IModule> modules)
        {
            Modules = modules;
        }
        public IEnumerable<IModule> Modules { get; private set; }
    }
}