using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web.Model
{
    [Table("Setting")]
    public class Setting
    {
        [Key]
        public int SettingId { get; set; }

        [Required]
        public string WebsiteName { get; set; }

        public string Description { get; set; }

        public string Country { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public decimal Longitude { get; set; }

        public decimal Lattitude { get; set; }

        public string Logo { get; set; }
        [Required]
        public string BaseCulture { get; set; }

        public string BaseCurrency { get; set; }

        public string CurrencyCode { get; set; }

        public string GoogleAnalyticScript { get; set; }

        public bool UseHttps { get; set; }
    }
}