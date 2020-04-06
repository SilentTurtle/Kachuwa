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
        private readonly IModuleService _moduleService;

        public ModuleManager(IServiceCollection services, ModuleContainer moduleContainer, IScriptRunner scriptRunner,IModuleService moduleService)
        {
            _services = services;
            _moduleContainer = moduleContainer;
            _scriptRunner = scriptRunner;
            _moduleService = moduleService;
        }
        public async Task<bool> InstallAsync(IModule module)
        {
            if (!module.IsInstalled)
            {
                string script = module.Assembly.GetDbInstallScript();
                var status= await _scriptRunner.Run(new []{script});
                module.IsInstalled = status;
                if (status)
                {
                    await _moduleService.Save(module);
                }
                return status;
            }
            return false;
        }

        public async Task<bool> UnInstallAsync(IModule module)
        {
            if (module.IsInstalled)
            {
                string script = module.Assembly.GetDbUnInstallScript();
                var status = await _scriptRunner.Run(new[] { script });
                module.IsInstalled = false;
                if (status)
                {
                   return await _moduleService.Uninstall(module.Name);
                }
                return false;
            }
            return false;
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