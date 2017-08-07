using System;
using System.ComponentModel.DataAnnotations;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web.Model
{
    [Table("Menu")]
    public class Menu
    {
        [Data.Crud.Attribute.Key]
        public int MenuId { get; set; }
        [Required]
        public string Name { get; set; }

        public string SubTitle { get; set; }
        [Required]
        public string Url { get; set; }

        public string Icon { get; set; }

        public string CssClass { get; set; }

        public bool IsChild { get; set; }

        public int ParentId { get; set; }

        public int MenuOrder { get; set; }

        public string GroupName { get; set; }

        public bool IsBackend { get; set; }

        public bool IsActive { get; set; }
        [AutoFill(false)]
        public bool IsDeleted { get; set; }
        [AutoFill(IsDate = true)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [AutoFill(GetCurrentUser = true)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}