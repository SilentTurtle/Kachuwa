//using Kachuwa.Data.Extension;
//using Kachuwa.Localization;
//using Kachuwa.Log;
//using Kachuwa.Web;
//using Kachuwa.Web.API;
//using Kachuwa.Web.Model;
//using Kachuwa.Web.Services;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Kachuwa.Web.Api
//{
//    [Route("api/v1/screen")]
//    public class ScreenApiController : BaseApiController
//    {
//        private readonly IScreenService _service;
//        private readonly ILogger _logger;
//        private readonly ILocaleResourceProvider _locale;
//        public ScreenApiController(IScreenService service, ILogger logger, ILocaleResourceProvider locale)
//        {
//            _service = service;
//            _logger = logger;
//            _locale = locale;
//        }
//        [HttpGet]
//        [Route("list")]
//        public async Task<dynamic> List(string query = "", int pageNo = 1, int pageSize = 20)
//        {
//            try
//            {
//                string condition = "";
//                if (query != "")
//                {
//                    condition = " where name like @query";
//                }
//                var data = await _service.CrudService.GetListPagedAsync(pageNo, pageSize, pageSize, condition, "Name", new { @query = "%" + query + "%" });
//                return HttpResponse(200, "", data);
//            }
//            catch (Exception e)
//            {
//                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
//                return ExceptionResponse(e, "");
//            }

//        }
//    }
//}
