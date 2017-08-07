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

        public AdminMenuViewComponent(ILogger logger,IMenuService menuService)
        {
            _logger = logger;
            _menuService = menuService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
              var menus=  await _menuService.MenuCrudService.GetListAsync("Where IsActive=@IsActive and IsBackend=@IsBackend Order By MenuOrder asc",
                    new
                    {
                        IsActive = true,
                        IsBackend = true,
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