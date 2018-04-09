using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace Kachuwa.Web.Security
{
    public class PagePermissionRequirement : IAuthorizationRequirement
    {
        public string PageName { get; }
        public string[] PageRoutes { get; }
        public PagePermissionRequirement()
        {
            //PageName = pageName ?? throw new ArgumentNullException(nameof(pageName));
            //PageRoutes = routes ?? throw new ArgumentNullException(nameof(routes));
        }
    }
}
