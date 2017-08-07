using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Model
{
    [Table("SeoSetting")]
    public class SEOSetting
    {
        [Data.Crud.Attribute.Key]
        public int SEOSettingId { get; set; }
    }
}