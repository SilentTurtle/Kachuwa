using System.ComponentModel.DataAnnotations;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Model
{
    [Table("MenuSetting")]
    public class MenuSetting
    {
        [Data.Crud.Attribute.Key]
        public int MenuSettingId { get; set; }
        [Required]
        public string MenuGroupName { get; set; }
        [Required]
        public int ShowMenuAs { get; set; }

        public string CssClasses { get; set; }
    }
}