using System.Threading.Tasks;
using Kachuwa.KGrid;
using Kachuwa.Web.Module;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Grid
{
    public class KachuwaPaginationViewComponent : KachuwaModuleViewComponent<KachuwaGridModule>
    {

        public KachuwaPaginationViewComponent(IModuleManager moduleManager) : base(moduleManager)
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync(KachuwaPager pager)
        {
            return View(pager);
        }

        public override string DisplayName { get; } = "Grid Pagination";
        public override bool IsVisibleOnUI { get; } = true;
    }
}