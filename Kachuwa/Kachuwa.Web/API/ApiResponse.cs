using System.ComponentModel;

namespace Kachuwa.Web.API
{
    public class ApiResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
    }
}