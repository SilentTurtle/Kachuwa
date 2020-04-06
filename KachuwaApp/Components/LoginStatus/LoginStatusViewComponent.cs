using System;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web;
using Microsoft.AspNetCore.Mvc;

namespace TixalayaApp.Components
{

    [ViewComponent(Name = "LoginStatus")]
    public class LoginStatusViewComponent : KachuwaViewComponent
    {
        private readonly ILogger _logger;

        public LoginStatusViewComponent(ILogger logger)
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
                _logger.Log(LogType.Error, () => "Now Login Status ViewComponent error.", e);
                throw e;
            }

        }

        public override string DisplayName { get; } = "Login Status Top Bar";
        public override bool IsVisibleOnUI { get; } = true;
    }
}
