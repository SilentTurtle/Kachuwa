using System;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Admin.ViewModel;
using Kachuwa.Data.Extension;
using Kachuwa.Extensions;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Kachuwa.Localization;
using Kachuwa.Web;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityRole = Kachuwa.Identity.Models.IdentityRole;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class RoleController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly IIdentityRoleService _identityRoleService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILocaleResourceProvider _localeResourceProvider;
        public RoleController(INotificationService notificationService,
            IIdentityRoleService identityRoleService,
            RoleManager<IdentityRole> roleManager, ILocaleResourceProvider localeResourceProvider)
        {
            _notificationService = notificationService;
            _identityRoleService = identityRoleService;
            _roleManager = roleManager;
            _localeResourceProvider = localeResourceProvider;
            _localeResourceProvider.LookUpGroupAt("Role");
        }

        #region Role Crud
        [Route("admin/role/page/{pageNo?}")]
        [Route("admin/role")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int pageNo = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = pageNo;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _identityRoleService.RoleService.GetListPagedAsync(pageNo, rowsPerPage, 1,
                "Where Name like @Query", "Name asc", new { Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/role/new")]
        public async Task<IActionResult> New()
        {

            return View();
        }


        [HttpPost]
        [Route("admin/role/new")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(IdentityRole model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    model.AutoFill();
                    if (await _identityRoleService.CheckNameExist(model.Name))
                    {
                        _notificationService.Notify(_localeResourceProvider.Get("Info"), _localeResourceProvider.Get("Role.AlreadyExist"), NotificationType.Info);
                        return View(model);
                    }
                    var status = await _identityRoleService.RoleService.InsertAsync<int>(model);
                    _notificationService.Notify(_localeResourceProvider.Get("Success"), _localeResourceProvider.Get("Data has been saved successfully!"), NotificationType.Success);
                    return RedirectToAction("Index");

                }
                return RedirectToAction("Index");
            }
            else
            {
                var d = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                _notificationService.Notify(_localeResourceProvider.Get("Validation"), string.Join(',', d), NotificationType.Error);

                return View(model);
            }
        }




        [Route("admin/role/edit/{roleId}")]
        public async Task<IActionResult> Edit([FromRoute]int roleId)
        {
            var role = await _identityRoleService.RoleService.GetAsync(roleId);
            if (role.IsSystem)
            {
                _notificationService.Notify(_localeResourceProvider.Get("Warning"),
                    _localeResourceProvider.Get("System roles are uneditable!"), NotificationType.Warning);
                return RedirectToAction("Index");
            }

            var model = new RoleEditViewModel
            {
                Id = role.Id,
                Name = role.Name,
                OldName = role.Name,
                IsSystem = role.IsSystem
            };
        
            return View(model);
        }

        [HttpPost]
        [Route("admin/role/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityRole model,string oldName)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0)
                {
                    model.AutoFill();
                    if (model.Name.ToLower() != oldName.ToLower())
                    {
                        if (await _identityRoleService.CheckNameExist(model.Name))
                        {
                            _notificationService.Notify(_localeResourceProvider.Get("Info"), _localeResourceProvider.Get("Role.AlreadyExist"), NotificationType.Info);
                            return View(model);
                        }
                    }
                    var status = await _identityRoleService.RoleService.UpdateAsync(model);
                    _notificationService.Notify(_localeResourceProvider.Get("Success"), _localeResourceProvider.Get("Data has been saved successfully!"),
                        NotificationType.Success);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            else
            {
                var d = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                _notificationService.Notify(_localeResourceProvider.Get("Validation"), string.Join(',', d), NotificationType.Error);

                return View(model);
            }
        }

        [HttpPost]
        [Route("admin/role/delete")]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var model = await _identityRoleService.RoleService.GetAsync(id);
                if (model.IsSystem)
                {
                    _notificationService.Notify(_localeResourceProvider.Get("Warning"),
                        _localeResourceProvider.Get("System roles are undeletable!"), NotificationType.Warning);
                    return Json(new { code = 403, Message = "", Data = false });
                }
                var result = await _identityRoleService.RoleService.DeleteAsync(id);
                _notificationService.Notify(_localeResourceProvider.Get("Success"), _localeResourceProvider.Get("Data deleted successfully!"), NotificationType.Success);
                return Json(new { code = 200, Message = "", Data = result });
            }
            catch (Exception e)
            {
                return Json(new { code = 200, Message = e.Message, Data = false });
            }

        }

        #endregion

    }
}