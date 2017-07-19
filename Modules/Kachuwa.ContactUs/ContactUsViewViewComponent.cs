using Kachuwa.Log;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.ContactUs
{
    public class ContactUsViewViewComponent : ViewComponent
    {
        private readonly ILogger _logger;

        public ContactUsViewViewComponent(ILogger logger)
        {
            _logger = logger;
        }
        public IViewComponentResult Invoke()
        {
            _logger.Log(LogType.Info, () => "Invoking Contact Us View");
            return View(new ContactUsInfo(){Name="Binod Tamang"});
        }
    }
}