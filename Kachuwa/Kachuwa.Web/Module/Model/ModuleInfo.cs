using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Module
{
    [Table("Module")]
    public class ModuleInfo
    {
        [Key]
        public int ModuleId { get; set; }
        [Required(ErrorMessage = "Module.Name.Required")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public bool IsInstalled { get; set; }
        public string Author { get; set; }
       
        public bool IsBuiltIn { get; set; }
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