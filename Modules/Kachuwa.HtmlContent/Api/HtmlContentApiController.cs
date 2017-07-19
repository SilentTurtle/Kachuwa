using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.HtmlContent.Service;
using Kachuwa.Log;
using Kachuwa.Web.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Kachuwa.HtmlContent.Api
{
    [Route("api/v1/htmlcontent")]
    public class HtmlContentApiController : BaseApiController
    {

        public IHtmlContentService Service { get; }
        public ILogger Logger { get; }

        public HtmlContentApiController(IHtmlContentService htmlContentService,ILogger logger)
        {
            Service = htmlContentService;
            Logger = logger;
        }

        [HttpGet]
        [Route("{key}")]
        public async Task<ApiResponse> Get([FromRoute]string key)
        {
            try
            {
                var data = await Service.HtmlService.GetAsync("Where IsActive=1 and KeyName=@KeyName", new { KeyName = key });

                return HttpResponse(StatusCodes.Status200OK, "", data);
            }
            catch (Exception e)
            {
                Logger.Log(LogType.Error, () => e.Message, e);
                return HttpResponse(StatusCodes.Status500InternalServerError, "","Server Error.");
            }
           
        }
        [HttpGet]
        [Route("preview/{key}")]
        //api/v1/htmlcontent/preview/key
        public async Task<ApiResponse> PreviewHtml([FromRoute]string key)
        {
            try
            {
                var data = await Service.HtmlService.GetAsync("Where KeyName=@KeyName", new { KeyName = key });
                if (data != null)
                {
                    if (data.IsMarkDown)
                    {
                        //TODO render markdown

                        return HttpResponse(StatusCodes.Status200OK, "", data.Content);
                    }
                    return HttpResponse(StatusCodes.Status200OK, "", data.Content);
                }
                else
                {
                    return HttpResponse(StatusCodes.Status404NotFound, "", "Content not found.");
                }

              
            }
            catch (Exception e)
            {
                Logger.Log(LogType.Error, () => e.Message, e);
                return HttpResponse(StatusCodes.Status500InternalServerError, "", "Server Error.");
            }
           
        }

        
    }
}
