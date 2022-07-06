using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Admin.ViewModel;
using Kachuwa.Data.Extension;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Kachuwa.Identity.ViewModels;
using Kachuwa.Localization;
using Kachuwa.Web;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityUser = Kachuwa.Identity.Models.IdentityUser;
using IdentityRole = Kachuwa.Identity.Models.IdentityRole;
using Kachuwa.Web.Service;
using Kachuwa.Extensions;
using System.Text.RegularExpressions;
using Kachuwa.Web.API;
using System.Net;
using Kachuwa.Log;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(PolicyConstants.PagePermission)]
    public class UserController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityRoleService _identityRoleService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAppUserService _appUserService;
        private readonly ILocaleResourceProvider _localeResourceProvider;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly ILogger _logger;

        public UserController(INotificationService notificationService,
            UserManager<IdentityUser> userManager, IIdentityRoleService identityRoleService,
            RoleManager<IdentityRole> roleManager, IAppUserService appUserService
            , ILocaleResourceProvider localeResourceProvider,
             IExportService exportService,
            IImportService importService,ILogger logger
            )
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _identityRoleService = identityRoleService;
            _roleManager = roleManager;
            _appUserService = appUserService;
            _localeResourceProvider = localeResourceProvider;
            _exportService = exportService;
            _importService = importService;
            _logger = logger;
            _localeResourceProvider.LookUpGroupAt("User");
        }

        #region User Crud
        [Route("admin/user/page/{pageNo?}")]
        [Route("admin/user")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int pageNo = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = pageNo;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _appUserService.AppUserCrudService.GetListPagedAsync(pageNo, rowsPerPage, 1,
                "Where FirstName like @Query and IsDeleted=@IsDeleted", "Addedon desc", new { IsDeleted=false, Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/user/new")]
        public async Task<IActionResult> New()
        {

            var model = new UserViewModel();
            model.UserRoles = await GetRoles();
            return View(model);
        }


        private async Task<List<UserRolesSelected>> GetRoles(List<int> roleIds = null)
        {
            var roles = await _identityRoleService.RoleService.GetListAsync();
            var model = roles.Select(r => new UserRolesSelected
            {
                RoleId = r.Id,
                IsSelected = roleIds != null ? roleIds.Contains((int)r.Id) : false,
                Name = r.Name
            }).ToList();
            return model;
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
                        model.UserRoles = await GetRoles();
                        _notificationService.Notify("Validation Error!", NotificationType.Error);
                        return View(model);
                    }
                    else
                    {
                        _notificationService.Notify("Success", "Data has been saved successfully!", NotificationType.Success);
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                var d = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                _notificationService.Notify("Validation Error!", NotificationType.Error);
                model.UserRoles = await GetRoles();
                return View(model);
            }
        }




        [Route("admin/user/edit/{appuserId}")]
        public async Task<IActionResult> Edit([FromRoute]int appuserId)
        {
            var user = await _appUserService.GetAsync(appuserId);
            var roles = await _identityRoleService.RoleService.GetListAsync();
            user.UserRoles = await GetRoles(user.UserRoleIds);

            return View(user);
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
                    var status = await _appUserService.UpdateUserAsync(model);
                    if (status.HasError)
                    {
                        model.UserRoles = await GetRoles(model.UserRoles.Where(c => c.IsSelected == true).Select(d => (int)d.RoleId).ToList());
                        ModelState.AddModelError("AppUserId", status.Message);
                        _notificationService.Notify("Validation Error!", NotificationType.Error);
                        return View(model);
                    }
                    else
                    {
                        _notificationService.Notify("Success", "Data has been saved successfully!", NotificationType.Success);
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                _notificationService.Notify("Validation Error!", NotificationType.Error);
                model.UserRoles = await GetRoles(model.UserRoles.Where(c => c.IsSelected == true).Select(d => (int)d.RoleId).ToList());
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
                _notificationService.Notify("Success", "Data deleted successfully!", NotificationType.Success);
                return Json(new { code = 200, Message = "", Data = result });
            }
            catch (Exception e)
            {
                return Json(new { code = 200, Message = e.Message, Data = false });
            }

        }

        #endregion
        [Route("admin/user/changepassword/{appuserId}")]
        public async Task<IActionResult> ChangePassword([FromRoute]int appuserId)
        {
            var user = await _appUserService.GetAsync(appuserId);
            return View(new ChangePasswordViewModel()
            {
                EmailOrUserName = user.Email
            });
        }
        [Route("admin/user/changepassword/{appuserId}")]
        [Route("admin/user/changepassword")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.EmailOrUserName);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result.Succeeded)
                {
                    _notificationService.Notify(_localeResourceProvider.Get("Success"),
                        _localeResourceProvider.Get("Data has been saved successfully."),
                        NotificationType.Success);
                }
                else
                {
                    _notificationService.Notify(_localeResourceProvider.Get("Error"), _localeResourceProvider.Get("failed to change password."),
                        NotificationType.Error);
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }
        [Route("admin/user/import")]
        public async Task<IActionResult> Import()
        {
            var model = new UserImportViewModel();          
            model.UserRoles = await GetRoles();
            return View(model);
        }
        [HttpPost]
        [Route("admin/user/import")]
        public async Task<IActionResult> Import(UserImportViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImportFile == null)
                {

                    ModelState.AddModelError("ImportFile", "Please upload excel file.");                 
                    model.UserRoles = await GetRoles();
                    return View(model);
                }
                else
                {
                    var importedUsers = _importService.Import<AppUser>(model.ImportFile);
                    foreach (var user in importedUsers)
                    {
                        var userViewModel = user.To<UserViewModel>();                       
                        userViewModel.Password = "";
                        userViewModel.UserRoles = model.UserRoles;
                        string suffix = "";

                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        Random random = new Random();
                        suffix = new string(Enumerable.Repeat(chars, 2)
                            .Select(s => s[random.Next(s.Length)]).ToArray());

                        if (model.AutoGenerateUserName)
                        {                           

                            userViewModel.UserName =
                                Regex.Replace(userViewModel.FirstName.Trim().Replace(" ", String.Empty),
                                    @"(\s+|@|&|'|\(|\)|<|>|#)", "") +
                                Regex.Replace(userViewModel.LastName.Trim().Replace(" ", String.Empty),
                                    @"(\s+|@|&|'|\(|\)|<|>|#)", "") + DateTime.Now.Year
                                + suffix;
                           
                        }

                        if (model.AutoGenerateEmailAddress)
                        {
                            if (string.IsNullOrEmpty(userViewModel.Email))
                            {
                                userViewModel.Email =
                                    Regex.Replace(userViewModel.FirstName.Trim().Replace(" ", String.Empty),
                                        @"(\s+|@|&|'|\(|\)|<|>|#)", "")
                                    + Regex.Replace(userViewModel.LastName.Trim().Replace(" ", String.Empty),
                                        @"(\s+|@|&|'|\(|\)|<|>|#)", "")
                                    + DateTime.Now.Year + suffix +
                                    "@kachuwaframework.com";
                            }
                            else
                            {

                                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                                Match match = regex.Match(userViewModel.Email);
                                if (!match.Success)
                                {
                                    userViewModel.Email =
                                        Regex.Replace(userViewModel.FirstName.Trim().Replace(" ", String.Empty),
                                            @"(\s+|@|&|'|\(|\)|<|>|#)", "")
                                        + Regex.Replace(userViewModel.LastName.Trim().Replace(" ", String.Empty),
                                            @"(\s+|@|&|'|\(|\)|<|>|#)", "")
                                        + DateTime.Now.Year + suffix +
                                        "@kachuwaframework.com";
                                }
                            }
                        }
                       
                        userViewModel.Password = "kachuwaframework";
                        
                        var status = await _appUserService.SaveNewUserAsync(userViewModel);
                        if (status.HasError)
                        {
                            userViewModel.ImportMessage = status.Message;
                            model.Users.Add(userViewModel);
                            ModelState.AddModelError("", $"{userViewModel.UserName}/{userViewModel.Email} {status.Message}");
                        }

                        // ModelState.AddModelError("", $"{userViewModel.UserName}/{userViewModel.Email} {status.Message}");
                    }

                    model.ImportStatus = true;
                    model.Message = "Imported Succefully.";                
                    model.UserRoles = await GetRoles();
                    return View(model);
                }


            }
            else
            {
                var d = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                _notificationService.Notify("Validation Error!", NotificationType.Error);
                model.UserRoles = await GetRoles();             
                return View(model);
            }
        }
        [HttpPost]
        [Route("admin/user/updatestatus")]
        public async Task<ApiResponse> UpdateUser(int id, bool status)
        {
            try
            {
                if (id < 1)
                {
                    return new ApiResponse { Code = (int)ApiResponseCodes.Codes.ModelValidationError, Message = _localeResourceProvider.Get("Course.InvalidCourse") };
                }
                var result = await _appUserService.AppUserCrudService.UpdateStatusAsync(id, status);
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