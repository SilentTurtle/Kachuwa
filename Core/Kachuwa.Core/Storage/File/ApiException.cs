using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Storage
{
    public class ApiException : System.Exception
    {
        private int code;
        public ApiException(int code, string msg) : base(msg)
        {
            this.code = code;
        }
    }
    public class BadRequestException : ApiException
    {
        private int code;

        public BadRequestException(string msg) : base(400, msg)
        {
            this.code = 400;
        }

        public BadRequestException(int code, string msg) : base(code, msg)
        {
            this.code = code;
        }
    }
    public class InvalidOperationException : ApiException
    {
        public InvalidOperationException(string msg) : base(401, msg)
        {

        }
    }
    public class NotFoundException : ApiException
    {
        public NotFoundException(string msg) : base(404, msg)
        {
        }
    }
    public class SampleExceptionMapper
    {
        public JsonResult Map(System.Exception exception)
        {
            JsonResult result = new JsonResult(null);

            if (exception is NotFoundException)
            {
                result.StatusCode = 404;
            }
            else if (exception is BadRequestException)
            {
                result.StatusCode = 400;
            }
            else if (exception is ApiException)
            {
                result.StatusCode = 403;
            }
            else
            {
                result.StatusCode = 500;
            }

            //result.Value = new ApiResponse(ApiResponse.ERROR, exception.Message);

            return result;
        }
    }
    public class SessionAlreadyBeingCreatedException : ApiException
    {
        public SessionAlreadyBeingCreatedException(string msg) : base(202, msg)
        {

        }
    }
}