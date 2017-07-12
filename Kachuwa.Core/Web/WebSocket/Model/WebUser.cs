using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Web
{
    public class WebUser
    {
        public long WebUserId { get; set; }
        public string ConnectionId { get; set; }

        public long UserId { get; set; }
        public string UserRoles { get; set; }
        public string Browser { get; set; }
        public string SessionId { get; set; }
    }
}
