using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Plugin
{
    [Table("Plugin")]
    public class Plugin
    {
        [Key]
        public int PluginId { get; set; }

        public int PluginType { get; set; }
        [Required(ErrorMessage = "Plugin.Name.Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Plugin.SystemName.Required")]
        public string SystemName { get; set; }

        public string Image { get; set; }

        public string Version { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public bool IsInstalled { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
        [IgnoreUpdate]
        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime AddedOn { get; set; }
        [IgnoreUpdate]
        [AutoFill(AutoFillProperty.CurrentUser)]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }

    }
}