using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;

namespace KachuwaApp.Views.Shared.Components.MenuIndex
{
    public class MenuIndexViewComponent:ViewComponent
    {
        private readonly ILogger _logger;
        private readonly IMenuService _menuService;
        private readonly ISettingService _settingService;

        public MenuIndexViewComponent(ILogger logger, IMenuService menuService, ISettingService settingService)
        {
            _logger = logger;
            _menuService = menuService;
            _settingService = settingService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var setting = await _settingService.CrudService.GetAsync(1);
                var menus = await _menuService.MenuCrudService.GetListAsync("Where IsActive=@IsActive and IsBackend=@IsBackend Order By MenuOrder asc",
                      new
                      {
                          IsActive = true,
                          IsBackend = false,
                          Culture = setting.BaseCulture
                      });
                return View(menus);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => "Admin menu loading error.", e);
                throw e;
            }

        }
    }
}
