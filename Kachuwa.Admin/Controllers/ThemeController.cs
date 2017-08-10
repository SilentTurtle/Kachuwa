using System.Threading.Tasks;
using Kachuwa.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ThemeController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }


}