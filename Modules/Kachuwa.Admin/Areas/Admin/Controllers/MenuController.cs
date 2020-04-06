using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Data.Extension;
using Kachuwa.Extensions;
using Kachuwa.Identity.Service;
using Kachuwa.Localization;
using Kachuwa.Web;
using Kachuwa.Web.Model;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Kachuwa.Web.Service;
using Kachuwa.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;
        private readonly INotificationService _notificationService;
        private readonly ILocaleResourceProvider _localeResourceProvider;
        private readonly IIdentityRoleService _identityRoleService;

        public MenuController(IMenuService menuService, INotificationService notificationService
        , ILocaleResourceProvider localeResourceProvider,IIdentityRoleService identityRoleService)
        {
            _menuService = menuService;
            _notificationService = notificationService;
            _localeResourceProvider = localeResourceProvider;
            _identityRoleService = identityRoleService;
            _localeResourceProvider.LookUpGroupAt("Menu");
        }
        #region Menu Crud
        [Route("admin/menu/page/{pageNo?}")]
        [Route("admin/menu")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int pageNo = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = pageNo;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _menuService.MenuCrudService.GetListAsync("Where IsBackend=@IsBackend and  Name like @Query and IsDeleted=0 order by MenuOrder asc;", new { IsBackend=true, Query = "%" + query + "%" });
            return View(model);
        }
    
        [Route("admin/menu/frontend")]//default make it at last
        public async Task<IActionResult> Frontend([FromRoute]int pageNo = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = pageNo;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _menuService.MenuCrudService.GetListAsync("Where  IsBackend=@IsBackend and Name like @Query and IsDeleted=0 order by MenuOrder asc;", new { IsBackend=false, Query = "%" + query + "%" });
            return View(model);
        }

        //[Route("admin/menu/new")]
        //public async Task<IActionResult> New()
        //{

        //    return View();
        //}

        //[HttpPost]
        //[Route("admin/menu/new")]
        //public async Task<IActionResult> New(Menu model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // model.Url = model.Url.TrimStart(new char[] { '/' });
        //        model.AutoFill();
        //        if (model.MenuId == 0)
        //            await _menuService.MenuCrudService.InsertAsync<int>(model);
        //        else
        //            await _menuService.MenuCrudService.UpdateAsync(model);
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return View(model);
        //    }
        //}


        //[Route("admin/menu/edit/{pageId}")]
        //public async Task<IActionResult> Edit([FromRoute]int pageId)
        //{
        //    var model = await _menuService.MenuCrudService.GetAsync(pageId);
        //    return View(model);
        //}

        //[HttpPost]
        //[Route("admin/menu/edit")]
        //public async Task<IActionResult> Edit(Menu model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        model.AutoFill();
        //        if (model.MenuId == 0)
        //            await _menuService.MenuCrudService.InsertAsync<int>(model);
        //        else
        //            await _menuService.MenuCrudService.UpdateAsync(model);
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return View(model);
        //    }
        //}

        [HttpPost]
        [Route("admin/menu/delete")]
        public async Task<JsonResult> Delete(int id)
        {
            var result = await _menuService.MenuCrudService.DeleteAsync(id);
            await _menuService.PermissionCrudService.DeleteAsync("Where MenuId=@MenuId", new { MenuId = id });
            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("admin/menu/get")]
        public async Task<JsonResult> GetMenu(int id)
        {
            var result = await _menuService.MenuCrudService.GetAsync("Where MenuId=@MenuId", new {MenuId = id});
            var permissions =
                await _menuService.PermissionCrudService.GetListAsync("Where MenuId=@MenuId", new { MenuId = id });
            var model = result.To<MenuViewModel>();
            model.Permissions = permissions.ToList();
            return Json(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("admin/menu/save")]
        public async Task<JsonResult> SaveMenu(MenuViewModel model)
        {
            var status = await _menuService.SaveMenu(model);
            return Json(status > 0);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("admin/menu/sort/save")]
        public async Task<JsonResult> SortMenu(List<MenuOrderViewModel> model)
        {
            var status = await _menuService.SaveMenuOrder(model);
            return Json(status);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("admin/menu/order/save")]
        public async Task<JsonResult> SaveMenuOrder(List<MenuOrderViewModel> orders)
        {
            var status = await _menuService.SaveMenuOrder(orders);
            return Json(status);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("admin/menu/roles")]
        public async Task<JsonResult> GetRoles( )
        {
            var roles = await _identityRoleService.RoleService.GetListAsync();
            var _roles = roles.Select(x => new { Id = x.Id, Name = x.Name });
            return Json(new {Code = 200, Data = _roles, Message = "Success"});
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("admin/menu/groups")]
        public async Task<JsonResult> GetGroups()
        {
            var groups = await _menuService.GroupCrudService.GetListAsync("Where IsActive=@IsActive",new{ IsActive =true});
            return Json(groups);
        }
        #endregion


        #region Menu group Crud
        [Route("admin/menu/group/page/{page?}")]
        [Route("admin/menu/group")]//default make it at last
        public async Task<IActionResult> GroupIndex([FromRoute]int page = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = page;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _menuService.GroupCrudService.GetListPagedAsync(page, rowsPerPage, 1,
                "Where Name like @Query and IsDeleted=0", "Addedon desc", new { Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/menu/group/new")]
        public async Task<IActionResult> GroupNew()
        {

            return View();
        }

        [HttpPost]
        [Route("admin/menu/group/new")]
        public async Task<IActionResult> GroupNew(MenuGroup model)
        {
            if (ModelState.IsValid)
            {
                // model.Url = model.Url.TrimStart(new char[] { '/' });
                model.AutoFill();
                if (model.MenuGroupId == 0)
                    await _menuService.GroupCrudService.InsertAsync<int>(model);
                else
                    await _menuService.GroupCrudService.UpdateAsync(model);
                return RedirectToAction("GroupIndex");
            }
            else
            {
                return View(model);
            }
        }


        [Route("admin/menu/group/edit/{groupId}")]
        public async Task<IActionResult> GroupEdit([FromRoute]int groupId)
        {
            var model = await _menuService.GroupCrudService.GetAsync(groupId);
            return View(model);
        }

        [HttpPost]
        [Route("admin/menu/group/edit")]
        public async Task<IActionResult> TypeEdit(MenuGroup model)
        {
            if (ModelState.IsValid)
            {
                model.AutoFill();
                if (model.MenuGroupId == 0)
                    await _menuService.GroupCrudService.InsertAsync<int>(model);
                else
                    await _menuService.GroupCrudService.UpdateAsync(model);
                return RedirectToAction("GroupIndex");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [Route("admin/menu/group/delete")]
        public async Task<JsonResult> GroupDelete(int id)
        {
            var result = await _menuService.GroupCrudService.DeleteAsync(id);
            return Json(result);
        }
        #endregion




    }
}