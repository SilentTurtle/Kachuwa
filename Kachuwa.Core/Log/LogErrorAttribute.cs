using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Log
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LogErrorAttribute : Attribute, IExceptionFilter
    {
        private  ILogger _logger;
        public LogErrorAttribute()
        {
            
        }
        public void OnException(ExceptionContext filterContext)
        {
            _logger = filterContext.HttpContext.RequestServices.GetService<ILogger>();
            _logger.Log(LogType.Error, () => filterContext.Exception.Message.ToString(), filterContext.Exception);

        }
    }
}