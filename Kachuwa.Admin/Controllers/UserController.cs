using System;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Admin.ViewModel;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Form;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Kachuwa.Identity.ViewModels;
using Kachuwa.Web;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityRoleService _identityRoleService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAppUserService _appUserService;
        public UserController(INotificationService notificationService, UserManager<IdentityUser> userManager, IIdentityRoleService identityRoleService, RoleManager<IdentityRole> roleManager, IAppUserService appUserService)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _identityRoleService = identityRoleService;
            _roleManager = roleManager;
            _appUserService = appUserService;
        }

        #region User Crud
        [Route("admin/user/page/{page?}")]
        [Route("admin/user")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int page = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = page;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _appUserService.AppUserCrudService.GetListPagedAsync(page, rowsPerPage, 1,
                "Where FirstName like @Query and IsDeleted=0", "Addedon desc", new { Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/user/new")]
        public async Task<IActionResult> New()
        {
           
            ViewData["FormDataSource"] = await GetDataSource();
            return View();
        }

        private async Task<FormDatasource> GetDataSource()
        {
            var formDataSource = new FormDatasource();
            var roles = await _identityRoleService.RoleService.GetListAsync();
            formDataSource.SetSource("Roles", roles.Select(r => new FormInputItem
            {
                Id = (int)r.Id,
                Value = r.Id.ToString(),
                Label = r.Name
            }).ToList());
            return formDataSource;
        }
        [HttpPost]
        [Route("admin/user/new")]
        public async Task<IActionResult> New(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.AppUserId == 0)
                {
                    model.AutoFill();
                    var status = await _appUserService.SaveNewUserAsync(model);
                    if (status.HasError)
                    {
                        ModelState.AddModelError("Save Error", status.Message);
                        ViewData["FormDataSource"] = await GetDataSource();
                        _notificationService.Notify("Validation Error!", NotificationType.Error);
                        return View(model);
                    }
                    else
                    {
                        _notificationService.Notify("Saved Successfully!", NotificationType.Success);
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                _notificationService.Notify("Validation Error!", NotificationType.Error);
                ViewData["FormDataSource"] = await GetDataSource();
                return View(model);
            }
        }




        [Route("admin/user/edit/{appuserId}")]
        public async Task<IActionResult> Edit([FromRoute]int appuserId)
        {
            var model = await _appUserService.GetAsync(appuserId);
            var formDataSource = new FormDatasource();
            var roles = await _identityRoleService.RoleService.GetListAsync();
            formDataSource.SetSource("Roles", roles.Select(r => new FormInputItem
            {
                Id = (int)r.Id,
                IsSelected = model.UserRoleIds.Contains((int)r.Id),
                Value = r.Id.ToString(),
                Label = r.Name
            }).ToList());
            ViewData["FormDataSource"] = formDataSource;
            return View(model);
        }

        [HttpPost]
        [Route("admin/user/edit")]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.AppUserId != 0)
                {
                    model.AutoFill();
                    var status = await _appUserService.SaveUserAsync(model);
                    if (status.HasError)
                    {
                        var formDataSource = new FormDatasource();
                        var roles = await _identityRoleService.RoleService.GetListAsync();
                        formDataSource.SetSource("Roles", roles.Select(r => new FormInputItem
                        {
                            Id = (int)r.Id,
                            IsSelected = model.UserRoleIds.Contains((int)r.Id),
                            Value = r.Id.ToString(),
                            Label = r.Name
                        }).ToList());
                        ViewData["FormDataSource"] = formDataSource;
                        ModelState.AddModelError("Save Error", status.Message);
                        _notificationService.Notify("Validation Error!", NotificationType.Error);
                        return View(model);
                    }
                    else
                    {
                        _notificationService.Notify("Saved Successfully!", NotificationType.Success);
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                _notificationService.Notify("Validation Error!", NotificationType.Error);
                var formDataSource = new FormDatasource();
                var roles = await _identityRoleService.RoleService.GetListAsync();
                formDataSource.SetSource("Roles", roles.Select(r => new FormInputItem
                {
                    Id = (int)r.Id,
                    IsSelected = model.UserRoleIds.Contains((int)r.Id),
                    Value = r.Id.ToString(),
                    Label = r.Name
                }).ToList());
                ViewData["FormDataSource"] = formDataSource;
                return View(model);
            }
        }

        [HttpPost]
        [Route("admin/user/delete")]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var result = await _appUserService.DeleteUserAsync(id);
                _notificationService.Notify("Deleted Successfully!", NotificationType.Success);
                return Json(new { code = 200, Message = "", Data = result });
            }
            catch (Exception e)
            {
                return Json(new { code = 200, Message = e.Message, Data = false });
            }

        }

        #endregion
        //    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        //    var existingUser = await _userManager.FindByEmailAsync(model.Email);
        //            if (existingUser != null)
        //            {
        //                ModelState.AddModelError("", "Email is already registered!.");
        //                return View(model);
        //}
        //var result = await _userManager.CreateAsync(user, model.Password);
        //            if (result.Succeeded)
    }
}