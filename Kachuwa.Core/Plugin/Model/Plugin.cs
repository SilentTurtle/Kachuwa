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

        public string Name { get; set; }

        public string SystemName { get; set; }

        public string Image { get; set; }

        public string Version { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public bool IsInstalled { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime AddedOn { get; set; }

        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }

    }
}