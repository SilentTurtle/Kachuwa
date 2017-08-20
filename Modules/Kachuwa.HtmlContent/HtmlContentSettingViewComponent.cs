using Kachuwa.HtmlContent.Service;
using Kachuwa.Log;
using Kachuwa.Web.Module;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.HtmlContent
{
    public class HtmlContentSettingViewComponent : KachuwaModuleViewComponent<HtmlModule>
    {
        private readonly IHtmlContentService _service;
        private readonly ILogger _logger;

        public HtmlContentSettingViewComponent(IHtmlContentService service, ILogger logger, IModuleManager moduleManager) : base(moduleManager)
        {
            _service = service;
            _logger = logger;
        }
        public IViewComponentResult Invoke()
        {
            _logger.Log(LogType.Info, () => "Invoking Html Setting Us View");
            //var data = await _service.GetAll();
            return View();
        }

        public override string DisplayName { get; } = "Html Content Setting";
        public override bool IsVisibleOnUI { get; } = false;
    }
}