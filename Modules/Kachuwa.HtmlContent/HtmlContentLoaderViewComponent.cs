using System;
using System.Threading.Tasks;
using Kachuwa.HtmlContent.Service;
using Kachuwa.Log;
using Kachuwa.Web.Module;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.HtmlContent
{
    public class HtmlContentLoaderViewComponent : KachuwaModuleViewComponent<HtmlModule>
    {
        private readonly IHtmlContentService _service;
        private readonly ILogger _logger;

        public HtmlContentLoaderViewComponent(IHtmlContentService service, ILogger logger, IModuleManager moduleManager) : base(moduleManager)
        {
            _service = service;
            _logger = logger;
        }
        public async Task<IViewComponentResult> InvokeAsync(string key)
        {
            try
            {
                var model = await _service.HtmlService.GetAsync("Where KeyName=@Key",new {Key=key});
                if (model != null)
                    return View(model);
                else
                    return View(new Model.HtmlContent());
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => "HtmlContent loader error.", e);
            }
            return View(key);
        }
      
    }
}