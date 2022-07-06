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
        [Required(ErrorMessage = "Menu.NameRequired")]
        public string Name { get; set; }

        public string SubTitle { get; set; }
        [Required(ErrorMessage = "Menu.UrlRequired")]

        public string Url { get; set; }

        public string Icon { get; set; }

        public string CssClass { get; set; }

        public bool IsChild { get; set; }
        [IgnoreInsert]
        public int ParentId { get; set; }

        [IgnoreInsert]
        public int MenuOrder { get; set; }
        [Required]
        [Range(1,int.MaxValue,ErrorMessage = "Menu.SelectGroup")]
        public int MenuGroupId { get; set; }

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

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long AddedBy { get; set; }


        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long DeletedBy { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime DeletedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public long UpdatedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
    }

}