using System.Threading.Tasks;
using Kachuwa.Web;
using Kachuwa.Web.Security;
using Kachuwa.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(PolicyConstants.PagePermission)]
    public class AuditController : BaseController
    {
        private readonly IAuditService _auditService;
        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }
        [Route("admin/audit/page/{pageNo?}")]
        [Route("admin/audit")]
        public async Task<IActionResult> Index([FromRoute]int pageNo = 1, [FromQuery]string query = "")
        {
            ViewData["Page"] = pageNo;
            int rowsPerPage = 10;
            //customized viewmodel with join
            var model = await _auditService.CrudService.GetListPagedAsync(pageNo, rowsPerPage,1, "Where Action like @Query", "AddedOn desc", new { Query = "%" + query + "%" });
            return View(model);
        }
    }
}