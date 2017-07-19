using Kachuwa.HtmlContent.Service;
using Kachuwa.Log;
using Kachuwa.Web.Module;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.HtmlContent
{
    public class HtmlContentManageViewComponent : KachuwaModuleViewComponent<HtmlModule>
    {
        private readonly IHtmlContentService _service;
        private readonly ILogger _logger;

        public HtmlContentManageViewComponent(IHtmlContentService service, ILogger logger, IModuleManager moduleManager) : base(moduleManager)
        {
            _service = service;
            _logger = logger;
        }
        public IViewComponentResult Invoke()
        {
            _logger.Log(LogType.Info, () => "Invoking HtmlContentManage UsView");
            //var data = await _service.GetAll();
            return View();
        }
    }
}