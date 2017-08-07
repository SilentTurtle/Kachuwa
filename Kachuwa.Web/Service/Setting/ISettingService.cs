using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Service
{
    public interface ISettingService
    {
        CrudService<Setting> CrudService { get; set; }
    }
}