using System.Threading.Tasks;
using Kachuwa.Data.Extension;
using Kachuwa.Web;
using Kachuwa.Web.Model;
using Kachuwa.Web.Security;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        #region Menu Crud
        [Route("admin/menu/page/{page?}")]
        [Route("admin/menu")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int page = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = page;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _menuService.MenuCrudService.GetListPagedAsync(page, rowsPerPage, 1,
                "Where Name like @Query and IsDeleted=0", "Addedon desc", new { Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/menu/new")]
        public async Task<IActionResult> New()
        {

            return View();
        }

        [HttpPost]
        [Route("admin/menu/new")]
        public async Task<IActionResult> New(Menu model)
        {
            if (ModelState.IsValid)
            {
                // model.Url = model.Url.TrimStart(new char[] { '/' });
                model.AutoFill();
                if (model.MenuId == 0)
                    await _menuService.MenuCrudService.InsertAsync<int>(model);
                else
                    await _menuService.MenuCrudService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }


        [Route("admin/menu/edit/{pageId}")]
        public async Task<IActionResult> Edit([FromRoute]int pageId)
        {
            var model = await _menuService.MenuCrudService.GetAsync(pageId);
            return View(model);
        }

        [HttpPost]
        [Route("admin/menu/edit")]
        public async Task<IActionResult> Edit(Menu model)
        {
            if (ModelState.IsValid)
            {
                model.AutoFill();
                if (model.MenuId == 0)
                    await _menuService.MenuCrudService.InsertAsync<int>(model);
                else
                    await _menuService.MenuCrudService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [Route("admin/menu/delete")]
        public async Task<JsonResult> Delete(int id)
        {
            var result = await _menuService.MenuCrudService.DeleteAsync(id);
            return Json(result);
        }
        #endregion



    }
}