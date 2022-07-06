using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Caching;
using Kachuwa.Core;
using Kachuwa.Data;
using Kachuwa.Data.Extension;
using Kachuwa.Web.Layout;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Kachuwa.Extensions;
using Kachuwa.Web.Model;
using Kachuwa.Web.ViewModels;

namespace Kachuwa.Web
{
    public class PageService : IPageService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILayoutRenderer _layoutRenderer;
        private readonly ISeoService _seoService;
        private readonly ICacheService _cacheService;

        public PageService(IWebHostEnvironment hostingEnvironment, ILayoutRenderer layoutRenderer, ISeoService seoService
            , ICacheService cacheService)
        {
            _hostingEnvironment = hostingEnvironment;
            _layoutRenderer = layoutRenderer;
            _seoService = seoService;
            _cacheService = cacheService;
        }
        public CrudService<Page> CrudService { get; set; } = new CrudService<Page>();
        public CrudService<PagePermission> PermissionCrudService { get; set; }

        public async Task<bool> CheckPageExist(string url)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result = await db.QueryAsync<int>("Select 1 from Page Where IsActive=@isActive and IsDeleted= @isDeleted and URL='@URL'", new { isActive = true, isDeleted = false, URL = url });
                return result != null && (result.SingleOrDefault() == 1 ? true : false);
            }
        }

        public string GetPageNamespaces(bool includeMasterLayout)
        {
            string viewImportsPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Views\\_ViewImports.cshtml");
            string viewStartPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Views\\_ViewStart.cshtml");

            if (File.Exists(viewImportsPath))
            {
                string fileContent = File.ReadAllText(viewImportsPath);
                if (includeMasterLayout)
                {

                    if (File.Exists(viewStartPath))
                    {
                        fileContent += "\n";
                           fileContent += File.ReadAllText(viewStartPath);
                    }
                }
                return fileContent;
            }
            return "";
        }
        public async Task<PageViewModel> Get(int pageId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result =
                    await db.QueryFirstAsync<PageViewModel>(
                        "select p.PageId,p.Name,p.Url,p.UseMasterLayout,p.IsBackend,p.IsActive,p.IsPublished,s.SEOId,s.MetaDescription,s.MetaTitle,s.Image from Page as p left join Seo as s on p.PageId=s.PageId and s.SeoType='page' where  p.IsDeleted = @IsDeleted and p.PageId = @PageId",
                        new { IsDeleted = false, PageId = pageId });
                return result;
            }
        }
        public async Task<bool> Save(PageViewModel model)
        {
            try
            {
                var dbfactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbfactory.GetConnection())
                {
                    await db.OpenAsync();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            if (model.PageId == 0)
                            {
                                var seo = model.To<SEO>();
                                seo.Url = model.Url;
                                seo.PageName = model.Name;
                                var page = new Page()
                                {
                                    PageId = model.PageId,
                                    Name = model.Name,
                                    Url = model.Url,
                                    IsActive = model.IsActive,
                                    IsPublished = model.IsPublished,
                                    UseMasterLayout = model.UseMasterLayout
                                };
                                page.AutoFill();
                                int pageId = await CrudService.InsertAsync<int>(db, page, tran, 30);
                                seo.AutoFill();
                                seo.PageId = pageId;
                                seo.Url = model.Url.StartsWith("/") ? model.Url : "/" + model.Url;
                                int seoId = await _seoService.Seo.InsertAsync<int>(db, seo, tran, 30);
                                //return newProductId;
                                model.PageId = pageId;
                            }
                            else
                            {
                                var page = new Page()
                                {
                                    PageId = model.PageId,
                                    Name = model.Name,
                                    Url = model.Url,
                                    IsActive = model.IsActive,
                                    IsPublished = model.IsPublished,
                                    UseMasterLayout = model.UseMasterLayout
                                };
                                page.AutoFill();
                                await CrudService.UpdateAsync(db, page, tran, 30);
                                var seo = model.To<SEO>();
                                seo.Url = model.Url.StartsWith("/") ? model.Url : "/" + model.Url;
                                seo.LastUrl = model.Url != model.OldUrl ? model.OldUrl : model.Url;
                                seo.AutoFill();
                                seo.PageId = (int)model.PageId;
                                seo.PageName = page.Name;
                                if (seo.SEOId == 0)
                                    await _seoService.Seo.InsertAsync<int>(db, seo, tran, 30);
                                else
                                    await _seoService.Seo.UpdateAsync(db, seo, tran, 30);
                            }
                            PageRolePermissionViewModel rolePermissionModel = new PageRolePermissionViewModel();
                            rolePermissionModel.PageId = (int)model.PageId;
                            rolePermissionModel.PagePermissionsRole = model.PagePermissionsRole;
                            await AddUpdatePagePermission(rolePermissionModel);
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> SavePageLayout(LayoutContent content)
        {
            var renderedContent = _layoutRenderer.Render(content, LayoutGridSystem.BootStrap);
            var jsonContent = JsonConvert.SerializeObject(content);

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result =
                    await db.ExecuteAsync(
                        "Update Page Set Content=@Content,ContentConfig=@ContentConfig Where PageId=@PageId",
                        new { Content = renderedContent, ContentConfig = jsonContent, PageId = content.PageId });
                return true;
            }
        }
        public async Task<bool> DeletePageAsync(long pageId)
        {
            var page = await CrudService.GetAsync(pageId);
            if (page.Url.ToLower() == "landing")
            {
                throw new Exception("unable to use this url.enter another url.");
            }
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result = await db.ExecuteAsync("Update Page Set IsDeleted=@IsDeleted, IsActive=@IsActive Where PageId=@PageId", new { IsActive = false, IsDeleted = true, PageId = pageId });
                var seoresult = await db.ExecuteAsync("Update Seo Set IsDeleted=@IsDeleted, IsActive=@IsActive Where  Url=@Url", new { IsActive = false, IsDeleted = true, Url = page.Url });
                return true;
            }

        }

        public async Task<bool> MakeLandingPage(long pageId)
        {
            var page = await CrudService.GetAsync(pageId);
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result = await db.ExecuteAsync(" Update Page set Url=@RandomUrl Where Url='landing'; " +
                                                   " Update Page set Url='landing',IsDeleted=false, IsActive=true,Ispublished=Ispublished " +
                                                   " Where PageId=@PageId", new
                                                   {
                                                       RandomUrl = "landing-" + new Random().Next(5000, 99999),
                                                       IsDeleted = false,
                                                       Ispublished = true,
                                                       PageId = pageId
                                                   });
                return true;
            }
        }

        public async Task<IEnumerable<PagePermissionViewModel>> GetPermissionsFromCache()
        {
            var permissions = await _cacheService.GetAsync<IEnumerable<PagePermissionViewModel>>(CacheKeys.PagePemissions, async () => await GetAllPermissions(), TimeSpan.FromMinutes(1));
            return permissions;
        }

        public async Task<IEnumerable<PagePermissionViewModel>> GetAllPermissions()
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<PagePermissionViewModel>("select pp.*,p.Name,p.Url,ir.Name as RoleName from pagepermission as pp " +
                                                                    " inner join Page as p on p.PageId=pp.PageId " +
                                                                    " left join IdentityRole as ir on pp.RoleId=ir.Id ;");
            }
        }

        public async Task<IEnumerable<PagePermissionViewModel>> GetPagePermission(int pageId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                if (pageId == 0)
                {
                    return await db.QueryAsync<PagePermissionViewModel>(
                        @"Select Name as RoleName, Id as RoleId From IdentityRole",
                        new { PageId = pageId });
                }
                else
                {
                    return await db.QueryAsync<PagePermissionViewModel>(
                                           @"Select ir.Name AS RoleName, ir.Id AS RoleId, pp.PagePermissionId, pp.PageId, pp.AllowAccessForAll, pp.AllowAccess
                                            From IdentityRole ir Left Join PagePermission pp on ir.Id = pp.RoleId AND pp.PageId = @PageId",
                                           new { PageId = pageId });
                }
            }
        }

        public async Task<bool> AddUpdatePagePermission(PageRolePermissionViewModel models)
        {
            try
            {
                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    await db.ExecuteAsync("Delete From PagePermission Where PageId = @PageId",
                        new { PageId = models.PageId });
                    models.AutoFill();
                    foreach (var pagePermissionRoleId in models.PagePermissionsRole)
                    {
                        await db.ExecuteAsync("Insert into PagePermission(PageId, AllowAccessForAll, AllowAccess, RoleId, AddedBy) Values(@PageId, @AllowAccessForAll, @AllowAccess, @RoleId, @AddedBy);", new
                        {
                            PageId = models.PageId,
                            AllowAccessForAll = false,
                            AllowAccess = true,
                            RoleId = pagePermissionRoleId,//pagePermission.RoleId,
                            AddedBy = models.AddedBy
                        });
                    }

                    _cacheService.Remove(CacheKeys.PagePemissions);

                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
