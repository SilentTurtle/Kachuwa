using Kachuwa.Identity.Extensions;
using Kachuwa.Localization;
using Kachuwa.Log;
using Kachuwa.Web.API;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Kachuwa.API.V1
{
    [Route("api/v1/menu")]
    public class MenuApiController : BaseApiController
    {
        private readonly ILogger _logger;
        private readonly ILocaleResourceProvider _locale;
        public readonly IMenuService _menuService;
        private readonly ISettingService _settingService;

        public MenuApiController(IMenuService menuService,ISettingService settingService,  ILogger logger, ILocaleResourceProvider locale)
        {
            _menuService = menuService;
            _settingService = settingService;
            _logger = logger;
            _locale = locale;
        }


        [HttpGet]
        [Route("all/role")]
        public async Task<dynamic> List()
        {
            try
            {

                var menus = await _menuService.GetAdminMenus(User.Identity.GetRoles());
                return HttpResponse(200, "", menus);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ExceptionResponse(e, "");
            }

        }
    }
}
