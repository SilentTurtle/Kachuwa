using System.Threading.Tasks;
using Kachuwa.Configuration;
using Kachuwa.Localization;
using Kachuwa.Web;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class ThemeController : BaseController
    {
        private readonly IThemeManager _themeManager;
        private readonly INotificationService _notificationService;
        private readonly ILocaleResourceProvider _localeResourceProvider;
        private readonly IKachuwaConfigurationManager _kachuwaConfigurationManager;
        private readonly KachuwaAppConfig _kachuwaAppConfig;

        public ThemeController(IThemeManager themeManager, INotificationService notificationService, ILocaleResourceProvider localeResourceProvider,
            IOptionsSnapshot<KachuwaAppConfig> optionsSnapshot)
        {
            _themeManager = themeManager;
            _notificationService = notificationService;
            _localeResourceProvider = localeResourceProvider;

            _localeResourceProvider.LookUpGroupAt("Themes");
            _kachuwaAppConfig = optionsSnapshot.Value;
        }
        [Route("admin/theme/manage/page/{pageNo}")]
        [Route("admin/theme/manage")]
        [Route("admin/theme")]
        public async Task<IActionResult> Index([FromQuery]string query = "", [FromRoute]int pageNo = 1, int limit = 10)
        {
            ViewData["Page"] = pageNo;
            ViewData["Config"] = _kachuwaAppConfig;
            var themes = await _themeManager.GetThemes(query, pageNo - 1, limit);

            return View(themes);
        }
        [HttpPost]
        [Route("admin/theme/change")]
        public async Task<JsonResult> ChangeDefaultTheme(ThemeInfo theme)
        {
            var status = await _themeManager.SetDefault(theme);
            _notificationService.Notify(_localeResourceProvider.Get("Success"),
                _localeResourceProvider.Get("Theme.ChangedSuccessfully"), NotificationType.Success);
            return Json(new { Code = 200, Data = status });
        }

    
        public async Task<ActionResult> New()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> New(ThemeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ThemeZip != null)
                {
                    var status = await _themeManager.UnzipAndInstall(model.ThemeZip);
                    if (status.IsInstalled)
                    {
                        _notificationService.Notify(_localeResourceProvider.Get("Success"),
                            _localeResourceProvider.Get("Theme.InstallMessage"), NotificationType.Success);
                        _notificationService.Notify(_localeResourceProvider.Get("Info"),
                            _localeResourceProvider.Get("Theme.ConfiguringMessage"), NotificationType.Info);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _notificationService.Notify(_localeResourceProvider.Get("Error"),
                            status.Error, NotificationType.Error);
                    }
                }
                else
                {
                    _notificationService.Notify(_localeResourceProvider.Get("Alert"),
                        _localeResourceProvider.Get("Theme.UploadZipFileFirst"),
                        NotificationType.Warning);
                    ModelState.AddModelError("Theme", "Please upload file first.");
                }
                return View();
            }

            return View();
        }

    }
}