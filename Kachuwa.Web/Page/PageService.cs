using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Web.Layout;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Kachuwa.Extensions;
using Kachuwa.Web.Model;

namespace Kachuwa.Web
{
    public class PageService : IPageService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILayoutRenderer _layoutRenderer;
        private readonly ISeoService _seoService;

        public PageService(IHostingEnvironment hostingEnvironment, ILayoutRenderer layoutRenderer, ISeoService seoService)
        {
            _hostingEnvironment = hostingEnvironment;
            _layoutRenderer = layoutRenderer;
            _seoService = seoService;
        }
        public CrudService<Page> CrudService { get; set; } = new CrudService<Page>();
        public async Task<bool> CheckPageExist(string url)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result = await db.QueryAsync<int>("Select 1 from Page Where IsActive=@isActive and IsDeleted= @isDeleted and URL=@URL", new { isActive=true, isDeleted =false, URL = url });
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
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result =
                    await db.QueryFirstAsync<PageViewModel>(
                        "select p.PageId,p.Name,p.Url,p.UseMasterLayout,p.IsActive,p.IsPublished,s.SEOId,s.MetaDescription,s.MetaTitle,s.Image from Page as p left join Seo as s on p.PageId=s.PageId and s.SeoType='page' where  p.IsDeleted = true and p.PageId = @PageId",
                        new { PageId = pageId });
                return result;
            }
        }
        public async Task<bool> Save(PageViewModel model)
        {
            try
            {

                var dbfactory = DbFactoryProvider.GetFactory();
                using (var db = (SqlConnection)dbfactory.GetConnection())
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
                                seo.Url = model.Url;
                                seo.LastUrl = model.Url != model.OldUrl ? model.OldUrl : model.Url;
                                seo.AutoFill();
                                seo.PageId = (int)model.PageId;
                                seo.PageName = page.Name;
                                if (seo.SEOId == 0)
                                    await _seoService.Seo.InsertAsync<int>(db, seo, tran, 30);
                                else
                                    await _seoService.Seo.UpdateAsync(db, seo, tran, 30);

                            }
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
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result =
                    await db.ExecuteAsync(
                        "Update Page Set Content=@Content,ContentConfig=@ContentConfig Where PageId=@PageId",
                        new { Content = renderedContent, ContentConfig = jsonContent, PageId = content.PageId });
                return true;
            }
        }

        public async Task<bool> DeletePageAsync(int pageId)
        {
            var page = await CrudService.GetAsync(pageId);
            if (page.Url.ToLower() == "landing")
            {

                throw new Exception( "unable to use this url.enter another url.");
            }
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result = await db.ExecuteAsync("Update Page Set IsDeleted=true, IsActive=false Where PageId=@id", new { id = pageId });
                var seoresult = await db.ExecuteAsync("Update Seo Set IsDeleted=true, IsActive=falseWhere  Url=@Url", new { Url = page.Url });
                return true;
            }

        }

        public async Task<bool> MakeLandingPage(int pageId)
        {
            var page = await CrudService.GetAsync(pageId);
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (SqlConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var result = await db.ExecuteAsync("Update Page set IsDeleted=true, IsActive=false,Ispublished=false Where Url='landing';  Update Page set Url='landing',IsDeleted=false, IsActive=true,Ispublished=true  Where PageId=@id", new { id = pageId });
                return true;
            }
        }
    }
}
