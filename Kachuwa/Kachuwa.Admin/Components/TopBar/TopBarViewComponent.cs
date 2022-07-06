using System;
using System.Threading.Tasks;
using Kachuwa.Log;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Components
{
    public class TopBarViewComponent : ViewComponent
    {
        private readonly ILogger _logger;

        public TopBarViewComponent(ILogger logger) 
        {
          
            _logger = logger;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => "Top bar loading error.", e);
                throw e;
            }
         
        }

    }
}