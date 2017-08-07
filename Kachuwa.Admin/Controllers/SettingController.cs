using System.Threading.Tasks;
using Kachuwa.Web;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}