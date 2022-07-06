using System;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Data.Extension;
using Kachuwa.Identity.Extensions;
using Kachuwa.Localization;
using Kachuwa.Log;
using Kachuwa.Web;
using Kachuwa.Web.Form;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Kachuwa.Web.Service;
using Kachuwa.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(PolicyConstants.PagePermission)]
    public class LocalizationController : BaseController
    {
        private readonly ResourceBuilder _builder;
        private readonly ILocaleResourceProvider _localeResourceProvider;
        private readonly ILocaleService _localeService;
        private readonly INotificationService _notificationService;
        private readonly ICountryService _countryService;
        private readonly ISettingService _settingService;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly ILogger _logger;

        public LocalizationController(ResourceBuilder builder, ILocaleResourceProvider localeResourceProvider
        , ILocaleService localeService, INotificationService notificationService, ICountryService countryService
          , ISettingService settingService
            , ILogger logger, IExportService exportService, IImportService importService)
        {
            _builder = builder;
            _localeResourceProvider = localeResourceProvider;
            _localeService = localeService;
            _notificationService = notificationService;
            _countryService = countryService;
            _settingService = settingService;
            _exportService = exportService;
            _importService = importService;
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
        [Route("admin/localization/import")]
        [HttpPost]
        public async Task<IActionResult> Index(LocaleResourcesImportViewModel model)
        {
            if (model.ImportFile == null)
            {
                _notificationService.Notify(_localeResourceProvider.Get("Warning"),
                    _localeResourceProvider.Get("Localization.UploadFile"), NotificationType.Warning);
                return RedirectToAction("Index");
            }
            var importedDatas = _importService.Import<LocaleResourcesImportModel>(model.ImportFile);
            _notificationService.Notify(_localeResourceProvider.Get("Info"),
                _localeResourceProvider.Get("Importing might take a time,you will be notified once is completed or error"), NotificationType.Info);
            var status = await _localeService.ImportLocaleResources(importedDatas, User.Identity.GetUserName());
            if (status.IsImported)
            {
                _notificationService.Notify(_localeResourceProvider.Get("Success"),
                    _localeResourceProvider.Get("Data has been imported successfully."), NotificationType.Success);

            }
            else
            {
                _notificationService.Notify(_localeResourceProvider.Get("Error"),
                    _localeResourceProvider.Get(status.Error), NotificationType.Error);

            }
            ViewData["Page"] = 1;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var modelx = await _localeService.GetLocaleRegions(1, rowsPerPage, "");
            return View(modelx);

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
                        await _localeService.AddNewResourceFromBaseCultureAsync(model.Culture);
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
        public async Task<IActionResult> Edit(int localRegionId = 0, int pageNo = 1)
        {
            var webSetting = await _settingService.GetSetting();

            ViewBag.Page = pageNo;
            ViewBag.localRegionId = localRegionId;
            int limit = 20;
            var region = await _localeService.RegionCrudService.GetAsync(localRegionId);
            ViewData["FormDataSource"] = await LoadCountries(region.CountryId);
            var model = await _localeService.GetAllResourcesAsync(localRegionId, webSetting.BaseCulture, pageNo, limit);
       
            ViewBag.RowTotal = model?.Resources.Any()==true?model?.Resources.FirstOrDefault()?.RowTotal:0;
            return View(model);
        }


        [Route("admin/localization/edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(LocaleRegionEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // model.Url = model.Url.TrimStart(new char[] { '/' });
                model.AutoFill();
                if (model.LocaleRegionId != 0)
                {

                    var countries = await _countryService.CountryCrudService.GetListAsync();
                    string flagName = countries.FirstOrDefault(x => x.CountryId == model.CountryId)?.ISO;
                    model.Flag = flagName + ".png";
                    await _localeService.RegionCrudService.UpdateAsync(model);
                    // await _localeService.AddNewResourceFromBaseCultureAsync(model.Culture);
                    _notificationService.Notify(_localeResourceProvider.Get("Success"),
                        _localeResourceProvider.Get("Data has been saved successfully."), NotificationType.Success);
                    if (!await _localeService.CheckAlreadyExist(model.CountryId, model.Culture))
                    {
                        await _localeService.AddNewResourceFromBaseCultureAsync(model.Culture);
                     
                    }
                    return RedirectToAction("Index");
                    //}
                    //else
                    //{
                    //    _notificationService.Notify(_localeResourceProvider.Get("Warning"), _localeResourceProvider.Get("Localization.AlreadyExists"),
                    //        NotificationType.Warning);
                    //    ViewData["FormDataSource"] = await LoadCountries(model.CountryId);
                    //    var webSetting = await _settingService.GetSetting();

                    //    ViewBag.Page = 1;
                    //    ViewBag.localRegionId = model.LocaleRegionId;
                    //    int limit = 20;
                    //    var region = await _localeService.RegionCrudService.GetAsync(model.LocaleRegionId);
                    //    ViewData["FormDataSource"] = await LoadCountries(region.CountryId);
                    //    var modelx = await _localeService.GetAllResourcesAsync(model.LocaleRegionId, webSetting.BaseCulture, 1, limit);
                    //    ViewBag.RowTotal = modelx?.Resources.FirstOrDefault()?.RowTotal;
                    //    return View(modelx);
                    //}
                }
                _notificationService.Notify(_localeResourceProvider.Get("Alert"), _localeResourceProvider.Get("Invalid inputs or missing inputs on submited form."),
                    NotificationType.Warning);
                ViewData["FormDataSource"] = await LoadCountries(model.CountryId);
                var webSetting2 = await _settingService.GetSetting();

                ViewBag.Page = 1;
                ViewBag.localRegionId = model.LocaleRegionId;
                int limit2 = 20;
                var region2 = await _localeService.RegionCrudService.GetAsync(model.LocaleRegionId);
                ViewData["FormDataSource"] = await LoadCountries(region2.CountryId);
                var modelx2 = await _localeService.GetAllResourcesAsync(model.LocaleRegionId, webSetting2.BaseCulture, 1, limit2);
                ViewBag.RowTotal = modelx2?.Resources.FirstOrDefault()?.RowTotal;
                return View(modelx2);
            }
            else
            {
                _notificationService.Notify(_localeResourceProvider.Get("Alert"), _localeResourceProvider.Get("Invalid inputs or missing inputs on submited form."),
                    NotificationType.Warning);
                ViewData["FormDataSource"] = await LoadCountries(model.CountryId);
                var webSetting = await _settingService.GetSetting();

                ViewBag.Page = 1;
                ViewBag.localRegionId = model.LocaleRegionId;
                int limit = 20;
                var region = await _localeService.RegionCrudService.GetAsync(model.LocaleRegionId);
                ViewData["FormDataSource"] = await LoadCountries(region.CountryId);
                var modelx = await _localeService.GetAllResourcesAsync(model.LocaleRegionId, webSetting.BaseCulture, 1, limit);
                ViewBag.RowTotal = modelx?.Resources.FirstOrDefault()?.RowTotal;
                return View(modelx);
            }
        }



        [Route("admin/localization/export/{localRegionId}")]
        public async Task<IActionResult> Export(int localRegionId = 0)
        {
            if (localRegionId == 0)
            {
                return Redirect("/page-not-found");
            }
            var webSetting = await _settingService.GetSetting();

            var model = await _localeService.GetAllResourcesForExportAsync(localRegionId, webSetting.BaseCulture);
            if (model.Any())
            {
                string fileName = $"{model.FirstOrDefault().Culture}-locales.xlsx";
                var responseContent =
                    await _exportService.Export<LocaleResourcesExportModel>(model.ToList(), fileName, "Sheet 1");
                return File(responseContent.Content.ReadAsByteArrayAsync().Result,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else
            {
                string fileName = $"no-data{webSetting.BaseCulture}-locales.xlsx";
                var responseContent =
                    await _exportService.Export<LocaleResourcesExportModel>(model.ToList(), fileName, "Sheet 1");
                return File(responseContent.Content.ReadAsByteArrayAsync().Result,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
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

        [HttpPost]
        [Route("language/set")]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        [Route("admin/localization/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var region = await _localeService.RegionCrudService.GetAsync(id);
            await _localeService.CrudService.DeleteAsync("Where Culture=@Culture", new { region.Culture });
            await _localeService.RegionCrudService.UpdateAsDeleted(id);
            _notificationService.Notify(_localeResourceProvider.Get("Success"),
                _localeResourceProvider.Get("Data has been deleted successfully."), NotificationType.Success);
            return Json(true);
        }

    }
}