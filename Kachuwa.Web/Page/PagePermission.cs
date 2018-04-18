using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web
{
    [Table("PagePermission")]
    public class PagePermission
    {
        [Key]
        public long PagePermissionId { get; set; }

        public long MenuId { get; set; }

        public bool AllowAccessForAll { get; set; }

        public bool AllowAccess { get; set; }

        public long RoleId { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
    }
}