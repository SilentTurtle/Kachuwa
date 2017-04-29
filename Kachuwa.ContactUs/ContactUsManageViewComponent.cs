using System.Threading.Tasks;
using Kachuwa.Log;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.ContactUs
{
    public class ContactUsManageViewComponent : ViewComponent
    {
        private readonly IContactUsService _service;
        private readonly ILogger _logger;

        public ContactUsManageViewComponent(IContactUsService service,ILogger logger)
        {
            _service = service;
            _logger = logger;
        }
        public IViewComponentResult Invoke()
        {
            _logger.Log(LogType.Info, () => "Invoking Contact UsView");
            //var data = await _service.GetAll();
            return View();
        }
    }
}