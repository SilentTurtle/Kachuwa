using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Localization
{

    [Table("Resource")]
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Culture { get; set; }
        public string Type { get; set; } = "string";

        [IgnoreAll]
        public int RowTotal { get; set; }


    }
}