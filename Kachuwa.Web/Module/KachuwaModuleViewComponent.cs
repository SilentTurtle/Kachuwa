using System;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web.Module
{
    public class KachuwaModuleViewComponent<T> : ViewComponent where T : IModule, new()
    {
        public readonly IModuleManager ModuleManager;
        public IModule Module;


        public KachuwaModuleViewComponent(IModuleManager moduleManager)
        {
            ModuleManager = moduleManager;
            T module = new T();
            Module = ModuleManager.FindAsync(module.Name).GetAwaiter().GetResult();
            checkIfInstalled();

        }

        private void checkIfInstalled()
        {
            if(!Module.IsInstalled)
                throw  new Exception("Module is not installed");
        }

    }
}