using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Kachuwa.Log;
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
        public object HttpResponse(int statusCode, string msg, object data)
        {

            return new 
            {
                Code = statusCode,
                Message = msg,
                Data = data
            };
        }

        public object HttpResponse(int statusCode, string msg)
        {
            return new 
            {
                Code = statusCode,
                Message = msg,
            };
        }
        public object ValidationResponse(List<string> errors)
        {
            
            return new
            {
                Code = 600,
                Message = "Validation Error",
                Errors = errors ?? new List<string>()
            };
        }
        public object ErrorResponse(int statusCode, string msg)
        {

            return new 
            {
                Code = statusCode,
                Message = msg,
                Errors=new string[] { msg }
            };
        }
        public object ErrorResponse(string msg)
        {
            return new 
            {
                Code = (int)HttpStatusCode.BadRequest,
                Message = msg,
                Errors = new string[] { msg }
            };
        }

    }

}