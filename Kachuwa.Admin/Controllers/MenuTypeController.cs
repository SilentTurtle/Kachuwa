using System.Threading.Tasks;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Web;
using Kachuwa.Web.Model;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    public class MenuTypeController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenuTypeController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        #region Menu Type Crud
        [Route("admin/menu/type/page/{page?}")]
        [Route("admin/menu/type")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int page = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = page;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _menuService.TypeCrudService.GetListPagedAsync(page, rowsPerPage, 1,
                "Where Name like @Query and IsDeleted=0", "Addedon desc", new { Query = "%" + query + "%" });
            return View(model);
        }

        [Route("admin/menu/type/new")]
        public async Task<IActionResult> New()
        {

            return View();
        }

        [HttpPost]
        [Route("admin/menu/type/new")]
        public async Task<IActionResult> New(MenuType model)
        {
            if (ModelState.IsValid)
            {
                // model.Url = model.Url.TrimStart(new char[] { '/' });
                model.AutoFill();
                if (model.MenuTypeId == 0)
                    await _menuService.TypeCrudService.InsertAsync<int>(model);
                else
                    await _menuService.TypeCrudService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }


        [Route("admin/menu/type/edit/{pageId}")]
        public async Task<IActionResult> Edit([FromRoute]int pageId)
        {
            var model = await _menuService.TypeCrudService.GetAsync(pageId);
            return View(model);
        }

        [HttpPost]
        [Route("admin/menu/type/edit")]
        public async Task<IActionResult> Edit(MenuType model)
        {
            if (ModelState.IsValid)
            {
                model.AutoFill();
                if (model.MenuTypeId == 0)
                    await _menuService.TypeCrudService.InsertAsync<int>(model);
                else
                    await _menuService.TypeCrudService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [Route("admin/menu/type/delete")]
        public async Task<JsonResult> Delete(int id)
        {
            var result = await _menuService.TypeCrudService.DeleteAsync(id);
            return Json(result);
        }
        #endregion



    }
   
}