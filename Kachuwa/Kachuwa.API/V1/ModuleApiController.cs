using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Data.Extension;
using Kachuwa.Extensions;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Kachuwa.Log;
using Kachuwa.Storage;
using Kachuwa.Web;
using Kachuwa.Web.API;
using Kachuwa.Web.Module;
using Kachuwa.Web.Service;
using Kachuwa.Web.ViewModels;
using Kachuwa.Web.ViewModels.Module;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.API.V1
{
    [Route("api/v1/module")]
    public class ModuleApiController : BaseApiController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IStorageProvider _storageProvider;
        private readonly IIdentityRoleService _identityRoleService;
        private readonly IPermissionService _permissionService;
        private readonly IModuleService _moduleService;
        private readonly IPageService _pageService;
        public readonly ILogger _logger;

        public ModuleApiController(IWebHostEnvironment hostingEnvironment, ILogger logger,
            IStorageProvider storageProvider, IIdentityRoleService identityRoleService,
            IPermissionService permissionService,IModuleService moduleService,IPageService pageService)
        {
            _hostingEnvironment = hostingEnvironment;
            _storageProvider = storageProvider;
            _identityRoleService = identityRoleService;
            _permissionService = permissionService;
            _moduleService = moduleService;
            _pageService = pageService;
            _logger = logger;
        }
        [Route("all/page")]
        public async Task<object> GetStateList()
        {
            try
            {
                var modules=await _moduleService.Service.GetListAsync();
                var pageWithModules = new List<ModuleWithPages>();
                foreach (var module in modules)
                {
                    var modulePages = module.To<ModuleWithPages>();
                    var pages = await _pageService.CrudService.GetListAsync("Where ModuleId=@ModuleId ",
                        new {ModuleId = module.ModuleId});
                    foreach (var page in pages)
                    {
                        var pageWIthPermission = page.To<PageWithPagePermission>();
                        modulePages.Pages.Add(pageWIthPermission);
                    }


                    pageWithModules.Add(modulePages);
                }

                return HttpResponse(200, "Success", pageWithModules);
            }
            catch (Exception ex)
            {

                _logger.Log(LogType.Error, () => ex.Message, ex);
                return HttpResponse(500, ex.Message, "");
            }
        }


    }
}