using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Kachuwa.Web.API;
using Kachuwa.Web.Services;
using Kachuwa.Log;
using Kachuwa.Identity.Service;
using Kachuwa.Storage;
using Kachuwa.Web.Service;
using Kachuwa.Configuration;
using Kachuwa.Identity.Extensions;
using Kachuwa.Web;
using Kachuwa.Identity.ViewModels;
using Kachuwa.Extensions;
using Kachuwa.Identity;
using Kachuwa.Identity.Models;
using Kachuwa.Data.Extension;
using IdentityUser = Kachuwa.Identity.Models.IdentityUser;
using Kachuwa.Data;
using System.Data.Common;
using Dapper;
using Kachuwa.API.ViewModels;

namespace Kachuwa.API.V1
{
    [Route("api/v1/user")]
    public partial class UserApiController : BaseApiController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityUserService _identityUserService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IAppUserService _userService;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IStorageProvider _storageProvider;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IExportService _exportService;
        private readonly ILoginHistoryService _loginHistoryService;
        // private readonly ICompanyService _branchService;


        private readonly KachuwaAppConfig _kachuwaAppConfig;
        public UserApiController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,

            IAppUserService userService, ILogger logger, IConfiguration configuration, IOptionsSnapshot<KachuwaAppConfig> kachuwaConfigSnap,
            IStorageProvider storageProvider, IWebHostEnvironment hostingEnvironment
            , IExportService exportService, ILoginHistoryService loginHistoryService,
            // ICompanyService branchService,
            IIdentityUserService identityUserService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _userService = userService;
            _logger = logger;
            _configuration = configuration;
            _storageProvider = storageProvider;
            _hostingEnvironment = hostingEnvironment;
            _exportService = exportService;
            _loginHistoryService = loginHistoryService;
            _kachuwaAppConfig = kachuwaConfigSnap.Value;
            // _branchService = branchService;
            _identityUserService = identityUserService;
        }


        [Route("profile/save")]
        [HttpPost]
        public async Task<dynamic> Save(AppUserRegisterModel model)
        {
            try
            {
                if (User.Identity.GetIdentityUserId() > 0)
                {


                    if (ModelState.IsValid)
                    {

                        var existing = await _userService.AppUserCrudService.GetAsync("Where IdentityUserId=@IdentityUserId", new { IdentityUserId = User.Identity.GetIdentityUserId() });
                        if (existing == null)
                            return ErrorResponse(500, "Unauthorized Access");
                        var appuser = model.To<AppUser>();
                        appuser.AppUserId = existing.AppUserId;
                        appuser.FirstName = model.FirstName;
                        string lastName = model.LastName;
                        if (string.IsNullOrEmpty(lastName))
                            lastName = " ";
                        appuser.LastName = lastName;
                        appuser.IdentityUserId = User.Identity.GetIdentityUserId();
                        appuser.AutoFill();
                        appuser.ProfilePicture = existing.ProfilePicture;
                        appuser.IsActive = true;
                        model.Email = existing.Email;
                        await _userService.AppUserCrudService.UpdateAsync(appuser);


                        return HttpResponse(200, "Your information saved successfully.",
                            model);

                    }
                    else
                    {
                        return ValidationResponse(
                            ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList());
                    }
                }
                else
                {
                    return ErrorResponse(500, "Unauthorized Access");
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }
        }
        [HttpGet]
        [Route("type/all")]
        public async Task<dynamic> GetUserTypes()
        {
            try
            {

                var data = await _userService.UserTypeCrudService.GetListAsync("where IsActive=@IsActive",
                    new { IsActive = true });
                return HttpResponse(200, "", data);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ExceptionResponse(e, "");
            }

        }
        [HttpGet]
        [Route("listactive")]
        public async Task<dynamic> ListActive()
        {
            try
            {
                string sql = "select IdentityUserId,FirstName+' '+LastName as FullName,UserName from AppUser where IsActive=@isActive";
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    var data = await db.QueryAsync(sql, new { @isActive = true });
                    return HttpResponse(200, "", data);
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ExceptionResponse(e, "");
            }

        }
        [HttpGet]
        [Route("list")]
        public async Task<dynamic> List(int pageNo = 1, int pageSize = 25, string key = "", string query = "")
        {
            try
            {
                string condition = " where IsDeleted=@isDeleted ";
                if (query != "")
                {
                    condition += " and (FirstName like @name or CompanyName like @name or @name is null)";
                }
                var data = await _userService.AppUserCrudService.GetListPagedAsync(pageNo, pageSize, pageSize, condition, "AppUserId desc", new { @isDeleted = false, @name = "%" + query + "%" });
                return HttpResponse(200, "", data);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ExceptionResponse(e, "");
            }

        }

        [Route("profile")]
        [HttpGet]

        public async Task<dynamic> Profile(long userId)
        {
            if (User.Identity.GetIdentityUserId() > 0)
            {

                var user = await _userService.AppUserCrudService.GetAsync("Where IdentityUserId=@IdentityUserId", new { IdentityUserId = User.Identity.GetIdentityUserId() });

                return HttpResponse(200, "success", user);
            }
            else
            {
                return ErrorResponse(401, "Unauthorized Access");
            }

        }
        [Route("detail")]
        [HttpGet]
        public async Task<dynamic> GetUserDetail(long userId)
        {
            if (User.Identity.GetIdentityUserId() > 0)
            {
                var user = await _userService.AppUserCrudService.GetAsync("Where AppUserId=@AppUserId", new { AppUserId = userId });
                return HttpResponse(200, "success", user);
            }
            else
            {
                return ErrorResponse(401, "Unauthorized Access");
            }



        }
        [Route("edit")]
        [HttpGet]

        public async Task<dynamic> Edit(long id)
        {
            if (User.Identity.GetIdentityUserId() > 0)
            {

                AppUser user = await _userService.AppUserCrudService.GetAsync("Where AppUserId=@id", new { @id = id });
                if (user != null)
                {
                    UserRegisterViewModel vm = user.To<UserRegisterViewModel>();
                    //BranchUser bu = await _branchService.CrudServiceBranchUser.GetAsync("where IdentityUserId=@userId and IsActive=@isActive", new { @userId = user.IdentityUserId, @isActive = true });
                    //if (bu != null)
                    //{
                    //    vm.BranchId = bu.BranchId;
                    //}
                    var roles = await _identityUserService.GetUserRoles(user.IdentityUserId);
                    if (roles.Any())
                    {
                        vm.Roles = new int[roles.Count()];
                        for (int i = 0; i < roles.Count(); i++)
                        {
                            vm.Roles[i] = (int)roles[i];
                        }
                    }
                    return HttpResponse(200, "success", vm);
                }
                else
                {
                    return ErrorResponse(401, "Unauthorized Access");
                }
            }
            else
            {
                return ErrorResponse(400, "No user found");
            }

        }

        [Route("check/active")]
        [HttpGet]

        public async Task<dynamic> CheckIsActive()
        {
            if (User.Identity.GetIdentityUserId() > 0)
            {

                var user = await _userService.AppUserCrudService.GetAsync(User.Identity.GetIdentityUserId());

                return HttpResponse(200, "success", user.IsActive);
            }
            else
            {
                return ErrorResponse(401, "Unauthorized Access");
            }

        }


        [Route("change/phone")]
        [HttpPost]
        public async Task<dynamic> ChangePhone(string phoneNumber)
        {
            try
            {
                var userId = User.Identity.GetIdentityUserId();
                if (userId > 0)
                {

                    await _userService.ChangePhoneNumberAsync(userId, phoneNumber);
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
                    await _userManager.VerifyChangePhoneNumberTokenAsync(user, token, phoneNumber);
                    return HttpResponse(200, "success");


                }
                else
                {
                    return ErrorResponse(401, "Unauthorized Access");
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }

        }
        [Route("phone/verification/status")]
        public async Task<dynamic> GetPhoneVerificationStatus()
        {
            try
            {
                var userId = User.Identity.GetIdentityUserId();
                if (userId > 0)
                {

                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    var status = await _userManager.IsPhoneNumberConfirmedAsync(user);
                    return HttpResponse(200, "success", status);

                }
                else
                {
                    return ErrorResponse(401, "Unauthorized Access");
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }

        }

        [Route("login")]
        [HttpPost]
        public async Task<dynamic> Login(string userName, string password, string branchCode)
        {
            try
            {

                var tokenResponse = await RequestToken(userName, password);
                string token = "", rfTorken = "";
                if (tokenResponse.IsError)
                {
                    return HttpResponse(500, "invalid username or password");
                    //return ErrorResponse(500, tokenResponse.ErrorDescription);
                }
                else
                {
                    token = tokenResponse.AccessToken;
                    rfTorken = tokenResponse.RefreshToken;

                }

                string ip = ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString();
                string ua = ControllerContext.HttpContext.Request.Headers["User-Agent"].ToString();
                var uaObj = new UserAgent(ua);
                if (userName.IndexOf('@') > -1)
                {


                    var appuser = await _userService.AppUserCrudService.GetAsync("Where Email=@Email and IsActive=1",
                          new { Email = userName });

                    var user = await _userManager.FindByIdAsync(appuser.IdentityUserId.ToString());
                    var isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                    appuser.Token = token;
                    appuser.RefreshToken = rfTorken;

                    return HttpResponse(200, "success", new { User = appuser });
                }
                else
                {
                    var appuser = await _userService.AppUserCrudService.GetAsync(
                         "Where IsActive=@isActive and UserName=@UserName",
                         new { UserName = userName, isActive = true });

                    appuser.Token = token;
                    appuser.RefreshToken = rfTorken;


                    return HttpResponse(200, "success", new { User = appuser });

                }


            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }

        }

        /// <summary>
        /// register new user from mobile devices
        /// </summary>
        /// <param name="model"></param>
        /// <returns>return profile with token </returns>
        [Route("signup")]
        [HttpPost]
        public async Task<dynamic> Register(RegisterViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Password) || model.Password.Length < 8)
                {
                    ModelState.AddModelError(model.Password, "Password must be of at least 8 character.");
                }

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(model.Email, "Email already registered.");
                }
                //if (!model.UserName.IsAlphaNumericWithUnderscore())
                //{
                //    ModelState.AddModelError("UserName", "invalid characters in UserName");

                //}
                if (ModelState.IsValid)
                {
                    UserViewModel newAppUser = model.To<UserViewModel>();

                    newAppUser.UserRoles = new List<UserRolesSelected>
                    {
                        new UserRolesSelected()
                        {
                            Name = KachuwaRoleNames.User,
                            IsSelected = true,
                            RoleId = KachuwaRoles.User
                        }
                    };
                    var userStatus = await _userService.SaveNewUserAsync(newAppUser);

                    if (!userStatus.HasError)
                    {
                        var user = await _userManager.FindByEmailAsync(model.Email);

                        var appuser =
                            await _userService.AppUserCrudService.GetAsync("Where IdentityUserId=@Id", new { Id = user.Id });
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var tokenResponse = await RequestToken(model.Email, model.Password);
                        string token = "", rfTorken = "";
                        if (tokenResponse.IsError)
                        {
                            return HttpResponse(500, "invalid username or password");
                            //return ErrorResponse(500, tokenResponse.ErrorDescription);
                        }
                        else
                        {
                            token = tokenResponse.AccessToken;
                            rfTorken = tokenResponse.RefreshToken;

                        }
                        appuser.Token = token;
                        appuser.RefreshToken = rfTorken;
                        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                        return HttpResponse(200, "success", new { User = appuser });
                    }
                    else
                    {
                        return ErrorResponse(500, "Failed to add new user at the moment,Try again later. ");
                    }
                }
                else
                {
                    return ValidationResponse(ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList());
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }
        }


        [Route("picture/upload")]
        [HttpPost]
        public async Task<dynamic> UploadProfilePicture(IFormFile file)
        {
            try
            {
                if (User.Identity.GetIdentityUserId() > 0)
                {

                    if (file != null)
                    {
                        string filepath = await _storageProvider.Save("UserProfile", file);
                        // return HttpResponse(ApiResponseCode.Success, "File uploaded saved successfully.", filepath);
                        await _userService.UpdateProfilePicture(User.Identity.GetIdentityUserId(), filepath);
                        return HttpResponse(200, "Your information saved successfully.", filepath);

                    }
                    return ValidationResponse(new string[] { "Please upload file first.." }.ToList());
                }
                else
                {
                    return ErrorResponse(401, "Unauthorized Access");
                }


            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }

        }


        /// <summary>
        /// update mobile users profice picture
        /// </summary>
        /// <param name="model"></param>
        /// <returns>return status </returns>
        [Route("picture/update")]
        [HttpPost]
        public async Task<dynamic> UpdatePicture(ProfilePictureViewModel model)
        {
            try
            {
                if (model.IdentityUserId == 0)
                {
                    ModelState.AddModelError(model.IdentityUserId.ToString(), "Invalid app user id.");
                }
                if (string.IsNullOrEmpty(model.ImagePath))
                {
                    ModelState.AddModelError(model.ImagePath, "Invalid image path.");
                }
                if (ModelState.IsValid)
                {
                    await _userService.UpdateProfilePicture(model.IdentityUserId, model.ImagePath);
                    return HttpResponse(200, "Your information saved successfully.", true);

                }
                else
                {
                    return ValidationResponse(ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList());
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }
        }

        [Route("changepassword")]
        [HttpPost]
        public async Task<dynamic> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (User.Identity.GetIdentityUserId() > 0)
                {
                    var user = await _userManager.FindByIdAsync(User.Identity.GetIdentityUserId().ToString());
                    var status = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (status.Succeeded)
                        return HttpResponse(200, "Password changed successfully.", true);
                    else
                        return HttpResponse(500, "Unable to change password.", false);
                }
                else
                {
                    return NotAuthorizedResponse();
                }




            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }

        }
        [HttpPost]
        [Route("resetpassword")]
        public async Task<dynamic> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResponse(ModelState, 400, null);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return HttpResponse(400, "No User Found", null);
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
            if (result.Succeeded)
            {
                return HttpResponse(200, "Password Successfully Reset", null);
            }
            else
            {
                string errorMessage = string.Join(";", result.Errors.SelectMany(x => x.Description));
                return HttpResponse(200, errorMessage, null);
            }
        }

        [Route("forgotpassword")]
        [HttpPost]
        public async Task<dynamic> ForgotPassword(string email)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var appuser =
                        await _userService.AppUserCrudService.GetAsync("Where IdentityUserId=@Id", new { Id = user.Id });
                    // For more information on how to enable account confirmation and password reset please
                    // visit https://go.microsoft.com/fwlink/?LinkID=532713
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl =
                        $"https://wholisticminds.com/account/resetpassword?code={code}&userName={appuser.UserName}"; //Url.ResetPasswordCallbackLink(user.UserName.ToString(), code, Request.Scheme);
                    await _emailSender.SendTemplatedEmailAsync("Reset Password", "emailtemplates/ok_forgot_password.html", new
                    {
                        Name = $"{appuser.FirstName} {appuser.LastName}",
                        ForgotPasswordLink = callbackUrl
                    }, new EmailAddress[]
                    {
                        new EmailAddress
                        {
                            Email = appuser.Email,
                            DisplayName = $"{appuser.FirstName} {appuser.LastName}"

                        }
                    });
                    //  _logger.Log(LogType.Info, () => $"Reset Password,Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>{to.ToArray()}");

                    //await _emailSender.SendEmailAsync("Reset Password",
                    //    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>", to.ToArray());
                }
                return HttpResponse(200, "Reset email has been sent to your email.", true);



            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }

        }

        [Route("email/send")]
        [HttpPost]
        public async Task<dynamic> SendVerificationEmail(string emailorUserName)
        {
            try
            {

                var user = emailorUserName.Contains("@") == true ? await _userManager.FindByEmailAsync(emailorUserName) : await _userManager.FindByNameAsync(emailorUserName);
                if (user != null)
                {
                    var appuser =
                        await _userService.AppUserCrudService.GetAsync("Where IdentityUserId=@Id", new { Id = user.Id });
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl =
                       $"https://wholisticminds.com/account/ConfirmEmail?code={code}&userName={appuser.UserName}"; //Url.ResetPasswordCallbackLink(user.UserName.ToString(), code, Request.Scheme);
                                                                                                                   // Url.EmailConfirmationLink(user.UserName.ToString(), code, Request.Scheme);

                    _logger.Log(LogType.Info, () => callbackUrl);
                    // await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _emailSender.SendTemplatedEmailAsync("Verify Your Email",
                        "emailtemplates/email_verify.html", new
                        {
                            VerificationLink = callbackUrl,
                            Name = $"{appuser.FirstName} {appuser.LastName}"
                        }, new EmailAddress[]
                        {
                            new EmailAddress
                            {
                                Email = appuser.Email,
                                DisplayName = $"{appuser.FirstName} {appuser.LastName}"

                            }
                        });
                    //  _logger.Log(LogType.Info, () => $"Reset Password,Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>{to.ToArray()}");

                    //await _emailSender.SendEmailAsync("Reset Password",
                    //    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>", to.ToArray());
                }
                return HttpResponse(200, "Reset email has been sent to your email.", true);



            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return ErrorResponse(500, e.Message);
            }

        }

        [HttpPost]
        [Route("save")]
        public async Task<dynamic> Save(UserRegisterViewModel model)
        {
            model.AutoFill();
            if (ModelState.IsValid)
            {
                if (model.AppUserId == 0)
                {
                    //if (!model.UserName.IsAlphaNumericWithUnderscore())
                    //{
                    //    return HttpResponse(500, "invalid characters in UserName", model);
                    //}
                    var newAppUser = model.To<UserViewModel>();
                    newAppUser.Email = newAppUser.Email.Trim();
                    newAppUser.UserRoles = new List<UserRolesSelected>();
                    if (model.Roles != null && model.Roles.Count() > 0)
                    {
                        foreach (var item in model.Roles)
                        {
                            newAppUser.UserRoles.Add(new UserRolesSelected()
                            {
                                Name = item.ToString(),
                                IsSelected = true,
                                RoleId = item
                            });
                        }
                    }
                    else
                    {
                        newAppUser.UserRoles = new List<UserRolesSelected>
                        {
                            new UserRolesSelected()
                            {
                                Name = KachuwaRoleNames.User,
                                IsSelected = true,
                                RoleId = KachuwaRoles.User
                            }

                        };
                    }
                    var status = await _userService.SaveNewUserAsync(newAppUser);
                    if (!status.HasError)
                    {
                        //BranchUser bu = new BranchUser
                        //{
                        //    BranchId = model.BranchId,
                        //    IdentityUserId = (int)status.IdentityUserId,
                        //    FromDate = DateTime.Now,
                        //    IsActive = true,

                        //};
                        //bu.AutoFill();
                        //await _branchService.CrudServiceBranchUser.InsertAsync<int>(bu);
                        var user = await _userManager.FindByEmailAsync(model.Email);
                        return HttpResponse(200, "Registration Successful", user);
                    }
                    else
                    {
                        return HttpResponse(400, status.Message, model);
                    }


                }
                else
                {
                    var data = model.To<AppUser>();
                    AppUser user = await _userService.AppUserCrudService.GetAsync("Where AppUserId=@AppUserId", new { AppUserId = data.AppUserId });
                    if (user != null)
                    {
                        data.AutoFill();
                        data.UserName = user.UserName;
                        await _userService.AppUserCrudService.UpdateAsync(data);
                        if (user.IdentityUserId > 0)
                        {//imported user dont't have any roles and ids
                            await _identityUserService.DeleteUserRoles(user.IdentityUserId);
                            await _identityUserService.AddUserRoles(model.Roles, user.IdentityUserId);
                        }

                        return HttpResponse(200, "User successfully updated", model);
                    }
                    else
                    {
                        return ErrorResponse(401, "Unauthorized Access");
                    }
                }

            }
            else
            {
                var d = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage).ToList();
                return HttpResponse(500, string.Join(',', d), "");
            }
        }
        [HttpPost]
        [Route("delete")]
        public async Task<dynamic> Delete(long id)
        {
            try
            {
                if (id == 1)
                {
                    return HttpResponse(400, "super user cannot be deleted", null);
                }
                int currentUserId = (int)User.Identity.GetIdentityUserId();
                if (id == currentUserId)
                {
                    return HttpResponse(400, "You cannot delete your own logged in user", null);
                }
                var user = await _userService.AppUserCrudService.GetAsync("Where IdentityUserId=@IdentityUserId", new { IdentityUserId = id });
                if (user == null)
                {
                    return HttpResponse(400, "No User Found", null);
                }
                var identityUser = await _identityUserService.UserService.GetAsync(user.IdentityUserId);
                user.IsDeleted = true;
                user.IsActive = false;
                await _userService.AppUserCrudService.UpdateAsDeleted(user.AppUserId);
                identityUser.LockoutEnabled = true;
                await _identityUserService.UserService.UpdateAsync(identityUser);

                return HttpResponse(200, "User successfully Deleted", null);
            }
            catch (Exception ex)
            {
                return HttpResponse(500, ex.Message, null);
            }

        }



        [Route("refresh/token")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<dynamic> GetRefreshToken(RefreshTokenViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var tokenResponse = await RequestRefreshToken(model.RefreshToken);
                    if (tokenResponse.IsError)
                        return Json(new { error = "invalid refresh token" });
                    return Json(new
                    {
                        id_token = tokenResponse.IdentityToken,
                        access_token = tokenResponse.AccessToken,
                        expires_in = tokenResponse.ExpiresIn,
                        token_type = tokenResponse.TokenType
                    });

                }
                else
                {
                    return Json(new { error = "invalid refresh token" });
                }

            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message, e);
                return Json(new { error = "server error" });
            }
        }





        private async Task<TokenResponse> RequestToken(string username, string password)
        {

            string siteUrl = _configuration["KachuwaAppConfig:TokenAuthority"].ToString();
            //var tokenClient = new TokenClient(new HttpClient{""}, );
            // var tokenResponse = await tokenClient.RequestPasswordTokenAsync(username, password, "", new { originrequest = "apps" });

            //if (tokenResponse.IsError) { /* Log failed login attempt! */ }


            var client = new HttpClient();

            var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = $"{siteUrl}/connect/token",
                ClientId = "ro.kachuwa.internal",
                UserName = username,
                Password = password

            });
            if (response.IsError)
            {

            }

            return response;
        }
        private async Task<TokenResponse> RequestRefreshToken(string refreshtoken)
        {

            string siteUrl = _configuration["KachuwaAppConfig:TokenAuthority"].ToString();
            var client = new HttpClient();
            var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = $"{siteUrl}/connect/token",
                ClientId = "ro.kachuwa.internal",
                RefreshToken = refreshtoken
            });
            return response;
        }

    }

}