using System;
using System.Threading.Tasks;
using Kachuwa.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Admin
{
   
    [Area("Admin")]
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