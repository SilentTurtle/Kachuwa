using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web.API
{
    public class BaseApiController : Controller
    {
        public ApiResponse HttpResponse(int statusCode, string msg, object data)
        {
            
            return new ApiResponse
            {
                Code = statusCode,
                Message = msg,
                Data = data
            };
        }

        public ApiResponse HttpResponse(int statusCode, string msg)
        {
            return new ApiResponse
            {
                Code = statusCode,
                Message = msg,
            };
        }

        public ApiResponse ErrorResponse(int statusCode, string msg)
        {
           
            return new ApiResponse
            {
                Code = statusCode,
                Message = msg,
            };
        }
        public ApiResponse ErrorResponse(string msg)
        {
            return new ApiResponse
            {
                Code = (int)HttpStatusCode.BadRequest,
                Message = msg
            };
        }
    }
}