using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Model
{
    [Table("Menu")]
    public class Menu
    {
        [Key]
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
        [Required]
        public string GroupName { get; set; }

        public bool IsBackend { get; set; }
        public bool IsSystem { get; set; }
        [AutoFill(AutoFillProperty.CurrentCulture)]
        public string Culture { get; set; }

        public bool IsActive { get; set; }
        [AutoFill(false)]
        public bool IsDeleted { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }

}