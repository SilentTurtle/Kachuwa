using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Model
{
   [Table("MenuGroup")]
    public class MenuGroup
    {
        [Key]
        public int MenuGroupId { get; set; }

        [Required(ErrorMessage = "MenuGroup.NameRequired")]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }

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
