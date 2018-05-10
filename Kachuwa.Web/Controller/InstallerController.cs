using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Configuration;
using Kachuwa.Extensions;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Kachuwa.Identity.ViewModels;
using Kachuwa.Installer;
using Kachuwa.Web.Extensions;
using Kachuwa.Web.Service;
using Kachuwa.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Kachuwa.Web
{
    public class InstallerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IKachuwaConfigurationManager _kachuwaManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IIdentityRoleService _identityRoleService;
        private readonly IAppUserService _appUserService;
        private readonly ISettingService _settingService;
        private readonly KachuwaAppConfig _kachuwaConfig;

        public InstallerController(IConfiguration configuration,
            IOptionsSnapshot<KachuwaAppConfig> kachuwaConfig,
            IKachuwaConfigurationManager kachuwaManager,
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender,
            SignInManager<IdentityUser> signInManager,
            IIdentityRoleService identityRoleService,
            IAppUserService appUserService, ISettingService settingService)
        {
            _configuration = configuration;
            _kachuwaManager = kachuwaManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _identityRoleService = identityRoleService;
            _appUserService = appUserService;
            _settingService = settingService;
            _kachuwaConfig = kachuwaConfig.Value;
        }
        [Route("install")]
        public async Task<IActionResult> Index()
        {
            var model = new InstallerViewModel();
            return PartialView("_Installer", model);
        }
        [Route("install/ping")]
        public Task<string> Ping()
        {
            return Task.FromResult("pong");
        }

        [Route("install")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Install(InstallerViewModel model)
        {
            try
            {

                var connectionString = model.ToString();
                if (await _kachuwaManager.Install(connectionString))
                {
                    return Json(new { Code = 200, Data = model, Message = "Installed Successfully." });
                }
                else
                {
                    return Json(new { Code = 500, Data = model, Message = "Something went wrong!Try using new database for setup." });
                }
            }
            catch (DivideByZeroException e)
            {
                return Json(new { Code = 500, Data = model, Message = "Connection String Error." });
            }
            catch (Exception e)
            {
                return Json(new { Code = 500, Data = model, Message = e.Message });
            }
        }

        [Route("install/checkconnection")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CheckConnection(InstallerViewModel model)
        {
            try
            {
                var connectionString = model.ToString();
                if (await _kachuwaManager.CheckConnection(connectionString))
                {
                    return Json(new { Code = 200, Data = model, Message = "Database connected successfully." });
                }
                else
                {
                    return Json(new { Code = 500, Data = model, Message = "Connection failed.Try Again." });
                }
            }
            catch (Exception e)
            {
                return Json(new { Code = 500, Data = model, Message = e.Message });
            }
        }


        [Route("install/setupadmin")]
        [HttpPost]
        public async Task<JsonResult> SetUpAdmin(InstallerUserViewModel model)
        {
            try
            {
               
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                    var userVm = user.To<UserViewModel>();
                    userVm.Password = model.Password;
                    userVm.FirstName = "First Name";
                    userVm.LastName = "Last Name";
                    userVm.UserRoles = new List<UserRolesSelected>
                    {
                        new UserRolesSelected{IsSelected = true,RoleId = KachuwaRoles.SuperAdmin},
                        new UserRolesSelected{IsSelected = true,RoleId = KachuwaRoles.Admin},
                    };
                 
                    var result = await _appUserService.SaveNewUserAsync(userVm);
                    if (!result.HasError)
                    {
                        await _signInManager.PasswordSignInAsync(model.Email, model.Password,true, lockoutOnFailure: false);
                        var defaultSetting = await _settingService.GetSetting();
                        defaultSetting.WebsiteName = model.SiteName;
                        defaultSetting.TimeZoneId = model.TimeZoneId;
                        await _settingService.SaveSetting(defaultSetting);
                        return Json(
                            new
                            {
                                Code = StatusCodes.Status201Created,
                                Message = "User Account Registered successfully.",
                                Data = user
                            });
                    }
                    return Json(new
                    {
                        Code = StatusCodes.Status412PreconditionFailed,
                        Message = result.Message,Data=new ArrayList()
                       
                    });
                }
                return Json(
                    new
                    {
                        Code = StatusCodes.Status406NotAcceptable,
                        Message = "Invalid inputs",
                        Data = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList()
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}