using Kachuwa.Data;

namespace Kachuwa.Web.Module
{
    public class ModuleService: IModuleService
    {
        public CrudService<ModuleInfo> Service { get; set; }=new CrudService<ModuleInfo>();
    }
}