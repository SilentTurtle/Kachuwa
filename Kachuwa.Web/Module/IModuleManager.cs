using System.Threading.Tasks;

namespace Kachuwa.Web.Module
{
    public interface IModuleManager
    {
        Task<bool> InstallAsync(IModule module);
        Task<bool> UnInstallAsync(IModule module);
        Task<IModule> FindAsync(string moduleName);
        Task<bool> UpdateModule(IModule module);

    }
}