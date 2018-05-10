using System.Threading.Tasks;
using Kachuwa.Data.Extension;
using Kachuwa.HtmlContent.Service;
using Kachuwa.Web;
using Kachuwa.Web.Model;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.HtmlContent.Controllers
{
    [Area("Admin")]
   //// [Authorize(PolicyConstants.PagePermission)]
    public class HtmlContentController : BaseController
    {
        private readonly IHtmlContentService _htmlContentService;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly Setting _webSetting;


        public HtmlContentController(IHtmlContentService htmlContentService,
            ISettingService settingService, INotificationService notificationService)
        {
            _htmlContentService = htmlContentService;
            _settingService = settingService;
            _notificationService = notificationService;
            _webSetting = _settingService.CrudService.Get(1);
        }

        #region Html Crud

        [Route("admin/html/page/{pageNo?}")]
        [Route("admin/html")]//default make it at last
        public async Task<IActionResult> Index([FromRoute]int pageNo = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = pageNo;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _htmlContentService.HtmlService.GetListPagedAsync(pageNo, rowsPerPage, 1,
                "Where KeyName like @Query and IsDeleted=0 and Culture=@Culture", "Addedon desc", new { Culture = _webSetting.BaseCulture, Query = "%" + query + "%" });
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
                {
                    await _htmlContentService.HtmlService.InsertAsync<int>(model);
                    _notificationService.Notify("Success", "Data has been saved successfully!", NotificationType.Success);
                    return RedirectToAction("Index");
                }
                return View(model);
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
                if (model.HtmlContentId != 0)
                {
                    await _htmlContentService.HtmlService.UpdateAsync(model);
                    _notificationService.Notify("Success", "Data has been saved successfully!", NotificationType.Success);
                    return RedirectToAction("Index");
                }
                return View(model);
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
            _notificationService.Notify("Success", "Data deleted successfully!", NotificationType.Success);
            return Json(result);
        }
        #endregion



    }
}