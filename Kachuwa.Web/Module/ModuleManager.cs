using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Data.Crud.FormBuilder;
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
                var status= await _scriptRunner.Run(script);
                module.IsInstalled = status;
                if (status)
                {
                    var moduleinfo = new ModuleInfo
                    {
                        Name = module.Name,
                        IsInstalled = true,
                        Author = module.Author,
                        Version = module.Version,
                        IsActive = true,
                        Description = ""
                    };
                    moduleinfo.AutoFill();
                    await _moduleService.Service.InsertAsync<int>(moduleinfo);
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
                var status = await _scriptRunner.Run(script);
                module.IsInstalled = status;
                if (status)
                {
                    var moduleinfo = new ModuleInfo
                    {
                        Name = module.Name,
                        IsInstalled = false,
                        Author = module.Author,
                        Version = module.Version,
                        IsActive = true,
                        Description = ""
                    };
                    moduleinfo.AutoFill();
                    await _moduleService.Service.UpdateAsync(moduleinfo,"Where Name=@Name",new{Name=moduleinfo.Name});
                }
                return status;
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