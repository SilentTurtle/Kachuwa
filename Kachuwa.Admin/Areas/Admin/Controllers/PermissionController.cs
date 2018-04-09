using System.Threading.Tasks;
using Kachuwa.Web;
using Kachuwa.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class PermissionController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}