using System;
using System.Threading.Tasks;
using Kachuwa.Web;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : BaseController
    {
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