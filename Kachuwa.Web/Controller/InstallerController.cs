using System.Threading.Tasks;
using Kachuwa.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kachuwa.Web
{
    public class InstallerController : Controller
    {
        private readonly KachuwaAppConfig _kachuwaConfig;

        public InstallerController(IOptions<KachuwaAppConfig> kachuwaConfig)
        {
            _kachuwaConfig = kachuwaConfig.Value;
        }
        [Route("install")]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [Route("install/finished")]
        public async Task<IActionResult> Finish()
        {
            return View();
        }
    }
}