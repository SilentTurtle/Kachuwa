using System;
using System.Threading.Tasks;
using Kachuwa.HtmlContent.Service;
using Kachuwa.Log;
using Kachuwa.Web.Module;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.HtmlContent
{
    public class HtmlContentLoaderViewComponent : KachuwaModuleViewComponent<HtmlModule>
    {
        private readonly IHtmlContentService _service;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;

        public HtmlContentLoaderViewComponent(IHtmlContentService service,ISettingService settingService,  ILogger logger, IModuleManager moduleManager) : base(moduleManager)
        {
            _service = service;
            _settingService = settingService;
            _logger = logger;
        }
        public async Task<IViewComponentResult> InvokeAsync(string key)
        {
            try
            {
                var setting=await _settingService.CrudService.GetAsync(1);
                var model = await _service.HtmlService.GetAsync("Where KeyName=@Key and Culture=@Culture", new
                {
                    Key = key
                    ,
                    Culture = setting.BaseCulture
                });
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

        public override string DisplayName { get; } = "Html Content Loder";
        public override bool IsVisibleOnUI { get; } = true;
    }
}