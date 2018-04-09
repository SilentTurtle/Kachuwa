using System;
using System.Threading.Tasks;
using Kachuwa.Configuration;
using Kachuwa.Installer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Kachuwa.Web
{
    public class InstallerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IKachuwaConfigurationManager _kachuwaManager;
        private readonly KachuwaAppConfig _kachuwaConfig;

        public InstallerController(IConfiguration configuration, IOptionsSnapshot<KachuwaAppConfig> kachuwaConfig, IKachuwaConfigurationManager kachuwaManager)
        {
            _configuration = configuration;
            _kachuwaManager = kachuwaManager;
            _kachuwaConfig = kachuwaConfig.Value;
        }
        [Route("install")]
        public async Task<IActionResult> Index()
        {
            var model = new InstallerViewModel();

            return PartialView("_Installer", model);
        }
        [Route("install/try")]
        public async Task<IActionResult> TryTest()
        {
            string connectionString = _configuration.GetSection("TestConnection").ToString();

            if (await _kachuwaManager.Install(connectionString))
            {
                return Redirect("/");
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        [Route("install")]
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<JsonResult> Install(InstallerViewModel model)
        {
            try
            {

                var connectionString = model.ToString();
                if (await _kachuwaManager.Install(connectionString))
                {
                    return Json(new {Code = 200, Data = model, Message = "Installed Successfully."});
                }
                else
                {
                    return Json(new {Code = 500, Data = model, Message = "Installation Error."});
                }
            }
            catch (DivideByZeroException e)
            {
                return Json(new { Code = 500, Data = model, Message = "Connection String Error." });
            }
            catch (Exception e)
            {
                return Json(new {Code = 500, Data = model, Message = e.Message});
            }
        }

        [Route("install/checkconnection")]
        public async Task<JsonResult> CheckConnection(InstallerViewModel model)
        {
            try
            {
                var connectionString = model.ToString();
                if (await _kachuwaManager.CheckConnection(connectionString))
                {
                    return Json(new { Code = 200, Data = model, Message = "Tested Successfully." });
                }
                else
                {
                    return Json(new { Code = 500, Data = model, Message = "Test Error." });
                }
            }
            catch (Exception e)
            {
                return Json(new { Code = 500, Data = model, Message = e.Message });
            }
        }


        [Route("install/finished")]
        public async Task<IActionResult> Finish()
        {
            return View();
        }
    }
}