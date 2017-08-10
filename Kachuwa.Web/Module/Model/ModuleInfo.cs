using System;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;

namespace Kachuwa.Web.Module
{
    [Table("Module")]
    public class ModuleInfo
    {
        [Key]
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public bool IsInstalled { get; set; }
        public string Author { get; set; }
        public bool IsActive { get; set; }
        [IgnoreUpdate]
        public bool IsDeleted { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
    }
}