using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Model
{
    [Table("Setting")]
    public class Setting
    {
        [Key]
        public int SettingId { get; set; }

        [Required(ErrorMessage ="Setting.WebSiteName.Required")]
        public string WebsiteName { get; set; }

        public string Description { get; set; }

        public string Country { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string State { get; set; }

        public string City { get; set; }
        public string TimeZoneOffset { get; set; }
        public string TimeZoneName { get; set; }

        public decimal Longitude { get; set; }

        public decimal Lattitude { get; set; }
        [IgnoreAll]
        public IFormFile LogoFile { get; set; }
        public string Logo { get; set; }
        [Required(ErrorMessage = "Setting.BaseCulture.Required")]
        public string BaseCulture { get; set; }
        [Required(ErrorMessage = "Setting.BaseCurrency.Required")]
        public string BaseCurrency { get; set; }
        [Required(ErrorMessage = "Setting.BaseCurrency.Required")]
        public string CurrencyCode { get; set; }

        public string GoogleAnalyticScript { get; set; }

        public bool UseHttps { get; set; }
        public string DefaultEmail { get; set; }
        public string SupportEmail { get; set; }
        public string SalesEmail { get; set; }
        public string MarketingEmail { get; set; }
    }
}