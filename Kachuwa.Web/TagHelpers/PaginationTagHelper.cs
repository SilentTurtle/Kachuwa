using System;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{
    [HtmlTargetElement("pagination")]
    public class PaginationTagHelper : TagHelper
    {
        private readonly IViewRenderService _renderService;
        private readonly ILogger _logger;


        [HtmlAttributeName("page")]
        public int Page { get; set; }

        [HtmlAttributeName("pageSize")]
        public int PageSize { get; set; }

        [HtmlAttributeName("rowTotal")]
        public int RowTotal { get; set; }

        [HtmlAttributeName("api")]
        public string Api { get; set; }
        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }
        public PaginationTagHelper(IViewComponentHelper componentHelper,
            IViewRenderService renderService, ILogger logger)
        {
            _componentHelper =  componentHelper as DefaultViewComponentHelper;
            _renderService = renderService;
            _logger = logger;
        }
        private readonly DefaultViewComponentHelper _componentHelper;
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            try
            {
                output.TagName = "div";
                var pager = new Pager(RowTotal, Page, PageSize);
                pager.Api = Api;
                _componentHelper.Contextualize(ViewContext);
                output.Content.AppendHtml(
                    await _componentHelper.InvokeAsync("Pagination", pager)
                );
                //output.Content.AppendHtml(await _renderService.RenderToStringAsync("_Pagination", pager));
            }
            catch (Exception e)
            {
                output.Content.AppendHtml("Component Loading Error!See Logs.");
                _logger.Log(LogType.Error, () => "Component Loading Error", e);


            }
        }
    }
}