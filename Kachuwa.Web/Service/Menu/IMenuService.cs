using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Service
{
    public interface IMenuService
    {
        CrudService<Menu> MenuCrudService { get; set; }

        CrudService<MenuType> TypeCrudService { get; set; }
        CrudService<MenuSetting> SettingCrudService { get; set; }
    }
}