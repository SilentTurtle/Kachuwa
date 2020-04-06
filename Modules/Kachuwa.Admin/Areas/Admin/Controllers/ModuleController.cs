using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Extensions;
using Kachuwa.Localization;
using Kachuwa.Web;
using Kachuwa.Web.Module;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class ModuleController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly IModuleComponentProvider _moduleComponentProvider;
        private readonly IModuleManager _moduleManager;
        private readonly ModuleContainer _moduleContainer;
        private readonly IModuleService _moduleService;
        private readonly ILocaleResourceProvider _localeResourceProvider;

        public ModuleController(INotificationService notificationService,
            IModuleComponentProvider moduleComponentProvider, IModuleManager moduleManager,
            ModuleContainer moduleContainer,
            IModuleService moduleService,ILocaleResourceProvider localeResourceProvider)
        {
            _notificationService = notificationService;
            _moduleComponentProvider = moduleComponentProvider;
            _moduleManager = moduleManager;
            _moduleContainer = moduleContainer;
            _moduleService = moduleService;
            _localeResourceProvider = localeResourceProvider;
            _localeResourceProvider.LookUpGroupAt("Module");
        }

        [Route("admin/module/page/{pageNo}")]
        [Route("admin/module")]
        public async Task<IActionResult> Index([FromRoute] int pageNo = 1)

        {
            var dbModules = await _moduleService.Service.GetListPagedAsync(pageNo, 8, 1, "", "", new { });

            ViewBag.Page = pageNo;

            if (dbModules.Any())
                ViewBag.RowTotal = dbModules.FirstOrDefault().RowTotal;
            return View(dbModules);
        }


        [Route("admin/module/install")]
        [HttpPost]
        public async Task<JsonResult> Install([FromForm][FromBody]string moduleName)

        {
            if (string.IsNullOrEmpty(moduleName))
            {
                _notificationService.Notify("Error", "Invalid module.", NotificationType.Error);
                return Json(new { Code = 500, Message = "Invalid module." });
            }
            var module = await _moduleManager.FindAsync(moduleName);
            var status = await _moduleManager.InstallAsync(module);
            if (status)
            {
                _notificationService.Notify("Success", "Module installed successfully.", NotificationType.Success);
                return Json(new { Code = 200, Message = "Module installed successfully." });
            }
            else
            {
                _notificationService.Notify("Error", "Module installation failed.", NotificationType.Error);
                return Json(new { Code = 500, Message = "Module installation failed." });
            }

        }
        [Route("admin/module/uninstall")]
        [HttpPost]
        public async Task<JsonResult> Uninstall([FromForm][FromBody]string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                _notificationService.Notify("Error", "Invalid module.", NotificationType.Error);
                return Json(new { Code = 500, Message = "Invalid module." });
            }
            var module = await _moduleManager.FindAsync(moduleName);
            var status = await _moduleManager.UnInstallAsync(module);
            if (status)
            {
                _notificationService.Notify("Success", "Module uninstalled successfully.", NotificationType.Success);
                return Json(new { Code = 200, Message = "Module uninstalled successfully." });
            }
            else
            {
                _notificationService.Notify("Error", "Module uninstallation failed.", NotificationType.Error);
                return Json(new { Code = 500, Message = "Module uninstallation failed." });
            }

        }
    }
}