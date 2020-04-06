using System;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;

namespace KachuwaApp.Components
{

    [ViewComponent(Name = "Navigation")]
    public class NavigationViewComponent : KachuwaViewComponent
    {
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IMenuService _menuService;

        public NavigationViewComponent(ILogger logger,ISettingService settingService, IMenuService  menuService)
        {
            _logger = logger;
            _settingService = settingService;
            _menuService = menuService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var setting = await _settingService.CrudService.GetAsync(1);
                var menus = await _menuService.MenuCrudService.GetListAsync("Where MenuGroupId=@MenuGroupId and IsActive=@IsActive and IsBackend=@IsBackend Order By MenuOrder asc",
                    new
                    {
                        MenuGroupId = 2,
                        IsActive = true,
                        IsBackend = false,
                        Culture = setting.BaseCulture
                    });
                return View(menus);

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => "Navigation ViewComponent error.", e);
                throw e;
            }

        }

        public override string DisplayName { get; } = "Navigation";
        public override bool IsVisibleOnUI { get; } = true;
    }
}
