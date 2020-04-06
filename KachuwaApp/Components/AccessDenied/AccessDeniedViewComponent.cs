using System;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web;
using Microsoft.AspNetCore.Mvc;

namespace KachuwaApp.Components
{

    [ViewComponent(Name = "AccessDenied")]
    public class AccessDeniedViewComponent : KachuwaViewComponent
    {
        private readonly ILogger _logger;

        public AccessDeniedViewComponent(ILogger logger)
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
                _logger.Log(LogType.Error, () => "AccessDenied View Component error.", e);
                throw e;
            }

        }

        public override string DisplayName { get; } = "Access Denied";
        public override bool IsVisibleOnUI { get; } = true;
    }
}
