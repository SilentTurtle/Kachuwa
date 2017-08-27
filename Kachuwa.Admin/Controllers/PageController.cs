using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kachuwa.Admin.ViewModel;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Web;
using Kachuwa.Web.Layout;
using Kachuwa.Web.Module;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PageController : BaseController
    {
        private readonly IPageService _pageService;
        private readonly IModuleComponentProvider _moduleComponentProvider;
        private readonly ILayoutRenderer _layoutRenderer;
        private readonly INotificationService _notificationService;

        public PageController(IPageService pageService,
            IModuleComponentProvider moduleComponentProvider, ILayoutRenderer layoutRenderer,INotificationService notificationService)
        {
            _pageService = pageService;
            _moduleComponentProvider = moduleComponentProvider;
            _layoutRenderer = layoutRenderer;
            _notificationService = notificationService;
        }
        #region PAge Crud
        [Route("admin/page/page/{page?}")]
        [Route("admin/page")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int page = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = page;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _pageService.CrudService.GetListPagedAsync(page, rowsPerPage, 1,
                "Where Name like @Query and IsDeleted=0", "Addedon desc", new { Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/page/new")]
        public async Task<IActionResult> New()
        {

            return View();
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

                    ModelState.TryAddModelError("System Url", "unable to use this url.enter another url.");
                    return View(model);
                }
                if (model.PageId == 0)
                {
                    if (!await _pageService.CheckPageExist(model.Url))
                    {
                        await _pageService.Save(model);
                        _notificationService.Notify("Saved Successfully!", NotificationType.Success);
                    }
                    else
                    {
                        ModelState.TryAddModelError("Existing Url", "url is already in use.");
                        return View(model);
                    }

                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [Route("admin/page/config/{pageId}")]
        public async Task<IActionResult> Config([FromRoute]int pageId)
        {
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
                ViewData["Layout"] = (LayoutContent) JsonConvert.DeserializeObject<LayoutContent>(model.ContentConfig);
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

                    ModelState.TryAddModelError("System Url", "unable to use this url.enter another url.");
                    return View(model);
                }
                if (model.PageId != 0)
                {
                    if (model.IsNew == false && model.OldUrl == model.Url)
                    {
                        await _pageService.Save(model);
                        _notificationService.Notify("Saved Successfully!", NotificationType.Success);
                    }
                    else
                    {
                        if (!await _pageService.CheckPageExist(model.Url))
                        {
                            await _pageService.Save(model);
                            _notificationService.Notify("Saved Successfully!", NotificationType.Success);
                        }
                        else
                        {
                            ModelState.TryAddModelError("Existing Url", "url is already in use.");
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
                var result = await _pageService.DeletePageAsync(id);
                _notificationService.Notify("Deleted Successfully!", NotificationType.Success);
                return Json(new { code = 200, Message = "", Data = result });
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
                _notificationService.Notify("Set Landing Page Successfully!", NotificationType.Success);
                return Json(new{code=200,Message="",Data=result});
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
        #endregion



    }

}