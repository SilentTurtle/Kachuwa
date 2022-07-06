using Kachuwa.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Data.Extension;
using Kachuwa.Extensions;
using Kachuwa.Identity.Extensions;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Kachuwa.Log;
using Kachuwa.Storage;
using Kachuwa.Web.API;
using Kachuwa.Web.Service;
using Kachuwa.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.API.V1
{
    [Route("api/v1/role")]
    public class RoleApiController : BaseApiController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IStorageProvider _storageProvider;
        private readonly IIdentityRoleService _identityRoleService;
        private readonly IPermissionService _permissionService;
        public readonly ILogger _logger;

        public RoleApiController(IWebHostEnvironment hostingEnvironment, ILogger logger,
            IStorageProvider storageProvider, IIdentityRoleService identityRoleService,
            IPermissionService permissionService)
        {
            _hostingEnvironment = hostingEnvironment;
            _storageProvider = storageProvider;
            _identityRoleService = identityRoleService;
            _permissionService = permissionService;
            _logger = logger;
        }
        [HttpGet]
        [Route("listactive")]
        public async Task<dynamic> ListActive()
        {
            try
            {
                var data = await _identityRoleService.RoleService.GetListAsync();
                return HttpResponse(200, "", data);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ExceptionResponse(e, "");
            }

        }
        [Route("all")]
        public async Task<object> GetAllRoles(int offset = 1, int limit = 10)
        {
            try
            {
                var roles = await _identityRoleService.RoleService.GetListPagedAsync(1, 10, 1, "Where IsDeleted=@IsDeleted", "AddedOn desc", new { IsDeleted = false });
                return HttpResponse(200, "Success", roles);
            }
            catch (Exception ex)
            {

                _logger.Log(LogType.Error, () => ex.Message, ex);
                return HttpResponse(500, ex.Message, "");
            }
        }

        [Route("new")]
        [HttpPost]
        public async Task<object> SaveNewRole(RoleWithPagePermission model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = new IdentityRole
                    {
                        Name = model.Name,

                    };
                    role.AutoFill();
                    if (!await _identityRoleService.CheckNameExist(model.Name))
                    {
                        var roleId = await _identityRoleService.RoleService.InsertAsync<int>(role);
                        model.Id = roleId;
                        await _permissionService.SaveRolePermissions(model);
                        return HttpResponse(200, "Success", true);
                    }
                    else
                    {
                        return HttpResponse(600, "Same name role already exist.");
                    }
                }
                else
                {
                    return ValidationResponse(ModelState.Select(x => x.Key).ToList());
                }

            }
            catch (Exception ex)
            {

                _logger.Log(LogType.Error, () => ex.Message, ex);
                return HttpResponse(500, ex.Message, "");
            }
        }


        [Route("permission")]
        public async Task<object> GetRolePermission(int roleId)
        {
            try
            {
                var permissions = await _permissionService.RoleMasterPermissionCrudService.GetListAsync("Where RoleId=@RoleId", new { RoleId = roleId });
                return HttpResponse(200, "Success", permissions);
            }
            catch (Exception ex)
            {

                _logger.Log(LogType.Error, () => ex.Message, ex);
                return HttpResponse(500, ex.Message, "");
            }
        }

        [Route("permission/save")]
        [HttpPost]
        public async Task<object> SavePermissionOnly(UpdateRoleWithPagePermission model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    model.AddedBy = User.Identity.GetIdentityUserId();
                    await _permissionService.UpdateRolePermissionsOnly(model);
                    return HttpResponse(200, "Success", true);

                }
                else
                {
                    return ValidationResponse(ModelState.Select(x => x.Key).ToList());
                }

            }
            catch (Exception ex)
            {

                _logger.Log(LogType.Error, () => ex.Message, ex);
                return HttpResponse(500, ex.Message, "");
            }
        }
        [Route("edit")]
        [HttpPost]
        public async Task<object> UpdateRole(RoleWithPagePermission model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityRole role = new IdentityRole
                    {
                        Id = model.Id,
                        IsTemporary = model.IsTemporary,
                        Name = model.Name,
                    };
                    role.AutoFill();
                    await _identityRoleService.RoleService.UpdateAsync(role);
                    return HttpResponse(200, "Success", true);
                }
                else
                {
                    return ValidationResponse(ModelState.Select(x => x.Key).ToList());
                }

            }
            catch (Exception ex)
            {

                _logger.Log(LogType.Error, () => ex.Message, ex);
                return HttpResponse(500, ex.Message, "");
            }
        }

        [Route("delete")]
        [HttpPost]
        public async Task<object> DeleteRole(IdentityRole model)
        {
            try
            {
                if (model.IsSystem)
                {

                    return HttpResponse(600, "System roles can not be deleted.");
                }
                else
                {
                    await _permissionService.DeleteRolePermissions((int)model.Id, User.Identity.GetIdentityUserId());
                    await _identityRoleService.RoleService.UpdateAsDeleted("Where Id=@RoleId",
                        new { RoleId = model.Id, DeletedBy = User.Identity.GetIdentityUserId() });
                    return HttpResponse(200, "Success", true);
                }

            }
            catch (Exception ex)
            {

                _logger.Log(LogType.Error, () => ex.Message, ex);
                return HttpResponse(500, ex.Message, "");
            }
        }

    }

}
