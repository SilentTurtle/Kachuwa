using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.IdentityServerAdmin.Infrastructure;
using Kachuwa.IdentityServerAdmin.Model;
using Kachuwa.IdentityServerAdmin.Services;
using Kachuwa.IdentityServerAdmin.ViewModel;
using Kachuwa.Log;
using Kachuwa.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.IdentityServerAdmin.Areas.Admin.Controllers
{

    [Route("api-resource")]
    public partial class ApiResourceController : BaseController
    {
        private readonly IIdentityAdminService _identityAdminService;
        private readonly ILogger _logger;

        public ApiResourceController(
            IIdentityAdminService identityAdminService,
           ILogger logger)
        {
            _identityAdminService = identityAdminService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int pageNo = 1, int limit = 10, string query = "")
        {
            var queryable = await _identityAdminService.ApiResourcesService.GetListPagedAsync(pageNo, limit, 1, "Where Action like @Query", "AddedOn desc", new { Query = "%" + query + "%" });

            var ids = queryable.Select(x => x.Id).ToList();
            var claims = await _identityAdminService.ApiClaimsService.GetListAsync("where ApiResourceId in @Ids",
                  new { Ids = ids });

            var viewModel = new List<ListApiResourceItemViewModel>(queryable.Select(x =>
                    new ListApiResourceItemViewModel
                    {
                        Name = x.Name,
                        DisplayName = x.DisplayName,
                        Description = x.Description,
                        Enabled = x.Enabled,
                        Id = x.Id,
                        UserClaims = !claims.Any(y => y.ApiResourceId == x.Id)
                            ? ""
                            : string.Join(" ", claims.Where(y => y.ApiResourceId == x.Id).Select(ic => ic.Type))
                    }));
            return View(viewModel);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> ViewAsync(int id, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var resource = await _identityAdminService.ApiResourcesService.GetAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            var apiclaims = await _identityAdminService.ApiClaimsService.GetListAsync("where ApiResourceId=@ApiResourceId",
            new { ApiResourceId = id });
            var claims = apiclaims
                .Select(x => x.Type)
                .ToList();
            var viewModel = new ApiResourceViewModel
            {
                Name = resource.Name,
                Enabled = resource.Enabled,

                Description = resource.Description,
                DisplayName = resource.DisplayName,
                UserClaims = string.Join(" ", claims)
            };

            return View("View", viewModel);
        }


        #region UPDATE

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, string returnUrl, ApiResourceViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("View", dto);
            }


            var resource = await _identityAdminService.ApiResourcesService.GetAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            dto.Name = dto.Name.Trim();
            var nameExits = await _identityAdminService.ApiResourcesService.GetAsync("Where Id!=@Id and Name=@Name", new { Id = id, Name = dto.Name });
            if (nameExits != null)
            {
                ModelState.AddModelError(string.Empty, $"Resource name {dto.Name} already exists");
                return View("View", dto);
            }

            resource.Name = dto.Name;

            resource.Description = dto.Description?.Trim();
            resource.DisplayName =
                string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim();
            resource.Enabled = dto.Enabled;

            var oldClaims = await _identityAdminService.ApiClaimsService.GetListAsync("where ApiResourceId=@ApiResourceId",
                new { ApiResourceId = id });


            try
            {
                foreach (var claim in oldClaims)
                {
                    await _identityAdminService.ApiClaimsService.DeleteAsync(claim.Id);
                }


                var newClaims = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                var claims = new List<ApiClaims>();
                if (newClaims != null)
                {
                    var list = newClaims.ToList();
                    list.Sort();
                    foreach (var identityClaim in list)
                    {

                        await _identityAdminService.ApiClaimsService.InsertAsync(new ApiClaims
                        {
                            Type = identityClaim,
                            ApiResourceId = resource.Id
                        });
                    }
                }

                //resource.UserClaims = claims;
                //dbContext.ApiResources.Update(resource);

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index");
                }

                return Redirect(returnUrl);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => $"Update api resource failed: {e}");

                ModelState.AddModelError(string.Empty, "Update api resource failed");
                return View("View", dto);
            }
        }
        #endregion



        #region Scopes


        [HttpGet("{id}/scope")]
        public async Task<IActionResult> ScopeAsync(int id)
        {

            var resource = await _identityAdminService.ApiResourcesService.GetAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            var scopes = await _identityAdminService.ApiScopesService.GetListAsync("where ApiResourceId=@ApiResourceId",
                new { ApiResourceId = id });
            var ids = scopes.Select(x => x.Id);

            var apiClaims = await _identityAdminService.ApiScopeClaimsService.GetListAsync("where Id in @Ids",
                 new { Ids = ids });
            var claims = apiClaims
                .ToList()
                .GroupBy(x => x.ApiScopeId).ToDictionary(g => g.Key);

            var viewModel = new List<ListApiResourceScopeViewModel>();
            foreach (var scope in scopes)
            {
                viewModel.Add(new ListApiResourceScopeViewModel
                {
                    Id = scope.Id,
                    Name = scope.Name,
                    Description = scope.Description,
                    DisplayName = scope.DisplayName,
                    ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument,
                    Emphasize = scope.Emphasize,
                    Required = scope.Required,
                    UserClaims = claims.ContainsKey(scope.Id)
                        ? string.Join(" ", claims[scope.Id].Select(x => x.Type))
                        : string.Empty
                });
            }

            ViewData["ApiResourceId"] = id;

            return View("Scope", viewModel);
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("{id}/scope/create")]
        public async Task<IActionResult> CreateScopeAsync(int id)
        {
            var resource = await _identityAdminService.ApiResourcesService.GetAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            ViewData["ApiResourceId"] = id;
            return View("CreateScope", new ApiResourceScopeViewModel());
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("{id}/scope/create")]
        public async Task<IActionResult> CreateScopeAsync(int id, ApiResourceScopeViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateScope", dto);
            }

            var resource = await _identityAdminService.ApiResourcesService.GetAsync(id);
            if (resource == null)
            {
                return NotFound();
            }

            dto.Name = dto.Name.Trim();
            var scope = new ApiScopes
            {
                Name = dto.Name,
                Description = dto.Description?.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim(),
                Required = dto.Required,
                ShowInDiscoveryDocument = dto.ShowInDiscoveryDocument,
                Emphasize = dto.Emphasize,
                ApiResourceId = resource.Id
            };

            try
            {
                var claimTypes = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var claims = new List<ApiScopeClaims>();
                if (claimTypes != null)
                {
                    foreach (var claimType in claimTypes)
                    {

                        await _identityAdminService.ApiScopesService.InsertAsync(new ApiScopeClaims
                        {
                            Type = claimType,
                            ApiScopeId = scope.Id
                        });
                    }
                }




                return Redirect($"/api-resource/{id}/scope");
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => $"Add scope failed: {e}");

                ModelState.AddModelError(string.Empty, "Add scope failed");
                return View("CreateScope", dto);
            }
        }


        [HttpGet("{resourceId}/scope/{id}")]
        public async Task<IActionResult> ScopeAsync(int resourceId, int id)
        {
            var resource = await _identityAdminService.ApiResourcesService.GetAsync(resourceId);
            if (resource == null)
            {
                return BadRequest();
            }

            var scope = await _identityAdminService.ApiScopesService.GetAsync(resourceId);
            if (scope == null)
            {
                return BadRequest();
            }

            if (scope.ApiResourceId != resourceId)
            {
                return BadRequest();
            }

            var viewModel = new ApiResourceScopeViewModel
            {
                Name = scope.Name,
                Required = scope.Required,
                Emphasize = scope.Emphasize,
                Description = scope.Description,
                ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument,
                DisplayName = scope.DisplayName
            };

            var apiClaims = await _identityAdminService.ApiScopeClaimsService.GetListAsync("where ApiScopeId=@ApiScopeId",
                new { ApiScopeId = id });
            var claims = apiClaims
                .ToList()
                .Select(x => x.Type);
            viewModel.UserClaims = string.Join(" ", claims);
            ViewData["ApiResource"] = resource.Name;
            ViewData["ApiResourceId"] = resourceId;
            ViewData["ScopeId"] = id;
            return View("ViewScope", viewModel);
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("{resourceId}/scope/{id}")]
        public async Task<IActionResult> UpdateScopeAsync(int resourceId, int id, string returnUrl,
            ApiResourceScopeViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("ViewScope", dto);
            }

            var resource = await _identityAdminService.ApiResourcesService.GetAsync(resourceId);
            if (resource == null)
            {
                return BadRequest();
            }

            var scope = await _identityAdminService.ApiScopesService.GetAsync(id);
            if (scope == null)
            {
                return BadRequest();
            }

            if (scope.ApiResourceId != resourceId)
            {
                return BadRequest();
            }

            dto.Name = dto.Name.Trim();
            scope.Name = dto.Name;
            scope.Description = dto.Description?.Trim();
            scope.DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim();
            scope.Required = dto.Required;
            scope.ShowInDiscoveryDocument = dto.ShowInDiscoveryDocument;
            scope.Emphasize = dto.Emphasize;


            var oldClaims = await _identityAdminService.ApiScopeClaimsService.GetListAsync("where ApiScopeId=@ApiScopeId",
                new { ApiScopeId = scope.Id });


            try
            {
                foreach (var claim in oldClaims)
                {
                    await _identityAdminService.ApiScopeClaimsService.DeleteAsync(claim.Id);
                }

                var claimTypes = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var claims = new List<ApiScopeClaims>();
                if (claimTypes != null)
                {
                    var list = claimTypes.ToList();
                    list.Sort();
                    foreach (var claimType in list)
                    {

                        await _identityAdminService.ApiScopeClaimsService.InsertAsync(new ApiScopeClaims
                        {
                            Type = claimType,
                            ApiScopeId = scope.Id
                        });
                    }
                }

                return Redirect($"/api-resource/{resourceId}/scope");
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => $"Update scope failed: {e}");

                ModelState.AddModelError(string.Empty, "Update scope failed");
                return View("ViewScope", dto);
            }
        }
        #endregion

        #region CREATE

        [HttpGet("create")]
        public Task<IActionResult> CreateAsync(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return Task.FromResult((IActionResult)View("Create", new ApiResourceViewModel()));
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(string returnUrl, ApiResourceViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", dto);
            }

            dto.Name = dto.Name.Trim();
            var resourse = await _identityAdminService.ApiResourcesService.GetAsync("where Name=@Name", new { dto.Name });
            if (resourse != null)
            {
                ModelState.AddModelError("Name", "Name already exists");
                return View("Create", dto);
            }

            var identityResource = new ApiResources
            {
                Name = dto.Name,
                Description = dto.Description?.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim(),
                Enabled = dto.Enabled,

            };
            var apiResourceId = await _identityAdminService.ApiResourcesService.InsertAsync<int>(identityResource);

            await _identityAdminService.ApiScopesService.InsertAsync<int>(new ApiScopes
            {
                Name = dto.Name,
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim(),
                ApiResourceId = apiResourceId
            });

            try
            {
                var claims = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                if (claims != null)
                {
                    foreach (var identityClaim in claims)
                    {

                        await _identityAdminService.ApiClaimsService.InsertAsync<int>(new ApiClaims
                        {
                            Type = identityClaim,
                            ApiResourceId = apiResourceId
                        });
                    }
                }



                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index");
                }

                return Redirect(returnUrl);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => $"Create api resource failed: {e}");

                ModelState.AddModelError(string.Empty, "Create api resource failed");
                return View("Create", dto);
            }
        }

        #endregion
    }
}