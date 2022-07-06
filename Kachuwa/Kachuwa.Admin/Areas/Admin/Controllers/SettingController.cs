using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Web;
using Kachuwa.Web.Model;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Kachuwa.Core.DI;
using Kachuwa.Data.Extension;
using Kachuwa.Extensions;
using Kachuwa.Web.Form;
using Kachuwa.Localization;
using Kachuwa.Storage;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Kachuwa.Web.Services;
using Microsoft.AspNetCore.Hosting;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(PolicyConstants.PagePermission)]
    public class SettingController : BaseController
    {
        private readonly ISettingService _settingService;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly INotificationService _notificationService;
        private readonly ILocaleResourceProvider _localeResourceProvider;
        private readonly ICacheService _cacheService;
        private readonly IEnumerable<ICacheService> _cacheServices;
        private readonly IStorageProvider _storageProvider;

        public SettingController(ISettingService settingService, IApplicationLifetime applicationLifetime,
            IStorageProvider storageProvider,
            IServiceCollection diService,
            INotificationService notificationService
            , ILocaleResourceProvider localeResourceProvider,ICacheService cacheService,IEnumerable<ICacheService> cacheServices )
        {
            _settingService = settingService;
            _applicationLifetime = applicationLifetime;
            _notificationService = notificationService;
            _localeResourceProvider = localeResourceProvider;
            _cacheService = cacheService;
            _cacheServices = cacheServices;
            _storageProvider = storageProvider;
            _localeResourceProvider.LookUpGroupAt("Setting");
        }
        [Route("admin/setting")]
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Web");
        }
        public async Task<IActionResult> Security()
        {
            return View();
        }
        
        public async Task<string> Shutdown()
        {
            // Later bro
            _applicationLifetime.StopApplication();
            return "Ok";
        }

        [Route("admin/setting/web")]
        public async Task<IActionResult> Web()
        {
            var _setting = await _settingService.CrudService.GetAsync(1);
            ViewData["FormDataSource"] = await GetFormDataSources(_setting.TimeZoneName);
            return View(_setting);
        }

        [HttpPost]
        [Route("admin/setting/web")]
        public async Task<IActionResult> Web(Setting model)
        {
           
            if (ModelState.IsValid)
            {
                model.AutoFill();
                model.Description.Trim();
                if (model.LogoFile != null)
                {
                    model.Logo = await _storageProvider.Save("Logo", model.LogoFile);
                }
                try
                {
                   var selectedTimezone= TimeZoneInfo.GetSystemTimeZones()
                        .SingleOrDefault(x => x.StandardName == model.TimeZoneName);
                  
                   TimeSpan ts = selectedTimezone.BaseUtcOffset;
                   if (ts.Hours > 0)
                       model.TimeZoneOffset = "+" + ts.ToString(@"hh\:mm");
                   else
                   {
                       model.TimeZoneOffset = "-" + ts.ToString(@"hh\:mm");
                   }
                }
                catch (Exception e)
                {

                }
                await _settingService.SaveSetting(model);
                _notificationService.Notify(_localeResourceProvider.Get("Success"),
                    _localeResourceProvider.Get("Data has been saved successfully."), NotificationType.Success);
                ViewData["FormDataSource"] = await GetFormDataSources(model.TimeZoneName);
                return View(model);
            }
            else
            {
               
                _notificationService.Notify(_localeResourceProvider.Get("Alert"), _localeResourceProvider.Get("Invalid inputs or missing inputs on submited form."),
                    NotificationType.Warning);
                ModelState.AddModelError("Invalid Setting Values", "Please enter valid values");
                return View(model);
            }

           
        }
        public async Task<IActionResult> Caching()
        {
           
            return View(_cacheServices);
        }
        public async Task<IActionResult> ResetCacheAll()
        {
            _cacheService.Flush();
            return RedirectToAction("Caching");
        }

        private async Task<FormDatasource> GetFormDataSources(string name)
        {
            var formDataSource = new FormDatasource();
            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            formDataSource.SetSource("TimeZoneNames", timeZones.Select(x => new FormInputItem()
            {
                IsSelected = x.StandardName == name,
                //Id = x.BaseUtcOffset,
                Value = x.StandardName.ToString(),
                Label = x.DisplayName
            }));
            return formDataSource;
        }
    }
}