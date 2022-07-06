using System.Collections.Generic;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web
{
    public class PageRolePermissionViewModel
    {
        public string PageName { get; set; }
        public int PageId { get; set; }
        public List<PagePermissionViewModel> PagePermissions { get; set; }
        public int[] PagePermissionsRole { get; set; }
        [AutoFill(true)]
        public string AddedBy { get; set; }
    }
}
