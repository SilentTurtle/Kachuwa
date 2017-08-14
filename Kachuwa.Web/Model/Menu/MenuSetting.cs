using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Model
{
    [Table("MenuSetting")]
    public class MenuSetting
    {
        [Key]
        public int MenuSettingId { get; set; }
        [Required]
        public string MenuGroupName { get; set; }
        [Required]
        public int ShowMenuAs { get; set; }

        public string CssClasses { get; set; }
    }
}