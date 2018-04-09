using System;
using System.Threading.Tasks;
using Kachuwa.Web;
using Kachuwa.Web.Security;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{

    [Area("Admin")]
    [Authorize]
    public class DashboardController : BaseController
    {
        [Authorize(PolicyConstants.PagePermission)]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
    public class LibraryDetails
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string BookName { get; set; }
        public string Category { get; set; }

    }
}