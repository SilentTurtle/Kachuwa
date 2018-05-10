using System;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;

namespace Kachuwa.Web.Module
{
    public class ModuleService: IModuleService
    {
        public CrudService<ModuleInfo> Service { get; set; } = new CrudService<ModuleInfo>();
        public async Task<bool> Save(IModule module)
        {
            try
            {
                var existingmodule = await Service.GetAsync("Where Name=@Name", new { module.Name });
                if (existingmodule == null)
                {
                    var moduleInfo = new ModuleInfo
                    {
                        Name = module.Name,
                        Author = module.Author,
                        Description = "",
                        AddedBy = "System",
                        IsInstalled = module.IsInstalled,
                        IsBuiltIn = module.IsInstalled,
                        Version = module.Version,
                        AddedOn = DateTime.Now,
                        IsActive = true


                    };
                    await Service.InsertAsync<int>(moduleInfo);
                    return true;
                }
                else
                {
                    if (!existingmodule.IsInstalled)
                    {
                        return await this.ReInstall(module.Name);
                    }
                }

                return false;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<bool> Uninstall(string moduleName)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection) dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Update dbo.Module set IsInstalled=@IsInstalled where Name=@Name",
                    new {Name = moduleName, IsInstalled = false});
                return true;
            }
        }
        public async Task<bool> ReInstall(string moduleName)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Update dbo.Module set IsInstalled=@IsInstalled where Name=@Name",
                    new { Name = moduleName, IsInstalled = true });
                return true;
            }
        }
    }
}