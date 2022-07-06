using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Kachuwa.Data.Extension;
using Kachuwa.Localization;
using Kachuwa.Log;
using Kachuwa.Storage;
using Kachuwa.Web;
using Kachuwa.Web.API;
using Kachuwa.Web.Form;
using Kachuwa.Web.Model;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(PolicyConstants.PagePermission)]
    public class SEOController : BaseController
    {
        private readonly ISeoService _seoService;
        private readonly INotificationService _notificationService;
        private readonly ILocaleResourceProvider _localeResourceProvider;
        private readonly ILogger _logger;
        private readonly IStorageProvider _storageProvider;

        public SEOController(ISeoService seoService,
            INotificationService notificationService, ILocaleResourceProvider localeResourceProvider, ILogger logger,IStorageProvider storageProvider)
        {
            _seoService = seoService;
            _notificationService = notificationService;
            _localeResourceProvider = localeResourceProvider;
            _logger = logger;
            _storageProvider = storageProvider;
            _localeResourceProvider.LookUpGroupAt("seo");
        }

        [Route("admin/seo/page/{pageNo?}")]
        [Route("admin/seo/page")]
        [Route("admin/seo")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int pageNo = 1, [FromQuery]string query = "")
        {
            try
            {
                ViewData["Page"] = pageNo;
                int rowsPerPage = 10;
                var model = await _seoService.Seo.GetListPagedAsync(pageNo, rowsPerPage, 1, "Where IsDeleted=0",
                    "AddedOn desc", new { });
                return View(model);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task LoadFormDataSource(string type = "page")
        {
            var fd = new FormDatasource();
            fd.SetSource("SEOTypes",new List<FormInputItem>()
            {
                new FormInputItem
                {
                    Id = 0,
                    Label = "Page",
                    Value = "page",
                    IsSelected = type=="page"
                },
                new FormInputItem
                {
                    Id = 0,
                    Label = "Product",
                    Value = "product",
                    IsSelected = type=="product"
                }
                ,
                new FormInputItem
                {
                    Id = 0,
                    Label = "Category",
                    Value = "category",
                    IsSelected = type=="category"
                }
            });
            ViewData["FormDataSources"] = fd;
        }
        [Route("admin/seo/new")]
        public async Task<IActionResult> New()
        {
            try
            {
               await  LoadFormDataSource();
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("admin/seo/edit/{seoId}")]
        public async Task<IActionResult> Edit([FromRoute]int seoId)
        {
            var model = await _seoService.Seo.GetAsync(seoId);
            await LoadFormDataSource(model.SeoType);
            return View(model);
        }

        [HttpPost]
        [Route("admin/seo/new")]
        public async Task<IActionResult> New(SEO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.AutoFill();
                    if (model.SEOId == 0)
                    {
                        if (model.ImageFile != null)
                        {
                            model.Image = await _storageProvider.Save("SEO", model.ImageFile);
                        }
                        await _seoService.Seo.InsertAsync<int>(model);
                        _notificationService.Notify(_localeResourceProvider.Get("Success"),
                            _localeResourceProvider.Get("Data has been saved successfully."), NotificationType.Success);
                        return RedirectToAction("Index");
                    }
                    _notificationService.Notify(_localeResourceProvider.Get("Alert"), _localeResourceProvider.Get("Invalid inputs or missing inputs on submited form."),
                        NotificationType.Warning);
                    return View(model);
                }
                else
                {
                    _notificationService.Notify(_localeResourceProvider.Get("Alert"), _localeResourceProvider.Get("Invalid inputs or missing inputs on submited form."),
                        NotificationType.Warning);
                    await LoadFormDataSource(model.SeoType);
                    return View(model);
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                await LoadFormDataSource(model.SeoType);
                return View(model);
            }
        }
        [HttpPost]
        [Route("admin/seo/edit")]
        public async Task<IActionResult> Edit(SEO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.AutoFill();
                    if (model.SEOId != 0)
                    {
                        if (model.ImageFile != null)
                        {
                            model.Image = await _storageProvider.Save("SEO", model.ImageFile);
                        }
                        await _seoService.Seo.UpdateAsync(model);
                        _notificationService.Notify(_localeResourceProvider.Get("Success"),
                            _localeResourceProvider.Get("Data has been saved successfully."), NotificationType.Success);
                        return RedirectToAction("Index");
                    }

                    _notificationService.Notify(_localeResourceProvider.Get("Alert"),
                        _localeResourceProvider.Get("Invalid inputs or missing inputs on submited form."),
                        NotificationType.Warning);
                    await LoadFormDataSource(model.SeoType);
                    return View(model);
                }
                else
                {
                    _notificationService.Notify(_localeResourceProvider.Get("Alert"),
                        _localeResourceProvider.Get("Invalid inputs or missing inputs on submited form."),
                        NotificationType.Warning);
                    await LoadFormDataSource(model.SeoType);
                    return View(model);
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                await LoadFormDataSource(model.SeoType);
                return View(model);
            }
        }

        [HttpPost]
        [Route("admin/seo/delete")]
        public async Task<JsonResult> Delete(int id)
        {
            await _seoService.Seo.UpdateAsDeleted(id);
            _notificationService.Notify(_localeResourceProvider.Get("Success"), _localeResourceProvider.Get("Data deleted successfully!"), NotificationType.Success);
            return Json(true);
        }

        [HttpPost]
        [Route("admin/seo/updatestatus")]
        public async Task<ApiResponse> Updateseo(int id, bool status)
        {
            try
            {
                if (id < 1)
                {
                    return new ApiResponse { Code = (int)ApiResponseCodes.Codes.ModelValidationError, Message = _localeResourceProvider.Get("seo.Invalidseo") };
                }
                var result = await _seoService.Seo.UpdateStatusAsync(id, status);
                _notificationService.Notify(_localeResourceProvider.Get("Success"),
                    _localeResourceProvider.Get("Data has been saved successfully."), NotificationType.Success);
                return new ApiResponse { Code = (int)HttpStatusCode.OK, Message = "Successfully Updated.", Data = result };
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return new ApiResponse { Code = (int)HttpStatusCode.InternalServerError, Message = "Internal Error" };
            }
        }
    }
}