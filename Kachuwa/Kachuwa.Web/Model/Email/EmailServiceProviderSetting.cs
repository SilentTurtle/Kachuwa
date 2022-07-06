using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.Web
{
    [Table("EmailServiceProviderSetting")]
    public class EmailServiceProviderSetting
    {
        [Key]
        public int EmailServiceProviderSettingId { get; set; }
        [Range(1,int.MaxValue)]
        public int EmailServiceProviderId { get; set; }
        [Required]
        public string ProviderKey { get; set; }
        [Required]
        public string ProviderValue { get; set; }
    }
    public class EmailServiceProviderSettingViewModel
    {
        public string SystemName { get; set; }
    }
}