using Kachuwa.Data.Extension;
using Kachuwa.Localization;
using Kachuwa.Log;
using Kachuwa.Web;
using Kachuwa.Web.API;
using Kachuwa.Web.Model;
using Kachuwa.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.API.V1
{
    [Route("api/v1/activitylog")]
    public class ActivityLogApiController : BaseApiController
    {
        private readonly IActivityLogService _service;
        private readonly ILogger _logger;
        private readonly ILocaleResourceProvider _locale;
        private readonly IScreenService _screenService;
        public ActivityLogApiController(IActivityLogService service, ILogger logger, ILocaleResourceProvider locale, IScreenService screenService)
        {
            _service = service;
            _logger = logger;
            _locale = locale;
            _screenService = screenService;
        }
        [HttpGet]
        [Route("list")]
        public async Task<dynamic> List(int screenId = 0, string actionType = "", string query = "", int pageNo = 1, int pageSize = 20)
        {
            try
            {
                string sql = "";
                sql = "select COUNT(1) OVER() AS RowTotal,u.UserName,sc.Name as ScreenName,al.* from ActivityLog al left join IdentityUser u on u.Id=al.AddedBy inner join Screen sc on sc.Id=al.ScreenId where IsCompleted=@isCompleted ";
                if (screenId > 0)
                {
                    sql += " and al.ScreenId=@screenId ";
                }
                if (!String.IsNullOrEmpty(actionType))
                {
                    sql += " and al.ActionType=@actionType";
                }
                if (!string.IsNullOrEmpty(query))
                {
                    sql += " and UserName like @userName ";
                }
                sql += " Order By al.Id  desc OFFSET @page * (@page-1) ROWS FETCH NEXT @pageSize ROWS ONLY";
                var data = await _service.CrudService.QueryListAsync(sql, new { @Page = pageNo, @pageSize = pageSize, @isCompleted = false, @screenId = screenId, @actionType = actionType, @username = query });
                return HttpResponse(200, "", data);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ExceptionResponse(e, "");
            }

        }
        [HttpGet]
        [Route("detail")]
        public async Task<dynamic> Detail(int id)
        {
            try
            {
                string sql = @"select sc.Name as ScreenName,u.UserName,  t.* from (
                            select *  from ActivityLog where Id=@id
                            )t inner join Screen sc on sc.Id=t.ScreenId
	                            left join IdentityUser u on u.Id=t.AddedBy";
                var data = await _service.CrudService.QueryAsync(sql, new { @id = id });

                if (data == null)
                {
                    return HttpResponse(400, "No Records Found", null);
                }
                if (data.ActionType == (char)ActionType.Edit)
                {
                    string sqlPrev = " select top (1) TransactionData from ActivityLog where ScreenId=@screenId and IsObsolate!=@isObsolate and RefrenceId=@refId and Id<@id order by Id Desc";
                    ActivityLog prevData = await _service.CrudService.GetAsync(sqlPrev, new { @screenId = data.ScreenId, @isObsolate = true, @refId = data.RefrenceId, @id = id });

                    if (prevData != null)
                    {
                        data.PreviousData = prevData.TransactionData;
                    }

                }
                Screen screen = await _screenService.CrudService.GetAsync(data.ScreenId);
                data.Screen = screen;
                return HttpResponse(200, "", data);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => e.Message.ToString(), e);
                return ExceptionResponse(e, "");
            }

        }

    }
}
