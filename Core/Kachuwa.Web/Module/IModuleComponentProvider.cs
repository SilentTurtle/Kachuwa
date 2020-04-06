using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Kachuwa.Web.Module
{
    public interface IModuleComponentProvider
    {
        Dictionary<string, List<ModuleComponentDescription>> GetComponents();
        IEnumerable<ModuleComponentDescription> GetComponents(string moduleName);

    }
}