using System;
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
        [Required(ErrorMessage = "Menu.SettingGroupName.Required")]
        public string MenuGroupName { get; set; }
        [Required(ErrorMessage = "Menu.ShowMenuAs.Required")]
        [Range(1,Int32.MaxValue,ErrorMessage = "Menu.ShowMenuAs.Required")]
        public int ShowMenuAs { get; set; }

        public string CssClasses { get; set; }
    }
}