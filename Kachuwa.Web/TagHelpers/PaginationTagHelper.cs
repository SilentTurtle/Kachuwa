using System;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{
    [HtmlTargetElement("pagination")]
    public class PaginationTagHelper : TagHelper
    {
        private readonly IViewRenderService _viewRender;
        private readonly ILogger _logger;


        [HtmlAttributeName("page")]
        public int Page { get; set; }

        [HtmlAttributeName("pageSize")]
        public int PageSize { get; set; }

        [HtmlAttributeName("rowTotal")]
        public int RowTotal { get; set; }

        [HtmlAttributeName("api")]
        public string Api { get; set; }

        public PaginationTagHelper(IViewRenderService viewRender, ILogger logger)
        {
            _viewRender = viewRender;
            _logger = logger;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            try
            {
                output.TagName = "div";
                var pager = new Pager(RowTotal, Page, PageSize);
                pager.Api = Api;
                string pagination=await _viewRender.RenderToStringAsync("", "_Pagination", pager);
                output.Content.AppendHtml(pagination);
            }
            catch (Exception e)
            {
                output.Content.AppendHtml("Component Loading Error!See Logs.");
                _logger.Log(LogType.Error, () => "Component Loading Error", e);


            }
        }
    }
}