using Kachuwa.Data;

namespace Kachuwa.Web.Module
{
    public interface IModuleService
    {
        CrudService<ModuleInfo> Service { get; set; }
    }
}