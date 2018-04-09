using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MXTires.Microdata.Core.Actions.TransferActions;

namespace Kachuwa.Web.Module
{
    public class KachuwaModuleController<T> : BaseController where T : IModule, new()
    {
        private readonly IModuleManager _moduleManager;
        private readonly IModule _module;
        protected KachuwaModuleController()
        {

            _moduleManager= ContextResolver.Context.RequestServices.GetService<IModuleManager>();
            _module = new T();
            _module = _moduleManager.FindAsync(_module.Name).GetAwaiter().GetResult();

        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!_module.IsInstalled)
            {
                filterContext.Result = new RedirectToActionResult("PageNotFound", "KachuwaPage", new {});
            }
        }
    }
}