using System.Threading.Tasks;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.HtmlContent.Service;
using Kachuwa.Web;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.HtmlContent.Controllers
{
    [Area("Admin")]
    public class HtmlContentController : BaseController
    {
        private readonly IHtmlContentService _htmlContentService;

        public HtmlContentController(IHtmlContentService  htmlContentService)
        {
            _htmlContentService = htmlContentService;
        }

        #region Html Crud

        [Route("admin/html/page/{page?}")]
        [Route("admin/html")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int page = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = page;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _htmlContentService.HtmlService.GetListPagedAsync(page, rowsPerPage, 1,
                "Where KeyName like @Query and IsDeleted=0", "Addedon desc", new { Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/html/new")]
        public async Task<IActionResult> New()
        {

            return View();
        }

        [HttpPost]
        [Route("admin/html/new")]
        public async Task<IActionResult> New(Model.HtmlContent model)
        {
            if (ModelState.IsValid)
            {
              
                model.AutoFill();
                if (model.HtmlContentId == 0)
                    await _htmlContentService.HtmlService.InsertAsync<int>(model);
                else
                    await _htmlContentService.HtmlService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }


        [Route("admin/html/edit/{htmlContentId}")]
        public async Task<IActionResult> Edit([FromRoute]int htmlContentId)
        {
            var model = await _htmlContentService.HtmlService.GetAsync(htmlContentId);
            return View(model);
        }

        [HttpPost]
        [Route("admin/html/edit")]
        public async Task<IActionResult> Edit(Model.HtmlContent model)
        {
            if (ModelState.IsValid)
            {
            
                model.AutoFill();
                if (model.HtmlContentId == 0)
                    await _htmlContentService.HtmlService.InsertAsync<int>(model);
                else
                    await _htmlContentService.HtmlService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [Route("admin/html/delete")]
        public async Task<JsonResult> Delete(int id)
        {
            var result = await _htmlContentService.HtmlService.DeleteAsync(id);
            return Json(result);
        }
        #endregion



    }
}