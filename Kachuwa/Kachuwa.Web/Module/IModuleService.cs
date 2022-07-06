using System.Threading.Tasks;
using Kachuwa.Data;

namespace Kachuwa.Web.Module
{
    public interface IModuleService
    {
        CrudService<ModuleInfo> Service { get; set; }
        Task<bool> Save(IModule module);
        Task<bool> Uninstall(string moduleName);
        Task<bool> ReInstall(string moduleName);
    }
}