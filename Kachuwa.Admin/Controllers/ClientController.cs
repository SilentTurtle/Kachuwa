using System.Threading.Tasks;
using Kachuwa.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Authorize()]
    public class ClientController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }


}