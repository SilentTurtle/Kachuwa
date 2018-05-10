using System;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Data.Extension;
using Kachuwa.Form;
using Kachuwa.Localization;
using Kachuwa.Log;
using Kachuwa.Web;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Kachuwa.Web.Service;
using Kachuwa.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class LocalizationController : BaseController
    {
        private readonly ResourceBuilder _builder;
        private readonly ILocaleResourceProvider _localeResourceProvider;
        private readonly ILocaleService _localeService;
        private readonly INotificationService _notificationService;
        private readonly ICountryService _countryService;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;

        public LocalizationController(ResourceBuilder builder, ILocaleResourceProvider localeResourceProvider
        , ILocaleService localeService, INotificationService notificationService, ICountryService countryService
          , ISettingService settingService
            , ILogger logger)
        {
            _builder = builder;
            _localeResourceProvider = localeResourceProvider;
            _localeService = localeService;
            _notificationService = notificationService;
            _countryService = countryService;
            _settingService = settingService;
            _logger = logger;
        }
        [Route("admin/localization/page/{pageNo?}")]
        [Route("admin/localization")]
        public async Task<IActionResult> Index([FromRoute]int pageNo = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = pageNo;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _localeService.GetLocaleRegions(pageNo, rowsPerPage, query);
            return View(model);
        }
        [Route("admin/localization/new")]
        public async Task<IActionResult> New()
        {

            ViewData["FormDataSource"] = await LoadCountries();
            return View();
        }

        private async Task<FormDatasource> LoadCountries(int countryId = 0)
        {
            var formDataSource = new FormDatasource();
            var countries = await _countryService.CountryCrudService.GetListAsync();
            formDataSource.SetSource("Countries", countries.Select(x => new FormInputItem
            {
                IsSelected = x.CountryId == countryId,
                Id = x.CountryId,
                Label = x.NiceName,
                Value = x.CountryId.ToString()

            }));
            return formDataSource;
        }

        [HttpPost]
        [Route("admin/localization/new")]
        public async Task<IActionResult> New(LocaleRegion model)
        {
            if (ModelState.IsValid)
            {
                // model.Url = model.Url.TrimStart(new char[] { '/' });
                model.AutoFill();
                if (model.LocaleRegionId == 0)
                {
                    if (!await _localeService.CheckAlreadyExist(model.CountryId, model.Culture))
                    {
                        var countries = await _countryService.CountryCrudService.GetListAsync();
                        string flagName = countries.FirstOrDefault(x => x.CountryId == model.CountryId)?.ISO;
                        model.Flag = flagName + ".png";
                        await _localeService.RegionCrudService.InsertAsync<int>(model);
                        _notificationService.Notify(_localeResourceProvider.Get("Success"),
                            _localeResourceProvider.Get("Data has been saved successfully."), NotificationType.Success);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _notificationService.Notify(_localeResourceProvider.Get("Warning"), _localeResourceProvider.Get("Localization.AlreadyExists"),
                            NotificationType.Warning);
                        ViewData["FormDataSource"] = await LoadCountries(model.CountryId);
                        return View(model);
                    }
                }
                _notificationService.Notify(_localeResourceProvider.Get("Alert"), _localeResourceProvider.Get("Invalid inputs or missing inputs on submited form."),
                    NotificationType.Warning);
                ViewData["FormDataSource"] = await LoadCountries(model.CountryId);
                return View(model);
            }
            else
            {
                _notificationService.Notify(_localeResourceProvider.Get("Alert"), _localeResourceProvider.Get("Invalid inputs or missing inputs on submited form."),
                    NotificationType.Warning);
                ViewData["FormDataSource"] = await LoadCountries(model.CountryId);
                return View(model);
            }
        }


       
        [Route("admin/localization/edit/{localRegionId}/page/{pageNo?}")]
        [Route("admin/localization/edit/{localRegionId}")]
        public async Task<IActionResult> Edit(int localRegionId=0,int pageNo=1)
        {
            var webSetting = await _settingService.GetSetting();
            ViewData["FormDataSource"] = await LoadCountries();
            ViewBag.Page = pageNo;
            ViewBag.localRegionId = localRegionId;
            int limit = 20;
            var model = await _localeService.GetAllResourcesAsync(localRegionId, webSetting.BaseCulture, pageNo, limit);
            ViewBag.RowTotal = model?.Resources.FirstOrDefault()?.RowTotal;
            return View(model);
        }

        [HttpPost]
        [Route("admin/localization/setdefault")]
        public async Task<JsonResult> SetDefault(LocaleRegion model)
        {
            try
            {
                var webSetting = await _settingService.GetSetting();
                var status = await _localeService.SetDefaultAsync(model.LocaleRegionId);
                if (status)
                {
                    webSetting.BaseCulture = model.Culture;
                    await _settingService.SaveSetting(webSetting);
                    _notificationService.Notify(_localeResourceProvider.Get("Success"),
                        _localeResourceProvider.Get("Data has been saved successfully."), NotificationType.Success);
                    return Json(new { Code = 200, Message = "" });
                }
                _notificationService.Notify(_localeResourceProvider.Get("Error"),
                    _localeResourceProvider.Get("Localization.UnableSetDefaultLocale"), NotificationType.Error);
                return Json(new { Code = 500, Message = "Unable to set default locale region." });
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                _notificationService.Notify(_localeResourceProvider.Get("Error"),
                    _localeResourceProvider.Get("Localization.UnableSetDefaultLocale"), NotificationType.Error);
                return Json(new { Code = 500, Message = "Unable to set default locale region." });
            }
        }

        [HttpPost]
        [Route("admin/localization/update/locale")]
        public async Task<JsonResult> UpdateLocaleValue(LocaleResource model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.AutoFill();
                    await _localeService.CrudService.UpdateAsync(model);
                    _notificationService.Notify(_localeResourceProvider.Get("Success"),
                        _localeResourceProvider.Get("Data has been saved successfully."), NotificationType.Success);
                    return Json(new { Code = 200, Message = "" });
                }
                _notificationService.Notify(_localeResourceProvider.Get("Error"),
                    _localeResourceProvider.Get("Localization.ValidationError"), NotificationType.Error);
                return Json(new { Code = 600, Message = "Invalid model state." });
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                _notificationService.Notify(_localeResourceProvider.Get("Error"),
                    _localeResourceProvider.Get("Localization.UnableUpdateLocale"), NotificationType.Error);
                return Json(new { Code = 500, Message = "Unable to update locale resource." });
            }
        }

    }
}