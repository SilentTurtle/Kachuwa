using System;
using System.Threading.Tasks;
using Kachuwa.KGrid;
using Kachuwa.Log;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{
    [HtmlTargetElement("pagination")]
    public class PaginationTagHelper : TagHelper
    {
        private readonly IViewRenderService _viewRender;
        private readonly IViewComponentHelper _componentHelper;
        private readonly ILogger _logger;


        [HtmlAttributeName("page")]
        public int Page { get; set; }

        [HtmlAttributeName("pageSize")]
        public int PageSize { get; set; }

        [HtmlAttributeName("rowTotal")]
        public int RowTotal { get; set; }

        [HtmlAttributeName("api")]
        public string Api { get; set; }

        public PaginationTagHelper(IViewComponentHelper componentHelper, ILogger logger)
        {
            _componentHelper = componentHelper;
            _logger = logger;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            try
            {
                output.TagName = "div";
                var pager = new KachuwaPager(RowTotal, Page, PageSize);
                pager.Api = Api;
                output.Content.AppendHtml(await _componentHelper.InvokeAsync("KachuwaPagination", pager));
            }
            catch (Exception e)
            {
                output.Content.AppendHtml("Component Loading Error!See Logs.");
                _logger.Log(LogType.Error, () => "Component Loading Error", e);


            }
        }
    }
}