using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web.Module
{
    public class ModuleManager : IModuleManager
    {
        private readonly IServiceCollection _services;
        private readonly ModuleContainer _moduleContainer;
        private readonly IScriptRunner _scriptRunner;

        public ModuleManager(IServiceCollection services, ModuleContainer moduleContainer, IScriptRunner scriptRunner)
        {
            _services = services;
            _moduleContainer = moduleContainer;
            _scriptRunner = scriptRunner;
        }
        public async Task<bool> InstallAsync(IModule module)
        {
            if (!module.IsInstalled)
            {
                string script = module.Assembly.GetDbInstallScript();
                var status= await _scriptRunner.Run(script);
                module.IsInstalled = status;
                return status;
            }
            return false;
        }

        public async Task<bool> UnInstallAsync(IModule module)
        {
            return true;
        }

        public async Task<IModule> FindAsync(string moduleName)
        {
            return _moduleContainer.Modules.SingleOrDefault(e => e.Name == moduleName);
        }

        public Task<bool> UpdateModule(IModule module)
        {
            var modules = _moduleContainer.Modules.ToList();
            modules.Remove(modules.First(x => x.Name == module.Name));
            modules.Add(module);
            _services.AddSingleton(new ModuleContainer(modules));
            return Task.FromResult(true);
        }
    }
}