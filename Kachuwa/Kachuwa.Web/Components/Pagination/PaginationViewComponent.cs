using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web.Components
{
    [ViewComponent(Name = "Pagination")]
    public class PaginationViewComponent : KachuwaViewComponent
    {

        public PaginationViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(Pager pager)
        {
            return View(pager);
        }

        public override string DisplayName { get; } = "Kachuwa Pagination";
        public override bool IsVisibleOnUI { get; } = false;
    }
}