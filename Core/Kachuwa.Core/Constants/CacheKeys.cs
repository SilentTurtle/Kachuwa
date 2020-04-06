using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Core
{
    public class CacheKeys
    {
        public const string PagePemissions= "Kachuwa.Page.Permissions";
        public const string LocaleRegion = "Kachuwa.LocaleRegion";
    }
    public class ApplicationClaim
    {
        public static readonly string CustomerId = "_customerId";
        public static readonly string SessionCode = "_sessionCode";
        public static readonly string Anonymous = "__anonymous";
        public static readonly string OnlineId = "_onlineId";
    }
}
