using System;
using System.Collections.Generic;
using System.Text;
using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Service
{

   public class MenuService:IMenuService
    {
        public CrudService<Menu> MenuCrudService { get; set; }=new CrudService<Menu>();
        public CrudService<MenuType> TypeCrudService { get; set; }=new CrudService<MenuType>();
        public CrudService<MenuSetting> SettingCrudService { get; set; }=new CrudService<MenuSetting>();
    }
}
