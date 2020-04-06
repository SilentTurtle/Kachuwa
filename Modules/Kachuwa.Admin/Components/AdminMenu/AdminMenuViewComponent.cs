using System;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Components
{
    public class AdminMenuViewComponent : ViewComponent
    {
        private readonly ILogger _logger;
        private readonly IMenuService _menuService;
        private readonly ISettingService _settingService;

        public AdminMenuViewComponent(ILogger logger,IMenuService menuService ,ISettingService settingService)
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
              var menus=  await _menuService.MenuCrudService.GetListAsync("Where IsActive=@IsActive and IsBackend=@IsBackend AND IsDeleted = @IsDeleted Order By MenuOrder asc",
                    new
                    {
                        IsActive = true,
                        IsBackend = true,
                        IsDeleted = false,
                        Culture= setting.BaseCulture
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