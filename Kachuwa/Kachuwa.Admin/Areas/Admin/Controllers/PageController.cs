﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kachuwa.Admin.ViewModel;
using Kachuwa.Data.Extension;
using Kachuwa.Localization;
using Kachuwa.Web;
using Kachuwa.Web.Layout;
using Kachuwa.Web.Module;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(PolicyConstants.PagePermission)]
    public class PageController : BaseController
    {
        private readonly IPageService _pageService;
        private readonly IModuleComponentProvider _moduleComponentProvider;
        private readonly ILayoutRenderer _layoutRenderer;
        private readonly INotificationService _notificationService;
        private readonly ILocaleResourceProvider _localeResourceProvider;

        public PageController(IPageService pageService,
            IModuleComponentProvider moduleComponentProvider,
            ILayoutRenderer layoutRenderer, INotificationService notificationService,ILocaleResourceProvider localeResourceProvider)
        {
            _pageService = pageService;
            _moduleComponentProvider = moduleComponentProvider;
            _layoutRenderer = layoutRenderer;
            _notificationService = notificationService;
            _localeResourceProvider = localeResourceProvider;
        }
        #region PAge Crud

        [Route("admin/page/page/{pageNo?}")]
        [Route("admin/page")]//default make it at last

        public async Task<IActionResult> Index([FromRoute]int pageNo = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = pageNo;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _pageService.CrudService.GetListPagedAsync(pageNo, rowsPerPage, 1,
                "Where Name like @Query and IsDeleted=@IsDeleted", "Addedon desc", new { IsDeleted=false,Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/page/new")]
        public async Task<IActionResult> New()
        {
            PageViewModel viewModel = new PageViewModel();
            viewModel.PagePermissions = await _pageService.GetPagePermission(0);
            return View(viewModel);
        }

        [HttpPost]
        [Route("admin/page/new")]
        public async Task<IActionResult> New(PageViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Url = model.Url.TrimStart(new char[] { '/' });
                if (model.Url.ToLower() == "landing")
                {

                    ModelState.TryAddModelError("Url", "Landing url is default,used by system.");
                    _notificationService.Notify("Warning", "Landing url is default,used by system.",
                        NotificationType.Warning);
                    return View(model);
                }
                if (model.PageId == 0)
                {
                    if (!await _pageService.CheckPageExist(model.Url))
                    {
                        await _pageService.Save(model);
                        _notificationService.Notify("Success", "Data has been saved successfully!", NotificationType.Success);
                    }
                    else
                    {
                        ModelState.AddModelError("", "url is already in use.");
                        _notificationService.Notify("Alert", "Url is already in use.",
                            NotificationType.Warning);
                        return View(model);
                    }

                }
                return RedirectToAction("Index");
            }
            else
            {

                model.PagePermissions = await _pageService.GetPagePermission(0);
                return View(model);
            }
        }

        [Route("admin/page/config/{pageId}")]
        public async Task<IActionResult> Config([FromRoute]int pageId)
        {
            var page = await _pageService.CrudService.GetAsync(pageId);
            if (page.IsBackend && page.IsSystem)
            {
                _notificationService.Notify("Alert", "Backend pages are not configurable.", NotificationType.Warning);
                return RedirectToAction("Index");
            }
            var model = await _pageService.CrudService.GetAsync(pageId);
            var moduleComponents = _moduleComponentProvider.GetComponents();
            var moduleList = new List<ModuleViewModel>();
            foreach (var key in moduleComponents.Keys)
            {
                var moduleViewComponents = moduleComponents[key];
                moduleList.Add(new ModuleViewModel()
                {
                    ModuleName = key,
                    ModuleComponents = moduleViewComponents
                });
            }
            ViewData["Modules"] = moduleList;
            if (string.IsNullOrEmpty(model.ContentConfig))
            {
                ViewData["Layout"] = new LayoutContent();
            }
            else
            {
                ViewData["Layout"] = (LayoutContent)JsonConvert.DeserializeObject<LayoutContent>(model.ContentConfig);
            }
            return View(model);
        }
        [HttpPost]
        [Route("admin/page/config")]
        public async Task<JsonResult> Config(LayoutContent model)
        {
            if (ModelState.IsValid)
            {
                await _pageService.SavePageLayout(model);
                _notificationService.Notify("Saved Successfully!", NotificationType.Success);
                return Json(true);
            }
            return Json(false);
        }


        [Route("admin/page/edit/{pageId}")]
        public async Task<IActionResult> Edit([FromRoute]int pageId)
        {
            var model = await _pageService.Get(pageId);
            if (model.IsBackend)
            {
                _notificationService.Notify("Alert", "Backed pages are not editable.", NotificationType.Warning);
                return RedirectToAction("Index");
            }
            model.Url = model.Url;
            return View(model);
        }

        [HttpPost]
        [Route("admin/page/edit")]
        public async Task<IActionResult> Edit(PageViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Url = model.Url.TrimStart(new char[] { '/' });
                if (model.Url.ToLower() == "landing")
                {

                    ModelState.TryAddModelError("URL", "Landing url is default,used by system.");
                    _notificationService.Notify("Warning", "Landing url is default,used by system.",
                        NotificationType.Warning);
                    return View(model);
                }
                if (model.PageId != 0)
                {
                    if (model.IsNew == false && model.OldUrl == model.Url)
                    {
                        await _pageService.Save(model);
                        _notificationService.Notify("Success", "Data has been saved successfully!", NotificationType.Success);
                    }
                    else
                    {
                        if (!await _pageService.CheckPageExist(model.Url))
                        {
                            await _pageService.Save(model);
                            _notificationService.Notify("Success", "Data has been saved successfully!", NotificationType.Success);
                        }
                        else
                        {
                            ModelState.TryAddModelError("Url", "url is already in use.");
                            _notificationService.Notify("Alert", "Url is already in use.",
                                NotificationType.Warning);
                            return View(model);
                        }

                    }

                }

                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [Route("admin/page/delete")]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var pageDetail = await _pageService.CrudService.GetAsync(id);
                if (pageDetail != null)
                {
                    if (pageDetail.IsBackend || pageDetail.IsSystem)
                    {
                        _notificationService.Notify("Warning", "Can't delete system or backend page.!", NotificationType.Warning);
                        return Json(new { code = 403, Message = "Can't delete system or backend page.", Data = false });
                    }
                    var result = await _pageService.DeletePageAsync(id);
                    _notificationService.Notify("Success", "Data deleted successfully!", NotificationType.Success);
                    return Json(new { code = 200, Message = "", Data = result });
                }
                return Json(new { code = 403, Message = "Unable to delete", Data = false });


            }
            catch (Exception e)
            {
                return Json(new { code = 200, Message = e.Message, Data = false });
            }

        }
        [HttpPost]
        [Route("admin/page/makelanding")]
        public async Task<JsonResult> MakeLandingPage(int id)
        {
            try
            {
                var result = await _pageService.MakeLandingPage(id);
                _notificationService.Notify("Success", "Updated landing page successfully!", NotificationType.Success);
                return Json(new { code = 200, Message = "", Data = result });
            }
            catch (Exception e)
            {
                return Json(new { code = 200, Message = e.Message, Data = false });
            }

        }
        [HttpGet]
        [Route("admin/page/modulesetting/{name}")]
        public async Task<IActionResult> LoadModuleSetting([FromRoute]string name)
        {
            var moduleComponents = _moduleComponentProvider.GetComponents(name);
            if (moduleComponents.FirstOrDefault().HasSetting)
                return ViewComponent(moduleComponents.FirstOrDefault().ModuleSettingComponent);
            else return Json(false);

        }
        [HttpGet]
        [Route("admin/page/pagepermission/{pageId}")]
        public async Task<IActionResult> PagePermission([FromRoute] int pageId)
        {
            PageRolePermissionViewModel model = new PageRolePermissionViewModel();
            model.PagePermissions = (await _pageService.GetPagePermission(pageId)).ToList();
            model.PageId = pageId;
            return View(model);
        }
        [HttpPost]
        [Route("admin/page/pagepermission")]
        public async Task<IActionResult> PagePermission(PageRolePermissionViewModel models)
        {
            models.AutoFill();
            if (await _pageService.AddUpdatePagePermission(models))
            {
                _notificationService.Notify("Success", "Page Permission has been successfully updated", NotificationType.Success);
                return RedirectToAction("Index");
            }
            models.PagePermissions = (await _pageService.GetPagePermission(models.PageId)).ToList();
            return View(models);
        }
        #endregion
    }

}