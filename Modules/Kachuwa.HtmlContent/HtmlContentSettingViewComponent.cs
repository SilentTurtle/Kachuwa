using System.Threading.Tasks;
using Kachuwa.HtmlContent.Service;
using Kachuwa.Log;
using Kachuwa.Web.Model;
using Kachuwa.Web.Module;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.HtmlContent
{
    public class HtmlContentSettingViewComponent : KachuwaModuleViewComponent<HtmlModule>
    {
        private readonly IHtmlContentService _service;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly Setting _webSetting;

        public HtmlContentSettingViewComponent(IHtmlContentService service,ISettingService settingService, ILogger logger, IModuleManager moduleManager) : base(moduleManager)
        {
            _service = service;
            _settingService = settingService;
            _webSetting = _settingService.CrudService.Get(1);
            _logger = logger;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            var model =
                await _service.HtmlService.GetListAsync("Where IsDeleted=0 and Culture=@Culture Order By Addedon desc",
                    new
                    {
                        Culture = _webSetting.BaseCulture
                    });


            return View(model);
        }

        public override string DisplayName { get; } = "Html Content Setting";
        public override bool IsVisibleOnUI { get; } = false;
    }
}