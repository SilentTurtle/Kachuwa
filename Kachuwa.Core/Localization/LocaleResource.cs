using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Localization
{

    [Table("LocaleResource")]
    public class LocaleResource
    {
        [Key]
        public int LocaleResourceId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Culture { get; set; }
        public string GroupName { get; set; } = "";

        [IgnoreAll]
        public int RowTotal { get; set; }


    }
}