using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Model
{
    [Table("SeoSetting")]
    public class SEOSetting
    {
        [Key]
        public int SEOSettingId { get; set; }
    }
}