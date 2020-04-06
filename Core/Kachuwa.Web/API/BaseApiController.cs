using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Kachuwa.Identity.Extensions;
using Kachuwa.Log;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Kachuwa.Web.API
{
    [LogError]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiAuthorize]
    public class BaseApiController : Controller
    {
        private string _sessionCode;
        public string SessionCode
        {
            get { return User.Identity.GetSessionId(); }
            set { _sessionCode = value; }
        }
        public BaseApiController()
        {
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public object HttpResponse(int statusCode, string msg, object data)
        {

            return new 
            {
                Code = statusCode,
                Message = msg,
                Data = data
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public object HttpResponse(int statusCode, string msg)
        {
            return new 
            {
                Code = statusCode,
                Message = msg,
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public object ValidationResponse(List<string> errors)
        {
            
            return new
            {
                Code = 600,
                Message = "Validation Error",
                Errors = errors ?? new List<string>()
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public object NotAuthorizedResponse()
        {

            return new
            {
                Code = 401,
                Message = "Unauthorized Request"
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public object ErrorResponse(int statusCode, string msg)
        {

            return new 
            {
                Code = statusCode,
                Message = msg,
                Errors=new string[] { msg }
            };
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public object ErrorResponse(string[] msgs)
        {
            return new 
            {
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Error",
                Errors = msgs
            };
        }

    }

}