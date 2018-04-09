using System.ComponentModel;

namespace Kachuwa.Web.API
{
    public class ApiResponse
    {
        [Description("Status code representing states of response")]

        public int Code { get; set; }

        [Description("Response message")]
        public string Message { get; set; }

        [Description("Container of responsed data")]
        public dynamic Data { get; set; }
    }
}