using System;
using System.Threading.Tasks;
using Kachuwa.Log;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web.Components
{
    [ViewComponent(Name = "SEO")]
    public class SeoViewComponent : ViewComponent
    {
        private readonly ILogger _logger;
        private readonly ISeoService _seoService;

        public SeoViewComponent(ILogger logger)
        {
            _logger = logger;
            //_seoService = seoService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
               
                return View();
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => "SeoViewComponent loading error.", e);
                throw e;
            }
         
        }

    }
}