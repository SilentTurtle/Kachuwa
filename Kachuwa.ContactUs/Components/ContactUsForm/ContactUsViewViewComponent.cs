using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web.Module;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.ContactUs
{
    public class ContactUsFormViewComponent :  KachuwaModuleViewComponent<ContactUsModule>
    {
        private readonly IContactUsService _service;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;

         public ContactUsFormViewComponent(IContactUsService service, ISettingService settingService, ILogger logger, IModuleManager moduleManager) : base(moduleManager)
         {
             _service = service;
             _settingService = settingService;
             _logger = logger;
         }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            return View();
        }

        public override string DisplayName { get; } = "Contact Us Form";
        public override bool IsVisibleOnUI { get; } = true;
    }
}