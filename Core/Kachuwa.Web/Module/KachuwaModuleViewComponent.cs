using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kachuwa.Web.Module
{
   
    public abstract class KachuwaModuleViewComponent<T> : KachuwaViewComponent where T : IModule, new()
    {
        public readonly IModuleManager ModuleManager;
        public IModule Module;
        protected KachuwaModuleViewComponent(IModuleManager moduleManager)
        {
            ModuleManager = moduleManager;
            Module = new T();
            Module = ModuleManager.FindAsync(Module.Name).GetAwaiter().GetResult();
            checkIfInstalled();

        }

        private void checkIfInstalled()
        {
            if(!Module.IsInstalled)
                throw  new Exception("Module is not installed");
        }

    }
}