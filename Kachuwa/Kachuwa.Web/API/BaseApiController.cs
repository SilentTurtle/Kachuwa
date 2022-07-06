using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Kachuwa.Identity.Extensions;
using Kachuwa.Log;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
                Errors = new string[] { msg }
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
        public object HttpResponse(int statusCode, string msg, object data, int currentPage = 1)
        {

            return new
            {
                Code = statusCode,
                Message = msg,
                Data = data,
                CurrentPage = currentPage,
            };
        }
        public object SuccessResponse(string msg, object data)
        {
            return new
            {
                Code = 200,
                Message = msg,
                Data = data
            };
        }
        public object ErrorResponse(ModelStateDictionary modelState, int code, object data)
        {
            return new
            {
                Code = code,
                Message = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)),
                Data = data
            };
        }
        public object ExceptionResponse(Exception ex, object data)
        {
            return new
            {
                Code = 500,
                Message = ex.Message,
                Data = data
            };
        }

        public int GetCompanyId()

        {
            var claim = ((ClaimsIdentity)User.Identity).FindFirst("CompanyId");
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }
        public int GetBranchId()
        {
            var claim = ((ClaimsIdentity)User.Identity).FindFirst("BranchId");
            return (claim != null) ? Convert.ToInt32(claim.Value) : 0;
        }
    }

}