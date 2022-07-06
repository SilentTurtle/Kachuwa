using System;
using System.Collections.Generic;
using System.Text;
using Kachuwa.Data.Crud.Attribute;
using  System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.Web.Model
{
    [Table("Country")]
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "Country.Iso.Required")]
        [MaxLength(2,ErrorMessage = "Country.TwoLetterOnly")]
        public string ISO { get; set; }

        [Required(ErrorMessage = "Country.Name.Required")]
        public string Name { get; set; }

        public string NiceName { get; set; }

        public string LocaleName { get; set; }
        [MaxLength(3, ErrorMessage = "Country.ThreeLetterOnly")]
        public string ISO3 { get; set; }

        public short Numcode { get; set; }

        public int PhoneCode { get; set; }

        public string CurrencySymbol { get; set; }

        public string CurrencyCode { get; set; }

        public string Currency { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}
