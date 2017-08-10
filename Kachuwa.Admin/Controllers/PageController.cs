using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PageController : BaseController
    {
        private readonly IPageService _pageService;

        public PageController(IPageService pageService)
        {
            _pageService = pageService;
        }
        #region PAge Crud
        [Route("admin/page/page/{page?}")]
        [Route("admin/page")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int page = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = page;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _pageService.CrudService.GetListPagedAsync(page, rowsPerPage, 1,
                "Where Name like @Query and IsDeleted=0", "Addedon desc", new { Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/page/new")]
        public async Task<IActionResult> New()
        {

            return View();
        }

        [HttpPost]
        [Route("admin/page/new")]
        public async Task<IActionResult> New(Page model)
        {
            if (ModelState.IsValid)
            {
                model.Url= model.Url.TrimStart(new char[] {'/'});
                model.AutoFill();
                if (model.PageId == 0)
                    await _pageService.CrudService.InsertAsync<int>(model);
                else
                    await _pageService.CrudService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }


        [Route("admin/page/edit/{pageId}")]
        public async Task<IActionResult> Edit([FromRoute]int pageId)
        {
            var model = await _pageService.CrudService.GetAsync(pageId);
            return View(model);
        }

        [HttpPost]
        [Route("admin/page/edit")]
        public async Task<IActionResult> Edit(Page model)
        {
            if (ModelState.IsValid)
            {
                model.Url = model.Url.TrimStart(new char[] { '/' });
                model.AutoFill();
                if (model.PageId == 0)
                    await _pageService.CrudService.InsertAsync<int>(model);
                else
                    await _pageService.CrudService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [Route("admin/page/delete")]
        public async Task<JsonResult> Delete(int id)
        {
            var result = await _pageService.CrudService.DeleteAsync(id);
            return Json(result);
        }
        #endregion



    }

}