using System;
using System.Collections.Generic;
using System.Text;
using Kachuwa.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web
{
    //[Authorize(Roles = "Admin,Developer")]
    public class LogController : BaseController
    {
        public ILoggerService LoggerService { get; }

        public LogController(ILoggerService loggerService)
        {
            LoggerService = loggerService;
        }
        public IActionResult Index(int offset = 1, int limit = 20)
        {
            var dailyLogs = LoggerService.GetTodaysLogs(offset, limit);
            return Json(dailyLogs.Logs);
        }
        public IActionResult ByDate(int offset = 1, int limit = 20,DateTime? day=null)
        {
            //day = day ?? DateTime.Now;
            var dailyLogs = LoggerService.GetLogs(offset, limit, day.GetValueOrDefault(DateTime.Now));
            return Json(dailyLogs.Logs);
        }
    }
}
